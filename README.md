MicroLite.Extensions.Mvc
========================

An extension project for MicroLite ORM to integrate with ASP.NET MVC

1. Install via NuGet `Install-Package MicroLite.Extensions.Mvc`
2. Load the extension in the application startup `Configure.Extensions().WithMvc(MvcConfigurationSettings.Default);` prior to calling `Conflgure.Fluently()...`
3. Inherit your controllers from `MicroLiteController` or `MicroLiteReadOnlyController`

To find out more, head over to the wiki!
