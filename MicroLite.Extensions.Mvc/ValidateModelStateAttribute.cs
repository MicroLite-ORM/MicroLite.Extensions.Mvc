// -----------------------------------------------------------------------
// <copyright file="ValidateModelStateAttribute.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This method is only called the MVC framework & the ActionExecutingContext should never be null.")]
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.SkipValidation)
            {
                return;
            }

            var viewData = filterContext.Controller.ViewData;

            if (!viewData.ModelState.IsValid)
            {
                filterContext.Result = new ViewResult
                {
                    TempData = filterContext.Controller.TempData,
                    ViewData = viewData
                };
            }
        }
    }
}