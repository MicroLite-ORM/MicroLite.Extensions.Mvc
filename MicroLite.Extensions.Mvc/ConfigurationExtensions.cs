﻿// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
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
namespace MicroLite.Configuration
{
    using System.Linq;
    using System.Web.Mvc;
    using MicroLite.Extensions.Mvc;
    using MicroLite.Logging;

    /// <summary>
    /// Extensions for the MicroLite configuration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        private static ILog log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Configures the MicroLite ORM Framework extensions for ASP.NET MVC registering a MicroLiteSessionAttribute configured with default values in GlobalFilters.Filters if one has not already been registered.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithMvc(this IConfigureExtensions configureExtensions)
        {
            return WithMvc(configureExtensions, true);
        }

        /// <summary>
        /// Configures the MicroLite ORM Framework extensions for ASP.NET MVC optionally registering a MicroLiteSessionAttribute configured with default values in GlobalFilters.Filters if one has not already been registered.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <param name="registerGlobalFilter">If set to true and the MicroLiteSessionAttribute is not already registered in GlobalFilters.Filters, registers a new MicroLiteSessionAttribute with the default settings.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithMvc(this IConfigureExtensions configureExtensions, bool registerGlobalFilter)
        {
            System.Diagnostics.Trace.TraceInformation(Messages.LoadingExtension);
            log.TryLogInfo(Messages.LoadingExtension);
            MicroLiteSessionAttribute.SessionFactories = Configure.SessionFactories;

            if (registerGlobalFilter
                && !GlobalFilters.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))))
            {
                log.TryLogInfo(Messages.RegisteringDefaultActionFilter);
                GlobalFilters.Filters.Add(new MicroLiteSessionAttribute());
            }

            return configureExtensions;
        }
    }
}