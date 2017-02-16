namespace MicroLite.Extensions.Mvc.Tests
{
    using System;
    using MicroLite.Configuration;
    using Xunit;

    public class MvcConfigurationExtensionsTests
    {
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