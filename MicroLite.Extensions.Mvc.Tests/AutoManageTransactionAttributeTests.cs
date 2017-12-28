﻿namespace MicroLite.Extensions.Mvc.Tests
{
    using System;
    using System.Data;
    using System.Web.Mvc;
    using MicroLite.Extensions.Mvc;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="AutoManageTransactionAttribute"/> class.
    /// </summary>
    public class AutoManageTransactionAttributeTests
    {
        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndAnActiveTransaction
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndAnActiveTransaction()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndAutoManageTransactionIsFalse()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.AutoManageTransaction = false;
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndCommittingAnActiveTransactionThrowsAnException
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndCommittingAnActiveTransactionThrowsAnException()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();

                Assert.Throws<InvalidOperationException>(() => attribute.OnActionExecuted(context));
            }

            [Fact]
            public void TheTransactionIsCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndNoActiveTransaction
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndNoActiveTransaction()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(false);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndNoCurrentTransaction
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            [Fact]
            public void OnActionExecutedDoesNotThrowAnException()
            {
                var controller = new Mock<MicroLiteController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();

                attribute.OnActionExecuted(context);
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(false);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller,
                    Exception = new System.Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBackAgain()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller,
                    Exception = new System.Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndAnActiveTransaction
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndAnActiveTransaction()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndAutoManageTransactionIsFalse()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.AutoManageTransaction = false;
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndCommittingAnActiveTransactionThrowsAnException
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndCommittingAnActiveTransactionThrowsAnException()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();

                Assert.Throws<InvalidOperationException>(() => attribute.OnActionExecuted(context));
            }

            [Fact]
            public void TheTransactionIsCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndNoActiveTransaction
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndNoActiveTransaction()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(false);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndNoCurrentTransaction
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();

            [Fact]
            public void OnActionExecutedDoesNotThrowAnException()
            {
                var controller = new Mock<MicroLiteReadOnlyController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();

                attribute.OnActionExecuted(context);
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(false);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller,
                    Exception = new System.Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBackAgain()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyController>(this.mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller,
                    Exception = new System.Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteController
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteController()
            {
                var controller = new Mock<MicroLiteController>(this.mockSession.Object).Object;

                var context = new ActionExecutingContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteController_AndAutoManageTransactionIsFalse()
            {
                var controller = new Mock<MicroLiteController>(this.mockSession.Object).Object;

                var context = new ActionExecutingContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.AutoManageTransaction = false;
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsNotStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyController
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyController()
            {
                var controller = new Mock<MicroLiteReadOnlyController>(this.mockSession.Object).Object;

                var context = new ActionExecutingContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyController_AndAutoManageTransactionIsFalse()
            {
                var controller = new Mock<MicroLiteReadOnlyController>(this.mockSession.Object).Object;

                var context = new ActionExecutingContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.AutoManageTransaction = false;
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsNotStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Never());
            }
        }

        public class WhenConstructed
        {
            private readonly AutoManageTransactionAttribute attribute = new AutoManageTransactionAttribute();

            [Fact]
            public void AutoManageTransactionIsTrue()
            {
                Assert.True(this.attribute.AutoManageTransaction);
            }

            [Fact]
            public void IsolationLevelIsReadCommitted()
            {
                Assert.Equal(IsolationLevel.ReadCommitted, this.attribute.IsolationLevel);
            }
        }
    }
}