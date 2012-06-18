namespace MicroLite.Extensions.Mvc.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteSessionActionFilterAttribute"/> class.
    /// </summary>
    [TestFixture]
    public class MicroLiteSessionActionFilterAttributeTests
    {
        [Test]
        public void ConstructorSetsConnectionName()
        {
            var connectionName = "Northwind";

            var attribute = new MicroLiteSessionActionFilterAttribute(connectionName);

            Assert.AreEqual(connectionName, attribute.ConnectionName);
        }
    }
}