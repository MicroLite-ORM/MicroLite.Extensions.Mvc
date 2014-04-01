namespace MicroLite.Extensions.Mvc.Tests
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using MicroLite.Configuration;
    using MicroLite.Extensions.Mvc.Filters;
    using Moq;
    using Xunit;

    public class ConfigurationExtensionsTests
    {
        public class WhenCallingWithMvc_AndConfigureExtensionsIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var configureExtensions = default(IConfigureExtensions);

                var exception = Assert.Throws<ArgumentNullException>(
                    () => configureExtensions.WithMvc(new GlobalFilterCollection(), new MvcConfigurationSettings()));

                Assert.Equal("configureExtensions", exception.ParamName);
            }
        }

        public class WhenCallingWithMvc_AndGlobalFilterCollectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                var exception = Assert.Throws<ArgumentNullException>(
                    () => configureExtensions.WithMvc(null, new MvcConfigurationSettings()));

                Assert.Equal("filterCollection", exception.ParamName);
            }
        }

        public class WhenCallingWithMvc_AndMvcConfigurationSettingsIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                var exception = Assert.Throws<ArgumentNullException>(
                    () => configureExtensions.WithMvc(new GlobalFilterCollection(), null));

                Assert.Equal("settings", exception.ParamName);
            }
        }

        public class WhenCallingWithMvcAndThereAreNoFiltersRegistered
        {
            private readonly GlobalFilterCollection filterCollection = new GlobalFilterCollection();

            public WhenCallingWithMvcAndThereAreNoFiltersRegistered()
            {
                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithMvc(this.filterCollection, MvcConfigurationSettings.Default);
            }

            [Fact]
            public void AMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = this.filterCollection
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute)))
                    .SingleOrDefault();

                Assert.NotNull(filter);
            }

            [Fact]
            public void AValidateModelStateAttributeShouldBeRegistered()
            {
                var filter = this.filterCollection
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute)))
                    .SingleOrDefault();

                Assert.NotNull(filter);
            }

            [Fact]
            public void TheMicroLiteSessionAttributeShouldBeSecond()
            {
                var filters = this.filterCollection.Select(f => f.Instance).ToArray();

                Assert.IsType<MicroLiteSessionAttribute>(filters[1]);
            }

            [Fact]
            public void TheValidateModelStateAttributeShouldBeFirst()
            {
                var filters = this.filterCollection.Select(f => f.Instance).ToArray();

                Assert.IsType<ValidateModelStateAttribute>(filters[0]);
            }
        }

        public class WhenCallingWithWithMvcWithConfigurationSettingsDisabled
        {
            private readonly GlobalFilterCollection filterCollection = new GlobalFilterCollection();

            public WhenCallingWithWithMvcWithConfigurationSettingsDisabled()
            {
                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithMvc(this.filterCollection, new MvcConfigurationSettings
                {
                    RegisterGlobalMicroLiteSessionAttribute = false,
                    RegisterGlobalValidateModelStateAttribute = false
                });
            }

            [Fact]
            public void NoMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = this.filterCollection
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute)))
                    .SingleOrDefault();

                Assert.Null(filter);
            }

            [Fact]
            public void NoValidateModelStateAttributeShouldBeRegistered()
            {
                var filter = this.filterCollection
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute)))
                    .SingleOrDefault();

                Assert.Null(filter);
            }
        }

        public class WhenCallingWithWithMvcWithDefaultSettingsButFiltersAreAlreadyRegistered
        {
            private readonly GlobalFilterCollection filterCollection = new GlobalFilterCollection();
            private readonly MicroLiteSessionAttribute microLiteSessionAttribute = new MicroLiteSessionAttribute();
            private readonly ValidateModelStateAttribute validateModelStateAttribute = new ValidateModelStateAttribute();

            public WhenCallingWithWithMvcWithDefaultSettingsButFiltersAreAlreadyRegistered()
            {
                this.filterCollection.Add(this.microLiteSessionAttribute);
                this.filterCollection.Add(this.validateModelStateAttribute);

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithMvc(this.filterCollection, MvcConfigurationSettings.Default);
            }

            [Fact]
            public void TheOriginalMicroLiteSessionAttributeShouldNotBeReplaced()
            {
                var filter = this.filterCollection
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute)))
                    .SingleOrDefault();

                Assert.Same(microLiteSessionAttribute, filter.Instance);
            }

            [Fact]
            public void TheOriginalValidateModelStateAttributeShouldNotBeReplaced()
            {
                var filter = this.filterCollection
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute)))
                    .SingleOrDefault();

                Assert.Same(validateModelStateAttribute, filter.Instance);
            }
        }
    }
}