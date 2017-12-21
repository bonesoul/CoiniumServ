using Nancy.Configuration;
using Nancy.Responses;

namespace Nancy.CustomErrors
{
    public class ErrorResponse : JsonResponse
    {
        private readonly Error _error;
        public string ErrorMessage { get { return _error.Message; } }
        public string FullException { get { return _error.FullException; } }
        public ErrorResponse(Error error, ISerializer serializer, INancyEnvironment environment) : base(error, serializer, environment)
        {
            if (!CustomErrors.Configuration.Debug)
            {
                error.FullException = null;
            }

            _error = error;
        }
    }
}
