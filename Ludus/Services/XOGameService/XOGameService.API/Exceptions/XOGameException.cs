using XOGameService.API.Exceptions.Enums;

namespace XOGameService.API.Exceptions
{
    public class XOGameException : Exception
    {
        public GameErrorCode Code {  get; }

        public XOGameException(GameErrorCode code, string message) : base(message) 
        { 
            Code = code; 
        }
    }
}
