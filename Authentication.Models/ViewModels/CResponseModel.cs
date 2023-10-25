using static Authentication.Models.Helpers.Enums;

namespace Authentication.Models.ViewModels
{
    /// <summary>
    /// Modal for Common Response modal of each API request
    /// </summary>
    public class CResponseModel
    {
        public ResponseStatusCode SuccessCode { get; set; }
        public bool IsSuccess { get; set; }
        public dynamic Data { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public dynamic ExceptionObject { get; set; }

    }
}
