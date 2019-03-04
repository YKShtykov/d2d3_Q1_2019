namespace Client
{
    using System;

    internal class Message:IEquatable<Message>
    {
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public override string ToString()
        {
            return $"{Time:HH:mm:ss}: {Author} - {Text}";
        }
        public bool Equals(Message other)
        {
            return Author == other.Author &&
                   Text == other.Text &&
                   Time == other.Time;
        }
    }
}