namespace TrackAPI.Exceptions
{
    public class WrongLoginException : Exception
    {
        public WrongLoginException(string message) : base(message) 
        {
            
        }
    }
}
