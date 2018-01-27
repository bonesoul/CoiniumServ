using System;
using System.IO;
using System.Linq;
using Nancy.ErrorHandling;
using Nancy.Responses;
using Nancy.Responses.Negotiation;
using Nancy.ViewEngines;

namespace Nancy.CustomErrors
{
	public class ErrorStatusCodeHandler : DefaultViewRenderer, IStatusCodeHandler
	{
		private readonly ISerializer _serializer;

		public ErrorStatusCodeHandler(IViewFactory viewFactory) : base(viewFactory)
		{
		}

		//Made this private as some DI containers like Autofac and Ninject have issues when multiple registered instances of an interface exist and
		// convention based DI is used for constructors
		private ErrorStatusCodeHandler(IViewFactory viewFactory, ISerializer serializer)
			: base(viewFactory)
		{
			_serializer = serializer;
		}

		public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
		{
			return statusCode == HttpStatusCode.NotFound
				   || statusCode == HttpStatusCode.InternalServerError
				   || statusCode == HttpStatusCode.Forbidden
				   || statusCode == HttpStatusCode.Unauthorized;
		}

		public void Handle(HttpStatusCode statusCode, NancyContext context)
		{			
			var headers = context.Response.Headers.Select(h => Tuple.Create(h.Key, h.Value)).ToArray();

			if (!ShouldRenderFriendlyErrorPage(context))
			{
				// Assume a valid error response was built earlier in the request lifecycle
				// Nothing more for us to do here, so just bail out.
				if (context.Response is ErrorResponse)
				{
					return;
				}

				var err = new Error
				{
					Message = CustomErrors.Configuration.ErrorSummary
				};

				if (context.Response is NotFoundResponse)
				{
					// Normally we return 404's ourselves so we have an ErrorResponse. 
					// But if no route is matched, Nancy will set a NotFound response itself. 
					// When this happens we still want to return our nice JSON response.
					err.Message = CustomErrors.Configuration.NotFoundSummary;
				} 
				else
				{
					switch (statusCode)
					{
						case HttpStatusCode.Forbidden :
						case HttpStatusCode.Unauthorized :
							err.Message = CustomErrors.Configuration.UnauthorizedSummary;
							break;
						case HttpStatusCode.NotFound :
							err.Message = CustomErrors.Configuration.NotFoundSummary;
							context.Response = new ErrorResponse(new Error
							{
								Message = CustomErrors.Configuration.NotFoundSummary
							}, _serializer, context.Environment);
							break;
					}
				}

				context.Response = new ErrorResponse(err, _serializer, context.Environment).WithHeaders(headers).WithStatusCode(statusCode);
				
				return;
			}
			
			var error = context.Response as ErrorResponse;

			var model = new ErrorViewModel
			{
				Details = error == null ? "" : error.FullException
			};
		
			switch (statusCode)
			{
				case HttpStatusCode.Forbidden:
					model.Title = CustomErrors.Configuration.ForbiddenTitle;
					model.Summary = CustomErrors.Configuration.ForbiddenSummary;

					break;

				case HttpStatusCode.Unauthorized:
					model.Title = CustomErrors.Configuration.UnauthorizedTitle;
					model.Summary = error == null ? CustomErrors.Configuration.UnauthorizedSummary : error.ErrorMessage;
										
					break;

				case HttpStatusCode.NotFound:
					model.Title = CustomErrors.Configuration.NotFoundTitle;
					model.Summary = CustomErrors.Configuration.NotFoundSummary;
					
					break;
				case HttpStatusCode.InternalServerError:
					model.Title = CustomErrors.Configuration.ErrorTitle;
					model.Summary = error == null ? CustomErrors.Configuration.ErrorSummary : error.ErrorMessage;
					
					break;
			}

			try
			{
				context.Response =
					RenderView(context, CustomErrors.Configuration.ErrorViews[statusCode], model)
						.WithStatusCode(statusCode)
						.WithHeaders(headers);
			}
			catch(Exception e)
			{
				context.Response = new Response
				{
					StatusCode = HttpStatusCode.InternalServerError,
					ContentType = "text/plain",
					Contents = stream =>
					{
						var writer = new StreamWriter(stream);
						writer.AutoFlush = true;
						writer.Write(string.Format("Could not locate your error view! Details: {0}", e.Message));
					}
				};
			}
		}

		private static bool ShouldRenderFriendlyErrorPage(NancyContext context)
		{
			if (CustomErrors.Configuration.AlwaysReturnJson)
			{
				return false;
			}

			if (context.Request.Headers.Accept.OrderByDescending(o => o.Item2)
				.Any(o => o.Item1 == "application/json" || o.Item1 == "text/json"))
			{
				return false;
			}

			return true;
		}
	}
}