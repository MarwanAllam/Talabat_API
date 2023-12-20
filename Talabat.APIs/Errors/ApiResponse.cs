
namespace Talabat.Apis.Errors
{
    public class ApiResponse
    {

        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public ApiResponse(int statusCode ,string? message=null )
        {
            StatusCode=statusCode;
            Message=message ?? GetDefaultMessageForStatusCode(StatusCode);

        }

        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            //500 ==>Internal Server Error
            //400==>BadRequest 
            //404==>NotFound
            //401==>Unthorizored
            //Switch Expression in C# 7
            return statusCode switch
            {
                400 => "BadRequest",
                401 => "You Are Not Authorized",
                404 => "Resource Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}
