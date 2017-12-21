using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using Nancy.Responses;

namespace Nancy.CustomErrors.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new NancyHost(new Uri("http://localhost:1234")))
            {
                host.Start();
                Console.ReadKey();
            }
        }
    }

    public class TestBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            CustomErrors.Enable(pipelines, new ErrorConfiguration(), new DefaultJsonSerializer(this.GetEnvironment()));
        }
    }

    public class TestModule : NancyModule
    {
        public TestModule()
            : base("/")
        {
            Get("/test", _ =>
            {
                var response = Response.AsText("test", "application/json");
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            });

            Get("/err", _ =>
            {
                throw new Exception("asdadsfdaf");
            });
        }
    }

    public class ErrorConfiguration : CustomErrorsConfiguration
    {
        public ErrorConfiguration()
        {
            // Map error status codes to custom view names
            ErrorViews[HttpStatusCode.NotFound] = "error";
            ErrorViews[HttpStatusCode.InternalServerError] = "error";
            ErrorViews[HttpStatusCode.Forbidden] = "error";
        }
        public override ErrorResponse HandleError(NancyContext context, Exception ex, ISerializer serializer)
        {
            var error = new Error
            {
                FullException = ex.ToString(),
                Message = ex.Message
            };

            return new ErrorResponse(error, serializer, context.Environment).WithStatusCode(HttpStatusCode.InternalServerError) as ErrorResponse;
        }
    }
}
