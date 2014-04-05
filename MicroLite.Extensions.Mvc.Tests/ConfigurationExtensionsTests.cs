namespace MicroLite.Extensions.Mvc.Tests
{
    using System;
    using MicroLite.Configuration;
    using Moq;
    using Xunit;

    public class ConfigurationExtensionsTests
    {
        public class WhenCallingWithMvc : IDisposable
        {
            public WhenCallingWithMvc()
            {
                MicroLiteSessionAttribute.SessionFactories = null;

                var configureExtensions = Mock.Of<IConfigureExtensions>();

                configureExtensions.WithMvc();
            }

            public void Dispose()
            {
                MicroLiteSessionAttribute.SessionFactories = null;
            }

            [Fact]
            public void TheSessionFactoriesShouldBeSetOnTheMicroLiteSessionAttribute()
            {
                Assert.NotNull(MicroLiteSessionAttribute.SessionFactories);
            }
        }

        public class WhenCallingWithMvc_AndConfigureExtensionsIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var configureExtensions = default(IConfigureExtensions);

                var exception = Assert.Throws<ArgumentNullException>(
                    () => configureExtensions.WithMvc());

                Assert.Equal("configureExtensions", exception.ParamName);
            }
        }
    }
}