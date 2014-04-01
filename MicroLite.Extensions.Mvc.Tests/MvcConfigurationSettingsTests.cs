namespace MicroLite.Extensions.Mvc.Tests
{
    using MicroLite.Configuration;
    using Xunit;

    public class MvcConfigurationSettingsTests
    {
        [Fact]
        public void PropertiesAreSetToTrueByDefault()
        {
            var settings = new MvcConfigurationSettings();

            Assert.True(settings.RegisterGlobalMicroLiteSessionAttribute);
            Assert.True(settings.RegisterGlobalValidateModelStateAttribute);
        }
    }
}