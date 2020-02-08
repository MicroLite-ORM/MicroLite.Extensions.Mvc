using Moq;
using Xunit;

namespace MicroLite.Extensions.Mvc.Tests
{
    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteReadOnlyController"/> class.
    /// </summary>
    public class MicroLiteReadOnlyControllerTests
    {
        public class WhenConstructedWithAnIReadOnlySession
        {
            private readonly MicroLiteReadOnlyController _controller;
            private readonly IAsyncReadOnlySession _session = new Mock<IAsyncReadOnlySession>().Object;

            public WhenConstructedWithAnIReadOnlySession()
            {
                var mockController = new Mock<MicroLiteReadOnlyController>(_session)
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
