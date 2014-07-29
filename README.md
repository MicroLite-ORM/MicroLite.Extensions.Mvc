MicroLite.Extensions.Mvc
========================

_MicroLite.Extensions.Mvc_ is an extension to the MicroLite ORM Framework which allows integration with ASP.NET MVC.

It is easy to use MicroLite with ASP.NET MVC, simply supply your controller with an `ISession` or `IReadOnlySession` and use it in your controller actions. However, using the MVC extension for MicroLite makes it even easier and contains some useful extras.

In order to use the MVC extension for the MicroLite ORM framework, you need to reference it in your solution. The easiest way to do this is install it via NuGet (if you are unfamiliar with NuGet, it's a package manager for Visual Studio - visit [nuget.org](http://www.nuget.org/) for more information).

    Install-Package MicroLite.Extensions.Mvc

## Configuring the Extension

Register the action filters for the behaviour you want in your `FilterConfig` in the App_Start folder of your project:

    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        filters.Add(new ErrorHandlerAttribute()); // this is part of ASP.NET and usually already added

        // Add the MicroLite filters in the following order (only add the ones you want based upon the descriptions of their behaviour below)
        filters.Add(new ValidateModelNotNullAttribute());
        filters.Add(new ValidateModelStateAttribute());
        filters.Add(new MicroLiteSessionAttribute("ConnectionName"));
        filters.Add(new AutoManageTransactionAttribute());
    }

Create a `MicroLiteConfig` class in your App_Start folder using the following template:

    public static class MicroLiteConfig
    {
        public static void ConfigureConnection()
        {
            // Load any MicroLite extensions
            Configure
                .Extensions() // if you are also using a logging extension, register it first
                .WithMvc();
        }

        public static void ConfigureExtensions()
        {
            // Create session factories for any connections
            Configure
                .Fluently()
                .For...Connection("ConnectionName")
                .CreateSessionFactory();
        }
    }

Call the `MicroLiteConfig` from the application start method in your Global.asax:

    protected void Application_Start()
    {
        // Register routes, bundles etc
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        ...

        // Always configure extensions before connections.
        MicroLiteConfig.ConfigureExtensions();
        MicroLiteConfig.ConfigureConnection();
    }

Lastly, implement `IHaveSession` or `IHaveReadOnlySession` - either directly or by inheriting from the `MicroLiteController` or `MicroLiteReadOnlyController`.

## What do the MicroLite Filters do?

### ValidateModelNotNullAttribute

The `ValidateModelNotNullAttribute` will throw an `ArgumentNullException` if the model in a request is null, it saves having to write `if (model == null) { throw new ArgumentNullException("model"); }` at the start of every action.

You may not want the attribute to apply to a certain action, therefore the `ValidateModelNotNullAttribute` has a `SkipValidation` property (false by default) which allows you to opt out individual actions.

    [ValidateModelNotNull(SkipValidation = true)] // will only affect the action it is applied to
    public ActionResult Edit(int id, Model model) { ... }

### ValidateModelStateAttribute

The `ValidateModelStateAttribute` validates the model state in a request and redirects to the view if it is invalid, it saves having to wrap all your methods with `if (ModelState.IsValid())`.

There are times when you may want to return a different model to the view than the one posted to the action so it is easy to opt out an action:

    [ValidateModelState(SkipValidation = true)] // will only affect the action it is applied to
    public ActionResult Edit(int id, Model model) { ... }

### MicroLiteSessionAttribute

The `MicroLiteSessionAttribute` will ensure that an `ISession` or `IReadOnlySession` is opened and assigned to your controller if it inherits from `MicroLiteController`/`MicroLiteReadOnlyController` or implements `IHaveSession`/`IHaveReadOnlySession` prior to an action being called. It will then ensure that the session is disposed of after the action has been called. When constructed, it requires the name of the connection the session should be opened for.

NOTE: If you use an IOC container to manage sessions, then you do not need to use the MicroLiteSessionAttribute.

### AutoManageTransactionAttribute

The `AutoManageTransactionAttribute` will ensure that a transaction is begun for the session provided to a controller if it inherits from `MicroLiteController`/`MicroLiteReadOnlyController` or implements `IHaveSession`/`IHaveReadOnlySession` prior to an action being called. It will then ensure that the transaction is either rolled back or committed depending on whether the action results in an exception being thrown or not.

There are times when you may want to manually manage the transaction so it is easy to opt out at either action or controller level:

    [AutoManageTransaction(AutoManageTransaction = false)] // will affect all actions in the controller
    public class CustomerController : MicroLiteController { ... }

    [AutoManageTransaction(AutoManageTransaction = false)] // will only affect the action it is applied to
    public ActionResult Edit(int id, Model model) { ... }

## Supported .NET Framework Versions

The NuGet Package contains binaries compiled against:

* .NET 4.0 (Full) and Microsoft.AspNet.Mvc 4.0.20710
* .NET 4.5 and Microsoft.AspNet.Mvc 5.0