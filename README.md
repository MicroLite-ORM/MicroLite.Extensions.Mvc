MicroLite.Extensions.Mvc
========================

_MicroLite.Extensions.Mvc_ is an extension to the MicroLite ORM Framework which allows integration with ASP.NET MVC 3 or later.

In order to use the extension, you first need to install it via NuGet:

    Install-Package MicroLite.Extensions.Mvc

You can then load the extension in your application start-up:

    // Load the extensions for MicroLite
    Configure
       .Extensions() // If you are also using a logging extension, that should be loaded first.
       .WithMvc();

    // Create the session factory...
    Configure
       .Fluently()
       ...

Then register the action filters in your `FilterConfig` class (usually in the App_Start folder):


    void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        filters.Add(new ErrorHandlerAttribute()); // this is part of ASP.NET and usually already added

        // Add the MicroLite filters in the following order (only add the ones you want based upon the descriptions of their behavior below)
        filters.Add(new ValidateModelNotNullAttribute());
        filters.Add(new ValidateModelStateAttribute());
        filters.Add(new MicroLiteSessionAttribute());
        filters.Add(new AutoManageTransactionAttribute());
    }

* The `ValidateModelNotNullAttribute` will throw an `ArgumentNullException` if the model in a request is null, it saves having to write if `(model == null) { throw new ArgumentNullException("model"); }` at the start of every action.
* The `ValidateModelStateAttribute` validates the model state in a request and redirects to the view if it is invalid, it saves having to wrap all your methods with `if (ModelState.IsValid())`.
* The `MicroLiteSessionAttribute` will ensure that a new `ISession\IReadOnlySession` is opened and assigned to the controller if it inherits from `MicroLiteController\MicroLiteReadOnlyController` or implements `IHaveSession\IHaveReadOnlySession`.
* The `AutoManageTransactionAttribute` will ensure that a transaction is started prior to the controller action being invoked and committed (or rolled back if the action throws an exception) after the controller action has been invoked.

To find out more, head over to the [Wiki](https://github.com/TrevorPilley/MicroLite.Extensions.Mvc/wiki).