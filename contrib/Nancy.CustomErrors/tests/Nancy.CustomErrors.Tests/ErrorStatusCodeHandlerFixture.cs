using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Configuration;
using Nancy.Responses;
using Nancy.Testing;
using Xunit;

namespace Nancy.CustomErrors.Tests
{
    public class ErrorStatusCodeHandlerFixture
    {
        private readonly CustomErrorsConfiguration configuration;
        private readonly Browser browser;

        public ErrorStatusCodeHandlerFixture()
        {
            configuration = new CustomErrorsConfiguration();
            
            browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                with.ApplicationStartup((container, pipelines) => CustomErrors.Enable(pipelines, configuration, new DefaultJsonSerializer(new DefaultNancyEnvironment())));
                with.Module<TestModule>();
                with.StatusCodeHandler<ErrorStatusCodeHandler>();
            }));
        }


        [Fact]
        public void Should_return_custom_error_response_for_route_not_found()
        {
            var response = browser.Get("/nuffin", with => with.Header("Accept", "application/json")).Result;

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("The requested resource could not be found.", response.Body.DeserializeJson<Error>().Message);
        }

        [Fact]
        public void Should_return_json_for_application_json_accept_header()
        {
            var response = browser.Get("/error", with => with.Header("Accept", "application/json")).Result;

            Assert.NotNull(response.Body.DeserializeJson<Error>());
        }

        [Fact]
        public void Should_return_json_for_text_json_accept_header()
        {
            var response = browser.Get("/error", with => with.Header("Accept", "text/json")).Result;

            Assert.NotNull(response.Body.DeserializeJson<Error>());
        }
        
        [Fact]
        public void Should_return_html_for_text_html_accept_header()
        {
            var response = browser.Get("/error", with => with.Header("Accept", "text/html")).Result;

            response.Body["title"].ShouldExistOnce().And.ShouldContain(configuration.ErrorTitle);
        }

        [Fact]
        public void Should_return_html_no_accept_header()
        {
            var response = browser.Get("/error").Result;

            response.Body["title"].ShouldExistOnce().And.ShouldContain(configuration.ErrorTitle);
        }

        [Fact]
        public void Should_return_custom_error_response_for_uncaught_exception()
        {
            var response = browser.Get("/error", with => with.Header("Accept", "application/json")).Result;

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("ERROR MESSAGE HERE", response.Body.DeserializeJson<Error>().Message);
        }

        [Fact]
        public void Should_return_custom_error_response_for_forbidden()
        {
            var response = browser.Get("forbidden", with => with.Header("Accept", "application/json")).Result;

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.NotNull(response.Body.DeserializeJson<Error>());
        }

        [Fact]
        public void Should_return_custom_error_response_for_unauthorised()
        {
            var response = browser.Get("unauthorised", with => with.Header("Accept", "application/json")).Result;

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotNull(response.Body.DeserializeJson<Error>());
        }

        [Fact]
        public void Should_render_custom_html_for_uncaught_exception()
        {
            var response = browser.Get("error").Result;

            response.Body["title"].ShouldExistOnce().And.ShouldContain(configuration.ErrorTitle);
            response.Body["h1"].ShouldExistOnce().And.ShouldContain("ERROR MESSAGE HERE");
        }

        [Fact]
        public void Should_suppress_full_stack_trace_by_default()
        {
            var response = browser.Get("/err", with => with.Header("Accept", "application/json")).Result;

            Assert.Null(response.Body.DeserializeJson<Error>().FullException);
        }

        [Fact]
        public void Should_expose_full_stack_trace_in_debug_mode()
        {
            CustomErrors.Configuration.Debug = true;

            var response = browser.Get("/error", with => with.Header("Accept", "application/json")).Result;
            
            Assert.NotNull(response.Body.DeserializeJson<Error>().FullException);

            CustomErrors.Configuration.Debug = false;
        }

        [Fact]
        public void Should_retain_headers_already_set()
        {
            var response = browser.Get("/headers", with => with.Header("Accept", "application/json")).Result;

            Assert.NotNull(response.Headers.Where(h => h.Key == "CustomHeader"));
        }
    }


    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get("error", _ =>
            {
                throw new Exception("ERROR MESSAGE HERE");
            });

            Get("forbidden", _ => HttpStatusCode.Forbidden);
            Get("unauthorised", _ => HttpStatusCode.Unauthorized);
            Get("headers", 
                _ =>
                    new Response().WithStatusCode(HttpStatusCode.InternalServerError)
                        .WithHeader("CustomHeader", "CustomHeaderValue"));
        }
    }
}
