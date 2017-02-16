﻿// -----------------------------------------------------------------------
// <copyright file="MicroLiteReadOnlyController.cs" company="MicroLite">
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
    using System.Web;
    using System.Web.Mvc;
    using MicroLite.Infrastructure;

    /// <summary>
    /// Provides access to a MicroLite IReadOnlySession in addition to the base ASP.NET MVC controller.
    /// </summary>
    public abstract class MicroLiteReadOnlyController : Controller, IHaveAsyncReadOnlySession
    {
        /// <summary>
        /// Initialises a new instance of the MicroLiteReadOnlyController class with an IReadOnlySession.
        /// </summary>
        /// <param name="session">The IReadOnlySession for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an IReadOnlySession via an IOC container.
        /// </remarks>
        protected MicroLiteReadOnlyController(IAsyncReadOnlySession session)
        {
            this.Session = session;
        }

        /// <summary>
        /// Gets the System.Web.HttpSessionStateBase object for the current HTTP request.
        /// </summary>
        /// <remarks>This property replaces the Controller.Session property so that we can use it for our ISession.</remarks>
        public HttpSessionStateBase HttpSession => base.Session;

        /// <summary>
        /// Gets or sets the <see cref="IReadOnlySession"/> for the current HTTP request.
        /// </summary>
        public new IAsyncReadOnlySession Session
        {
            get;
            set;
        }
    }
}