namespace CentralServer
{
    using System;

    internal class ChunkedFile
    {
        private byte[] _fileBytes;

        public ChunkedFile(byte[] fileBytes, int parts)
        {
            _fileBytes = fileBytes;
            IncludedParts = 1;
            Parts = parts;
        }

        public ChunkedFile(int parts)
        {
            Parts = parts;
        }

        public byte[] ConstructedArray => _fileBytes;

        /// <summary>
        ///     Number of parts file consists of.
        /// </summary>
        public int Parts { get; }

        /// <summary>
        ///     Number of parts already included.
        /// </summary>
        public int IncludedParts { get; private set; }

        /// <summary>
        ///     Returns true if file is comleted.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool AppendFilePart(byte[] part, int position)
        {
            if (IncludedParts >= position)
                throw new Exception(
                    $"Attempt to insert the {position}th part of the file. But last part is {IncludedParts}.");

            var currentFileSize = _fileBytes?.Length ?? 0;
            Array.Resize(ref _fileBytes, currentFileSize + part.Length);
            part.CopyTo(_fileBytes, currentFileSize);
            IncludedParts += 1;

            return IncludedParts == Parts;
        }
    }
}