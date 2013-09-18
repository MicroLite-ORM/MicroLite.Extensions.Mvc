// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using MicroLite.Extensions.Mvc;
    using MicroLite.Extensions.Mvc.Filters;
    using MicroLite.Logging;

    /// <summary>
    /// Extensions for the MicroLite configuration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Configures the MicroLite ORM Framework extensions for ASP.NET Mvc using the specified configuration settings.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <param name="settings">The settings used for configuration.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithMvc(this IConfigureExtensions configureExtensions, MvcConfigurationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            System.Diagnostics.Trace.TraceInformation(Messages.LoadingExtension);
            Log.TryLogInfo(Messages.LoadingExtension);
            MicroLiteSessionAttribute.SessionFactories = Configure.SessionFactories;

            if (settings.RegisterGlobalMicroLiteSessionAttribute
                && !GlobalFilters.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))))
            {
                Log.TryLogInfo(Messages.RegisteringDefaultActionFilter);
                GlobalFilters.Filters.Add(new MicroLiteSessionAttribute());
            }

            return configureExtensions;
        }
    }
}