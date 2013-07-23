namespace MicroLite.Extensions.Mvc.Tests
{
    using System.Linq;
    using System.Web.Mvc;
    using MicroLite.Configuration;
    using MicroLite.Extensions.Mvc.Filters;
    using Moq;
    using Xunit;

    public class ConfigurationExtensionsTests
    {
        public class WhenCallingWithWebApiAndThereAreNoFiltersRegistered
        {
            public WhenCallingWithWebApiAndThereAreNoFiltersRegistered()
            {
                GlobalFilters.Filters.Clear();

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithMvc(MvcConfigurationSettings.Default);
            }

            [Fact]
            public void AMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = GlobalFilters.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.NotNull(filter);
            }
        }

        public class WhenCallingWithWebApiWithConfigurationSettingsDisabled
        {
            public WhenCallingWithWebApiWithConfigurationSettingsDisabled()
            {
                GlobalFilters.Filters.Clear();

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithMvc(new MvcConfigurationSettings
                {
                    RegisterGlobalMicroLiteSessionAttribute = false
                });
            }

            [Fact]
            public void NoMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = GlobalFilters.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.Null(filter);
            }
        }

        public class WhenCallingWithWebApiWithDefaultSettingsButFiltersAreAlreadyRegistered
        {
            private readonly MicroLiteSessionAttribute microLiteSessionAttribute = new MicroLiteSessionAttribute();

            public WhenCallingWithWebApiWithDefaultSettingsButFiltersAreAlreadyRegistered()
            {
                GlobalFilters.Filters.Clear();
                GlobalFilters.Filters.Add(this.microLiteSessionAttribute);

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithMvc(MvcConfigurationSettings.Default);
            }

            [Fact]
            public void TheOriginalMicroLiteSessionAttributeShouldNotBeReplaced()
            {
                var filter = GlobalFilters.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.Same(microLiteSessionAttribute, filter.Instance);
            }
        }
    }
}