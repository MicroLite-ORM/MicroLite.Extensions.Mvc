namespace MicroLite.Extensions.Mvc.Tests
{
    using System.Linq;
    using System.Web.Mvc;
    using MicroLite.Configuration;
    using Moq;
    using Xunit;

    public class ConfigurationExtensionsTests
    {
        public class WhenCallingWithMvcAndRegisterGlobalFilterFalse
        {
            public WhenCallingWithMvcAndRegisterGlobalFilterFalse()
            {
                GlobalFilters.Filters.Clear();

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithMvc(registerGlobalFilter: false);
            }

            [Fact]
            public void NoMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = GlobalFilters.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.Null(filter);
            }
        }

        public class WhenCallingWithMvcAndThereIsAMicroLiteSessionAttributeRegistered
        {
            private readonly MicroLiteSessionAttribute attribute = new MicroLiteSessionAttribute();

            public WhenCallingWithMvcAndThereIsAMicroLiteSessionAttributeRegistered()
            {
                GlobalFilters.Filters.Clear();
                GlobalFilters.Filters.Add(this.attribute);

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithMvc(registerGlobalFilter: true);
            }

            [Fact]
            public void TheOriginalFilterShouldNotBeReplaced()
            {
                var filter = GlobalFilters.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.Same(attribute, filter.Instance);
            }
        }

        public class WhenCallingWithMvcAndThereIsNoMicroLiteSessionAttributeRegistered
        {
            public WhenCallingWithMvcAndThereIsNoMicroLiteSessionAttributeRegistered()
            {
                GlobalFilters.Filters.Clear();

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithMvc(registerGlobalFilter: true);
            }

            [Fact]
            public void AMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = GlobalFilters.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.NotNull(filter);
            }
        }
    }
}