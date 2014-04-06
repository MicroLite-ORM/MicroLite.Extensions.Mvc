﻿// -----------------------------------------------------------------------
// <copyright file="AutoManageTransactionAttribute.cs" company="MicroLite">
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
    using System.Data;
    using System.Web.Mvc;
    using MicroLite.Infrastructure;

    /// <summary>
    /// An action filter attribute which can be applied to a class or method to automatically begin an <see cref="ITransaction"/>
    /// before an action is executed and either commit or roll it back after the action is executed depending on whether an exception occurred.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AutoManageTransactionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AutoManageTransactionAttribute"/> class using <see cref="IsolationLevel"/>.ReadCommitted.
        /// </summary>
        public AutoManageTransactionAttribute()
        {
            this.AutoManageTransaction = true;
            this.IsolationLevel = IsolationLevel.ReadCommitted;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to begin a transaction when OnActionExecuting is called
        /// and either commit or roll it back when OnActionExecuted is called depending on whether the ActionExecutedContext has an exception.
        /// </summary>
        /// <remarks>
        /// Allows an individual controller or action to opt-out if an instance of the attribute is registered in the global filters collection.
        /// </remarks>
        public bool AutoManageTransaction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the isolation level to be used when a transaction is started.
        /// </summary>
        public IsolationLevel IsolationLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This method is only called the MVC framework & the ActionExecutingContext should never be null.")]
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!this.AutoManageTransaction)
            {
                return;
            }

            var controller = filterContext.Controller as IHaveSession;

            if (controller != null)
            {
                OnActionExecuted(controller.Session, filterContext.Exception);
                return;
            }

            var readOnlyController = filterContext.Controller as IHaveReadOnlySession;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This method is only called the MVC framework & the ActionExecutingContext should never be null.")]
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.AutoManageTransaction)
            {
                return;
            }

            var controller = filterContext.Controller as IHaveSession;

            if (controller != null)
            {
                controller.Session.BeginTransaction(this.IsolationLevel);
                return;
            }

            var readOnlyController = filterContext.Controller as IHaveReadOnlySession;

            if (readOnlyController != null)
            {
                readOnlyController.Session.BeginTransaction(this.IsolationLevel);
                return;
            }
        }

        private static void OnActionExecuted(IReadOnlySession session, Exception exception)
        {
            if (session.CurrentTransaction == null)
            {
                return;
            }

            if (exception == null && session.CurrentTransaction.IsActive)
            {
                session.CurrentTransaction.Commit();
                return;
            }

            if (exception != null && !session.CurrentTransaction.WasRolledBack)
            {
                session.CurrentTransaction.Rollback();
                return;
            }
        }
    }
}