﻿using System;
using System.Data;
using System.Web.Mvc;
using Moq;
using Xunit;

namespace MicroLite.Extensions.Mvc.Tests
{
    /// <summary>
    /// Unit Tests for the <see cref="AutoManageTransactionAttribute"/> class.
    /// </summary>
    public class AutoManageTransactionAttributeTests
    {
        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndAnActiveTransaction
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndAnActiveTransaction()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteController controller = new Mock<MicroLiteController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndAutoManageTransactionIsFalse()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteController controller = new Mock<MicroLiteController>(_mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute
                {
                    AutoManageTransaction = false
                };
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndCommittingAnActiveTransactionThrowsAnException
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndCommittingAnActiveTransactionThrowsAnException()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteController controller = new Mock<MicroLiteController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndNoActiveTransaction
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndNoActiveTransaction()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(false);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteController controller = new Mock<MicroLiteController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndNoCurrentTransaction
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            [Fact]
            public void OnActionExecutedDoesNotThrowAnException()
            {
                MicroLiteController controller = new Mock<MicroLiteController>(_mockSession.Object).Object;

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
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(false);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteController controller = new Mock<MicroLiteController>(_mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller,
                    Exception = new Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBackAgain()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteController controller = new Mock<MicroLiteController>(_mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller,
                    Exception = new Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndAnActiveTransaction
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndAnActiveTransaction()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyController controller = new Mock<MicroLiteReadOnlyController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndAutoManageTransactionIsFalse()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyController controller = new Mock<MicroLiteReadOnlyController>(_mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute
                {
                    AutoManageTransaction = false
                };
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndCommittingAnActiveTransactionThrowsAnException
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndCommittingAnActiveTransactionThrowsAnException()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyController controller = new Mock<MicroLiteReadOnlyController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndNoActiveTransaction
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndNoActiveTransaction()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(false);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyController controller = new Mock<MicroLiteReadOnlyController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndNoCurrentTransaction
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();

            [Fact]
            public void OnActionExecutedDoesNotThrowAnException()
            {
                MicroLiteReadOnlyController controller = new Mock<MicroLiteReadOnlyController>(_mockSession.Object).Object;

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
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(false);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyController controller = new Mock<MicroLiteReadOnlyController>(_mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller,
                    Exception = new Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBackAgain()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyController controller = new Mock<MicroLiteReadOnlyController>(_mockSession.Object).Object;

                var context = new ActionExecutedContext
                {
                    Controller = controller,
                    Exception = new Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteController
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteController()
            {
                MicroLiteController controller = new Mock<MicroLiteController>(_mockSession.Object).Object;

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
                _mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteController_AndAutoManageTransactionIsFalse()
            {
                MicroLiteController controller = new Mock<MicroLiteController>(_mockSession.Object).Object;

                var context = new ActionExecutingContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute
                {
                    AutoManageTransaction = false
                };
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsNotStarted()
            {
                _mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyController
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyController()
            {
                MicroLiteReadOnlyController controller = new Mock<MicroLiteReadOnlyController>(_mockSession.Object).Object;

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
                _mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyController_AndAutoManageTransactionIsFalse()
            {
                MicroLiteReadOnlyController controller = new Mock<MicroLiteReadOnlyController>(_mockSession.Object).Object;

                var context = new ActionExecutingContext
                {
                    Controller = controller
                };

                var attribute = new AutoManageTransactionAttribute
                {
                    AutoManageTransaction = false
                };
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsNotStarted()
            {
                _mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Never());
            }
        }

        public class WhenConstructed
        {
            private readonly AutoManageTransactionAttribute _attribute = new AutoManageTransactionAttribute();

            [Fact]
            public void AutoManageTransactionIsTrue()
            {
                Assert.True(_attribute.AutoManageTransaction);
            }

            [Fact]
            public void IsolationLevelIsReadCommitted()
            {
                Assert.Equal(IsolationLevel.ReadCommitted, _attribute.IsolationLevel);
            }
        }
    }
}
