// -----------------------------------------------------------------------
// <copyright file="MicroLiteSessionAttribute.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// An action filter attribute which can be applied to a class or method to supply a <see cref="MicroLiteController"/>
    /// with a new <see cref="ISession"/> when an action is executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class MicroLiteSessionAttribute : ActionFilterAttribute
    {
        private readonly string connectionName;

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteSessionAttribute"/> class.
        /// </summary>
        public MicroLiteSessionAttribute()
        {
            this.AutoManageTransaction = true;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteSessionAttribute"/> class.
        /// </summary>
        /// <param name="connectionName">Name of the connection to manage the session for.</param>
        public MicroLiteSessionAttribute(string connectionName)
            : this()
        {
            this.connectionName = connectionName;
        }

        /// <summary>
        /// Gets or sets the session factory.
        /// </summary>
        public static IEnumerable<ISessionFactory> SessionFactories
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to begin a transaction when OnActionExecuting is called
        /// and either commit or roll it back when OnActionExecuted is called depending on whether the ActionExecutedContext has an exception.
        /// </summary>
        public bool AutoManageTransaction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        public string ConnectionName
        {
            get
            {
                return this.connectionName;
            }
        }

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

            var controller = (MicroLiteController)filterContext.Controller;

            if (controller.Session != null)
            {
                if (controller.Session.Transaction != null)
                {
                    if (filterContext.Exception != null)
                    {
                        if (!controller.Session.Transaction.WasRolledBack)
                        {
                            controller.Session.Transaction.Rollback();
                        }
                    }
                    else
                    {
                        if (controller.Session.Transaction.IsActive)
                        {
                            controller.Session.Transaction.Commit();
                        }
                    }
                }

                controller.Session.Dispose();
            }
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SessionFactories == null)
            {
                throw new MicroLiteException(ExceptionMessages.NoSessionFactoriesSet);
            }

            if (this.connectionName == null && SessionFactories.Count() > 1)
            {
                throw new MicroLiteException(ExceptionMessages.NoConnectionNameMultipleSessionFactories);
            }

            var controller = filterContext.Controller as MicroLiteController;

            if (controller == null)
            {
                throw new NotSupportedException(ExceptionMessages.ControllerNotMicroLiteController);
            }

            if (this.AutoManageTransaction)
            {
                var sessionFactory =
                    SessionFactories.SingleOrDefault(x => this.connectionName == null || x.ConnectionName == this.connectionName);

                if (sessionFactory == null)
                {
                    throw new MicroLiteException(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.NoSessionFactoryFoundForConnectionName, this.connectionName));
                }

                controller.Session = sessionFactory.OpenSession();
                controller.Session.BeginTransaction();
            }
        }
    }
}