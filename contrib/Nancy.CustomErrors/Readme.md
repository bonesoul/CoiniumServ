# Custom error handling for Nancyfx applications

This work is *heavily* influenced by Paul Stovell's excellent [Consistent error handling with Nancy](http://paulstovell.com/blog/consistent-error-handling-with-nancy) article.

## Features

- Simple setup of custom views for error pages, 404 pages and authorization/authentication failure pages
- Supports any Nancy view engine
- Will send JSON serialized representation of errors if client has requested JSON
- Easily add support for custom error handling logic or for mapping application specific exception types to appropriate error responses

## Installation

Use Nuget!

```
Install-Package Nancy.CustomErrors
```

## Usage

### Bootstrapping

Custom error handling is set up in the ApplicationStartup method of your Nancy Bootstrapper. In its simplest form, your simply call Nancy.CustomErrors.Enable, passing an IPipelines instance.

```csharp
protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
{
	base.ApplicationStartup(container, pipelines);

	// ...
	CustomErrors.Enable(pipelines);
}
```

In this form, default configuration options will be assumed. The default configuration assumes the following

- Any and all unhandled exceptions will generate a response with a 500 HTTP status code
- Any request with an Accept header containing a recognised json type will be served with a JSON representation of the error, rather than a rendered view

*Note that if you are using the Nancy.Elmah package, ElmahLogging.Enable() should be called before CustomErrors.Enable()* 

### Configuration

Setting up custom configuration is a simple case of implementing an configuration class that extends Nancy.CustomErrors.CustomErrorsConfiguration, and passing an instance of the class along in the call to CustomErrors.Enable()

```csharp
public class MyErrorConfiguration : Nancy.CustomErrors.CustomErrorsConfiguration
{
	public MyErrorConfiguration() : base()
	{
		// Map error status codes to custom view names
		ErrorViews[HttpStatusCode.NotFound] = "CustomNotFoundView";
		ErrorViews[HttpStatusCode.InternalServerError] = "CustomErrorView";
		ErrorViews[HttpStatusCode.Forbidden] = "Forbidden";			
	}

	// Custom redirection handler for an unauthorised request
	// Returning an empty string will result in the response being sent with HttpStatusCode.Forbidden
	// Otherwise, return the url to redirect the client to. 
	public override string GetAuthorizationUrl(NancyContext context) {
		if (context.CurrentUser == null) {
			return "/accounts/login"
		}			
		return String.Empty;
	}

	// Custom mapping of a thrown exception to an ErrorResponse with status code
	// The implementation in this example is the default implementation used in
	// Nancy.CustomErrors.CustomErrorConfiguration. Override this if you need to
	// Map custom exception types to different status codes, or error objects.
	// An example might be to map a custom security exception to HttpForbidden status
	// code, rather than the default InternalServerError status code
	public override ErrorResponse HandleError(NancyContext context, Exception ex, ISerializer serializer)
	{
		var error = new Error
		{
			FullException = ex.ToString(),
			Message = ex.Message
		};

		return new ErrorResponse(error, serializer).WithStatusCode(HttpStatusCode.InternalServerError) as ErrorResponse;
	}
}
```

Once you've implemented a custom error handling configuration, just pass it along in the CustomErrors.Enable call in your bootstrapper

```csharp
protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
{
	base.ApplicationStartup(container, pipelines);

	// ...
	CustomErrors.Enable(pipelines, new MyErrorConfiguration());
}
```

### Error views

Your error views will be rendered with a simple object model, containing a Title, Summary and optional Details field. All fields are c# string type.

We use whatever view engine is presently configured in your project. By default you will need to create a single view resolved by the name "Error" to handle all error types. If you would like to use different views for different status codes, just set up a custom configuration as shown above.

Here is a simple example using the Razor view engine
	
```html
@inherits Nancy.ViewEngines.Razor.NancyRazorViewBase<dynamic>

<h1>Ouch!</h1>
<h2>@Model.Title</h2>
<p>@Model.Summary</p>
<p>@Model.Details</p>
```
