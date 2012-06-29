MicroLite.Extensions.Mvc
========================

An extension project for MicroLite ORM to integrate with ASP.NET MVC 3

1. Install via NuGet `Install-Package MicroLite.Extensions.Mvc`
2. Load the extension in the application startup `Configure.Extensions().WithMvc();`
3. Make your controllers inherit from `MicroLiteController`
4. Add the `MicroLiteSessionAttribute` to methods which require an `ISession`

To find out more, head over to the wiki!