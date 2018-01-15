// -----------------------------------------------------------------------
// <copyright file="MicroLiteController.cs" company="Project Contributors">
// Copyright 2012 - 2018 Project Contributors
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
    using System.Web;
    using System.Web.Mvc;
    using MicroLite.Infrastructure;

    /// <summary>
    /// Provides access to a MicroLite ISession in addition to the base ASP.NET MVC controller.
    /// </summary>
    public abstract class MicroLiteController : Controller, IHaveAsyncSession
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteController"/> class.
        /// </summary>
        /// <param name="session">The <see cref="IAsyncSession"/> for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an <see cref="IAsyncSession"/> via an IOC container.
        /// </remarks>
        protected MicroLiteController(IAsyncSession session)
        {
            this.Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        /// <summary>
        /// Gets the System.Web.HttpSessionStateBase object for the current HTTP request.
        /// </summary>
        /// <remarks>This property replaces the Controller.Session property so that we can use it for our ISession.</remarks>
        public HttpSessionStateBase HttpSession => base.Session;

        /// <summary>
        /// Gets or sets the <see cref="IAsyncSession"/> for the current HTTP request.
        /// </summary>
        public new IAsyncSession Session
        {
            get;
            set;
        }
    }
}