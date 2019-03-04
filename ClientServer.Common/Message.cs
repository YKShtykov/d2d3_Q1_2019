namespace ClientServer.Common
{ 
    using System;

    public class Message:IEquatable<Message>
    {
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return $"{Time}: {Author} - {Text}";
        }

        public bool Equals(Message other)
        {
            return Author == other.Author &&
                   Text == other.Text &&
                   Time == other.Time;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (!ReferenceEquals(obj,this))
            {
                return false;
            }

            if (!(obj is Message))
            {
                return false;
            }

            return Equals(obj as Message);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}