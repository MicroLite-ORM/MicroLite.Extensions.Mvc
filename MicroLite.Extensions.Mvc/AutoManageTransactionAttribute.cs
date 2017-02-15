// -----------------------------------------------------------------------
// <copyright file="AutoManageTransactionAttribute.cs" company="MicroLite">
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
    using System.Data;
    using System.Web.Mvc;
    using MicroLite.Infrastructure;

    /// <summary>
    /// An action filter attribute which can be applied to a class or method to automatically begin an <see cref="ITransaction"/>
    /// before an action is executed and either commit or roll it back after the action is executed depending on whether an exception occurred.
    /// </summary>
    /// <example>
    /// Add to the global filters list for it to apply to every action on every controller unless opted out:
    /// <code>
    /// static void RegisterGlobalFilters(GlobalFilterCollection filters)
    /// {
    ///     filters.Add(new AutoManageTransactionAttribute());
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// Add to a controller to opt out all actions in a controller:
    /// <code>
    /// [AutoManageTransactionAttribute(AutoManageTransaction = false)]
    /// public class CustomerController : MicroLiteController { ... }
    /// </code>
    /// </example>
    /// <example>
    /// Add to an individual action to opt out that particular action:
    /// <code>
    /// [AutoManageTransactionAttribute(AutoManageTransaction = false)]
    /// public ActionResult Edit(int id, Model model)
    /// </code>
    /// </example>
    /// <example>
    /// Override the IsolationLevel of the transaction for a specific method (could also be done at controller level)
    /// <code>
    /// [AutoManageTransactionAttribute(IsolationLevel = IsolationLevel.Chaos)]
    /// public ActionResult Edit(int id, Model model)
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AutoManageTransactionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AutoManageTransactionAttribute"/> class using <see cref="IsolationLevel"/>.ReadCommitted.
        /// </summary>
        public AutoManageTransactionAttribute()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to begin a transaction when OnActionExecuting is called
        /// and either commit or roll it back when OnActionExecuted is called depending on whether the ActionExecutedContext has an exception.
        /// </summary>
        /// <remarks>
        /// Allows an individual controller or action to opt-out if an instance of the attribute is registered in the global filters collection.
        /// </remarks>
        public bool AutoManageTransaction { get; set; } = true;

        /// <summary>
        /// Gets or sets the isolation level to be used when a transaction is started.
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!this.AutoManageTransaction)
            {
                return;
            }

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var controller = filterContext.Controller as IHaveAsyncSession;

            if (controller != null)
            {
                OnActionExecuted(controller.Session, filterContext.Exception);
                return;
            }

            var readOnlyController = filterContext.Controller as IHaveAsyncReadOnlySession;

            if (readOnlyController != null)
            {
                OnActionExecuted(readOnlyController.Session, filterContext.Exception);
                return;
            }
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.AutoManageTransaction)
            {
                return;
            }

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var controller = filterContext.Controller as IHaveAsyncSession;

            if (controller != null)
            {
                controller.Session.BeginTransaction(this.IsolationLevel);
                return;
            }

            var readOnlyController = filterContext.Controller as IHaveAsyncReadOnlySession;

            if (readOnlyController != null)
            {
                readOnlyController.Session.BeginTransaction(this.IsolationLevel);
                return;
            }
        }

        private static void OnActionExecuted(IAsyncReadOnlySession session, Exception exception)
        {
            if (session.CurrentTransaction == null)
            {
                return;
            }

            var transaction = session.CurrentTransaction;

            try
            {
                if (transaction.IsActive && exception == null)
                {
                    transaction.Commit();
                }
                else if (transaction.IsActive && exception != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}