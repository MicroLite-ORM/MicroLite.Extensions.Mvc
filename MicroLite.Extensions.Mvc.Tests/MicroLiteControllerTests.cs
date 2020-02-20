using Moq;
using Xunit;

namespace MicroLite.Extensions.Mvc.Tests
{
    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteController"/> class.
    /// </summary>
    public class MicroLiteControllerTests
    {
        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteController _controller;
            private readonly ISession _session = new Mock<ISession>().Object;

            public WhenConstructedWithAnISession()
            {
                var mockController = new Mock<MicroLiteController>(_session)
                {
                    CallBase = true
                };

                _controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsSet()
            {
                Assert.Equal(_session, _controller.Session);
            }
        }
    }
}
