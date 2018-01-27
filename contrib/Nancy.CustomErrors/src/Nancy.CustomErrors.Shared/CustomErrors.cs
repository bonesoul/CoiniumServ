using System;
using Nancy.Bootstrapper;
using Nancy.Responses;

namespace Nancy.CustomErrors
{
    public class CustomErrors
    {
        private static CustomErrorsConfiguration _configuration;
        public static CustomErrorsConfiguration Configuration
        {
            get { return _configuration ?? (_configuration = new CustomErrorsConfiguration()); }
        }

		public static void Enable(IPipelines pipelines, CustomErrorsConfiguration configuration, ISerializer serializer)
        {
            if (pipelines == null)
            {
                throw new ArgumentNullException("pipelines");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _configuration = configuration;
            
            pipelines.OnError.AddItemToEndOfPipeline(GetErrorHandler(configuration, serializer));
        }

        private static Func<NancyContext, Exception, Response> GetErrorHandler(CustomErrorsConfiguration configuration, ISerializer serializer)
        {
            return (context, ex) => configuration.HandleError(context, ex, serializer);
        }
    }
}