namespace MicroLite.Extensions.Mvc.Tests
{
    using MicroLite.Extensions.Mvc;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteReadOnlyController"/> class.
    /// </summary>
    public class MicroLiteReadOnlyControllerTests
    {
        public class WhenConstructedUsingTheDefaultConstructor
        {
            private readonly MicroLiteReadOnlyController controller;

            public WhenConstructedUsingTheDefaultConstructor()
            {
                var mockController = new Mock<MicroLiteReadOnlyController>();
                mockController.CallBase = true;

                this.controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsNull()
            {
                Assert.Null(this.controller.Session);
            }
        }

        public class WhenConstructedWithAnIReadOnlySession
        {
            private readonly MicroLiteReadOnlyController controller;
#if NET_4_0
            private readonly IReadOnlySession session = new Mock<IReadOnlySession>().Object;
#else
            private readonly IAsyncReadOnlySession session = new Mock<IAsyncReadOnlySession>().Object;
#endif

            public WhenConstructedWithAnIReadOnlySession()
            {
                var mockController = new Mock<MicroLiteReadOnlyController>(this.session);
                mockController.CallBase = true;

                this.controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsSet()
            {
                Assert.Equal(this.session, this.controller.Session);
            }
        }
    }
}