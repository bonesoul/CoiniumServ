using System;
using System.Collections.Generic;

namespace Nancy.CustomErrors
{
    public class CustomErrorsConfiguration
    {
        public string NotFoundTitle = "404 Not Found";
        public string NotFoundSummary = "The requested resource could not be found.";
        public string ForbiddenTitle = "Forbidden";
        public string ForbiddenSummary = "You do not have permission to do that.";
        public string UnauthorizedTitle = "Unauthorized";
        public string UnauthorizedSummary = "You do not have permission to do that.";
        public string ErrorTitle = "Error";
        public string ErrorSummary = "An unexpected error occurred.";

        public bool AlwaysReturnJson = false;

        /// <summary>
        /// If set to true, then we will emit full stack traces in our ErrorResponse
        /// </summary>
        public bool Debug = false;

        /// <summary>
        /// Converts a thrown exception to the appropriate ErrorResponse. Override this method if you need
        /// to handle custom exception types, or implement your own error handling logic. The default 
        /// implementation converts all thrown exceptions to a regular ErrorResponse with an HttpStatusCode
        /// of 500
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public virtual ErrorResponse HandleError(NancyContext context, Exception ex, ISerializer serializer)
        {
            var error = new Error
            {
                FullException = ex.ToString(),
                Message = ex.Message
            };

            return new ErrorResponse(error, serializer, context.Environment).WithStatusCode(HttpStatusCode.InternalServerError) as ErrorResponse;
        }

        /// <summary>
        /// Maps different HttpStatusCodes to the appropriate views.
        /// </summary>
        public IDictionary<HttpStatusCode, string> ErrorViews = new Dictionary<HttpStatusCode, string>
            {
                { HttpStatusCode.NotFound,              "Error" },
                { HttpStatusCode.InternalServerError,   "Error" },
                { HttpStatusCode.Forbidden,             "Error" }
            };
    }
}
