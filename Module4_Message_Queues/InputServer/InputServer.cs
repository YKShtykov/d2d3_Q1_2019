namespace InputServer
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Timers;
    using Autofac.Extras.DynamicProxy;
    using Common;
    using MessageProcessors;
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.Rendering;
    using Image = MigraDoc.DocumentObjectModel.Shapes.Image;
    using Timer = System.Timers.Timer;

    [Intercept(typeof(CallLogger))]
    internal class InputServer : IServer
    {
        private static Timer _completeFileTimer;
        private static Timer _sendStatusTimer;
        private readonly Regex _imageNumberPattern;
        private readonly IMessageSender _pdfMessageSender;
        private readonly int _sendStatusInterval = 10000;
        private readonly ServerSettings _serviceSettings;
        private readonly SettingsReceiver _settingsReceiver;
        private readonly AutoResetEvent _startWorkEvent;
        private readonly StatusMessageSender _statusSender;
        private readonly ManualResetEvent _stopEvent;
        private readonly FileSystemWatcher _watcher;
        private readonly Thread _workThread;
        private Document _document;
        private string _fileCorruptedDirectory;
        private string _fileMonitorDirectory;
        private string _fileOutputDirectory;
        private Regex _imageNamePattern;
        private int _pdfFileNumber;
        private Regex _pdfNamePattern;
        private ServerStatus _serviceStatus;

        public InputServer()
        {
            SetServiceConfiguration();
            CreateDerictories(_fileMonitorDirectory, _fileOutputDirectory, _fileCorruptedDirectory);

            _imageNumberPattern = new Regex(@"\d+");
            _startWorkEvent = new AutoResetEvent(false);
            _stopEvent = new ManualResetEvent(false);
            _serviceStatus = new ServerStatus
            {
                Address = Environment.MachineName,
                Code = "01",
                Description = "Waiting for processing."
            };

            _serviceSettings = new ServerSettings
            {
                UpdateStatus = false,
                PageTimeout = 10000
            };

            ValidateOutputFolder();
            _document = new Document();
            _workThread = new Thread(WorkProcedure);
            _watcher = new FileSystemWatcher(_fileMonitorDirectory);
            _watcher.Created += WatcherOnCreated;


            SetupTimerSettings(ref _completeFileTimer, _serviceSettings.PageTimeout, OnCompliteFileEvent);
            SetupTimerSettings(ref _sendStatusTimer, _sendStatusInterval, OnSendStatusEvent);
            _pdfMessageSender = new FileMessageSender();
            _statusSender = new StatusMessageSender();
            _settingsReceiver = new SettingsReceiver();
            _settingsReceiver.ReceiveMessageFromSubscription(ApplyServiceSettings);
        }


        public void Start()
        {
            _workThread.Start();
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _stopEvent.Set();
            _workThread.Join();
            _settingsReceiver.CancelProcessing();
        }

        private void CreateDerictories(params string[] directories)
        {
            foreach (var directory in directories.Where(directory => !Directory.Exists(directory)))
                Directory.CreateDirectory(directory);
        }

        private void WorkProcedure(object obj)
        {
            var addedPaths = new List<string>();
            var firstFilePath = GetSortedPaths().FirstOrDefault();
            var prevImgNumber = -1;
            var section = _document.AddSection();

            if (firstFilePath != null)
                prevImgNumber = int.Parse(GetImageNumber(firstFilePath));

            do
            {
                var filePaths = GetSortedPaths().ToList();

                if (filePaths.Count == 0)
                    continue;

                _serviceStatus.Code = "01";
                _serviceStatus.Description = "Processing a sequence.";

                foreach (var filePath in filePaths)
                {
                    var curImgNumber = int.Parse(GetImageNumber(filePath));
                    if (prevImgNumber + 1 != curImgNumber && prevImgNumber != curImgNumber && prevImgNumber != -1)
                    {
                        RenderAndStartNewDocument();
                        section = _document.AddSection();
                        addedPaths.Clear();
                    }

                    if (_stopEvent.WaitOne(TimeSpan.Zero))
                        return;

                    if (!TryOpen(filePath, 3) || addedPaths.Contains(filePath))
                        continue;

                    if (!RotateImageIfValid(filePath))
                    {
                        ExportCorruptedSequence(section, filePath);
                        _document = new Document();
                        section = _document.AddSection();
                        continue;
                    }

                    var image = section.AddImage(filePath);

                    ConfigureImage(image);
                    addedPaths.Add(filePath);
                    ApplyServiceSettings(_serviceSettings);
                    section.AddPageBreak();
                    prevImgNumber = curImgNumber;

                    RenderDocument();
                }

                _serviceStatus.Code = "01";
                _serviceStatus.Description = "Waiting for processing.";
            } while (WaitHandle.WaitAny(new WaitHandle[] {_stopEvent, _startWorkEvent}, Timeout.Infinite) != 0);

            RenderDocument();
        }

        private IEnumerable<string> GetSortedPaths()
        {
            var filePaths = Directory.EnumerateFiles(_fileMonitorDirectory);
            filePaths = filePaths.Where(p => ValidateFileName(p, _imageNamePattern));
            return SortPaths(filePaths);
        }

        private IEnumerable<string> SortPaths(IEnumerable<string> paths)
        {
            var orderedPaths = paths.OrderBy(GetImageNumber);

            return orderedPaths;
        }

        private string GetImageNumber(string filePath)
        {
            if (filePath == null)
                return string.Empty;

            var matches = _imageNumberPattern.Matches(filePath);
            var lastMatch = matches[matches.Count - 1];

            return lastMatch.Value;
        }

        private void ExportCorruptedSequence(Section section, string corruptedFilePath)
        {
            MoveCorruptedFile(corruptedFilePath);
            var imagePaths = GetImagePathsFromFile(section);
            foreach (var filePath in imagePaths) MoveCorruptedFile(filePath);
        }

        private void MoveCorruptedFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            File.Move(filePath, Path.Combine(_fileCorruptedDirectory, fileName));
        }

        private void DeleteManagedSequence(Document document)
        {
            foreach (var filePath in document.Sections.Cast<Section>()
                .Select(GetImagePathsFromFile).SelectMany(imagePaths => imagePaths))
                File.Delete(filePath);
        }

        private IEnumerable<string> GetImagePathsFromFile(Section section)
        {
            return section.Elements.OfType<Image>().Select(
                image => image.GetFilePath(_fileMonitorDirectory)).ToList();
        }

        private void ValidateOutputFolder()
        {
            var existingPdfFiles = Directory.EnumerateFiles(_fileOutputDirectory);
            foreach (var filePath in existingPdfFiles)
            {
                var fileName = Path.GetFileName(filePath);
                if (ValidateFileName(filePath, _pdfNamePattern))
                {
                    var fileNumber = _imageNumberPattern.Match(fileName);
                    if (int.TryParse(fileNumber.Value, out var num) && _pdfFileNumber < num)
                        _pdfFileNumber = num;
                }
            }
        }

        private void RenderAndStartNewDocument()
        {
            RenderDocument();
            _document = new Document();
        }

        /// <summary>
        ///     Rotates image it has correct format.
        /// </summary>
        /// <param name="filePath">Path to image.</param>
        /// <returns>Returns false if the file does not have a valid image format.</returns>
        private bool RotateImageIfValid(string filePath)
        {
            try
            {
                System.Drawing.Image image;
                using (image = System.Drawing.Image.FromFile(filePath))
                {
                    if (image.Height < image.Width)
                    {
                        image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        image.Save(filePath);
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                return false;
            }

            return true;
        }

        private void SetServiceConfiguration()
        {
            var imageNamePattern = ConfigurationManager.AppSettings["ImageNamePattern"];
            var pdfNamePattern = ConfigurationManager.AppSettings["PdfNamePattern"];
            _imageNamePattern = new Regex(imageNamePattern, RegexOptions.CultureInvariant);
            _pdfNamePattern = new Regex(pdfNamePattern, RegexOptions.CultureInvariant);

            var moduleFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var inputDirectory = ConfigurationManager.AppSettings["DirectoryMonitorPath"];
            var outputDirectory = ConfigurationManager.AppSettings["DirectoryOutputPath"];
            var corruptedDirectory = ConfigurationManager.AppSettings["DirectoryCorruptedSequencePath"];
            _fileMonitorDirectory = inputDirectory == "" ? Path.Combine(moduleFolder, "input") : inputDirectory;
            _fileOutputDirectory = outputDirectory == "" ? Path.Combine(moduleFolder, "output") : outputDirectory;
            _fileCorruptedDirectory =
                corruptedDirectory == "" ? Path.Combine(moduleFolder, "corrupted") : corruptedDirectory;
        }

        private bool ValidateFileName(string filePath, Regex filePattern)
        {
            var fileName = Path.GetFileName(filePath);
            return filePattern.IsMatch(fileName);
        }

        private void ConfigureImage(Image image)
        {
            image.Height = _document.DefaultPageSetup.PageHeight;
            image.RelativeVertical = RelativeVertical.Page;
            image.RelativeHorizontal = RelativeHorizontal.Page;
            image.Width = _document.DefaultPageSetup.PageWidth;
        }

        private void RenderDocument()
        {
            var renderedDocument = _document;
            var pdfRenderer = new PdfDocumentRenderer
            {
                Document = renderedDocument
            };
            _document = new Document();
            _document.AddSection();

            pdfRenderer.RenderDocument();

            using (var ms = new MemoryStream())
            {
                pdfRenderer.Save(ms, false);
                var arr = ms.ToArray();
                File.WriteAllBytes("hello.pdf", arr);
                _pdfMessageSender.SendMessage(ms);
            }

            DeleteManagedSequence(renderedDocument);
        }

        private void WatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            _startWorkEvent.Set();
        }

        private void SetupTimerSettings(ref Timer timer, double interval, ElapsedEventHandler onTimeEvent)
        {
            timer = new Timer {Interval = interval};
            timer.Elapsed += onTimeEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void ApplyServiceSettings(ServerSettings settings)
        {
            _completeFileTimer.Interval = settings.PageTimeout * 1000;

            if (settings.UpdateStatus)
                _statusSender.SendMessage(_serviceStatus);

            settings.UpdateStatus = false;
        }

        private void OnCompliteFileEvent(object source, ElapsedEventArgs e)
        {
            if (_document.LastSection != null && _document.LastSection.Elements.Count > 0)
            {
                var previousServiceStatus = _serviceStatus;
                _serviceStatus = new ServerStatus
                {
                    Address = previousServiceStatus.Address,
                    Code = "01",
                    Description = "Processing a sequence."
                };

                RenderAndStartNewDocument();
                _serviceStatus = previousServiceStatus;
            }

            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
        }

        private void OnSendStatusEvent(object source, ElapsedEventArgs e)
        {
            _statusSender.SendMessage(_serviceStatus);
        }

        private bool TryOpen(string path, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
                try
                {
                    var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    file.Close();

                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(4000);
                }

            return false;
        }
    }
}