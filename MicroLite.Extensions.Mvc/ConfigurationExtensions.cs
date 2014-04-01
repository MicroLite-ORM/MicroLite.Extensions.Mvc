// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
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
        /// <param name="filterCollection">The GlobalFilterCollection (GlobalFilters.Filters if using WebHost).</param>
        /// <param name="settings">The settings used for configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        /// <returns>The configure extensions.</returns>
        /// <example>
        /// If hosted in IIS and using the default settings:
        /// <code>
        /// Configure
        ///     .Extensions()
        ///     .WithMvc(GlobalFilters.Filters, MvcConfigurationSettings.Default);
        /// </code>
        /// </example>
        /// <example>
        /// If hosted in IIS and using custom settings:
        /// <code>
        /// Configure
        ///     .Extensions()
        ///     .WithMvc(
        ///         GlobalFilters.Filters,
        ///         new MvcConfigurationSettings { ... });
        /// </code>
        /// </example>
        public static IConfigureExtensions WithMvc(
            this IConfigureExtensions configureExtensions,
            GlobalFilterCollection filterCollection,
            MvcConfigurationSettings settings)
        {
            if (configureExtensions == null)
            {
                throw new ArgumentNullException("configureExtensions");
            }

            if (filterCollection == null)
            {
                throw new ArgumentNullException("filterCollection");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            System.Diagnostics.Trace.TraceInformation(Messages.LoadingExtension);
            if (Log.IsInfo)
            {
                Log.Info(Messages.LoadingExtension);
            }

            MicroLiteSessionAttribute.SessionFactories = Configure.SessionFactories;

            if (settings.RegisterGlobalValidateModelStateAttribute
                && !filterCollection.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute))))
            {
                if (Log.IsInfo)
                {
                    Log.Info(Messages.RegisteringValidateModelStateAttribute);
                }

                filterCollection.Add(new ValidateModelStateAttribute());
            }

            if (settings.RegisterGlobalMicroLiteSessionAttribute
                && !filterCollection.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))))
            {
                if (Log.IsInfo)
                {
                    Log.Info(Messages.RegisteringDefaultMicroLiteSessionActionFilter);
                }

                filterCollection.Add(new MicroLiteSessionAttribute());
            }

            return configureExtensions;
        }
    }
}