// -----------------------------------------------------------------------
// <copyright file="ValidateModelStateAttribute.cs" company="Project Contributors">
// Copyright 2012 - 2017 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Extensions.Mvc
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// An <see cref="ActionFilterAttribute"/> which verifies the model state is valid.
    /// </summary>
    /// <example>
    /// Add to the global filters list for it to apply to every action on every controller unless opted out:
    /// <code>
    /// static void RegisterGlobalFilters(GlobalFilterCollection filters)
    /// {
    ///     filters.Add(new ValidateModelStateAttribute());
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// Add to a controller to opt out all actions in a controller:
    /// <code>
    /// [ValidateModelStateAttribute(SkipValidation = true)]
    /// public class CustomerController : MicroLiteController { ... }
    /// </code>
    /// </example>
    /// <example>
    /// Add to an individual action to opt out that particular action:
    /// <code>
    /// [ValidateModelStateAttribute(SkipValidation = true)]
    /// public ActionResult Edit(int id, Model model)
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ValidateModelStateAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether to skip validation (false by default).
        /// </summary>
        /// <remarks>
        /// Allows overriding the default behaviour on an individual action/controller if an instance
        /// is already registered in the global filters.
        /// </remarks>
        public bool SkipValidation
        {
            get;
            set;
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.SkipValidation)
            {
                return;
            }

            if (filterContext != null && !filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.Result = new ViewResult
                {
                    TempData = filterContext.Controller.TempData,
                    ViewData = filterContext.Controller.ViewData,
                };
            }
        }
    }
}