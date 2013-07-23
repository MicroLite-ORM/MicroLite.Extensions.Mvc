// -----------------------------------------------------------------------
// <copyright file="MvcConfigurationSettings.cs" company="MicroLite">
// Copyright 2012-2013 Trevor Pilley
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
    /// <summary>
    /// A class containing configuration options for the MicroLite Mvc extension.
    /// </summary>
    public sealed class MvcConfigurationSettings
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MvcConfigurationSettings"/> class.
        /// </summary>
        public MvcConfigurationSettings()
        {
            this.RegisterGlobalMicroLiteSessionAttribute = true;
        }

        /// <summary>
        /// Gets an instance of the settings with the default options set.
        /// </summary>
        public static MvcConfigurationSettings Default
        {
            get
            {
                return new MvcConfigurationSettings();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to register a MicroLiteSessionAttribute in the GlobalFilters.Filters
        /// is one is not already registered (defaults to true).
        /// </summary>
        public bool RegisterGlobalMicroLiteSessionAttribute
        {
            get;
            set;
        }
    }
}