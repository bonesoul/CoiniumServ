using System.Linq;
using Nancy.Responses.Negotiation;

namespace Nancy.CustomErrors
{
    public static class ResponseFormatterExtensions
    {
        public static Response AsError(this IResponseFormatter formatter, string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            var serializer = formatter.SerializerFactory.GetSerializer(new MediaRange("application/json"));

            return new ErrorResponse(new Error {Message = message}, serializer, formatter.Environment).WithStatusCode(statusCode);
        }
    }
}