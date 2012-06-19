namespace MicroLite.Extensions.Mvc.Tests
{
    using System;
    using System.Web.Mvc;
    using Moq;
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

        [Test]
        public void OnActionExecutedCommitsTransactionIfFilterContextHasNoException()
        {
            var mockTransaction = new Mock<ITransaction>();
            mockTransaction.Setup(x => x.Commit());

            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Transaction).Returns(mockTransaction.Object);

            var controller = new Mock<MicroLiteController>().Object;
            controller.Session = mockSession.Object;

            var context = new ActionExecutedContext
            {
                Controller = controller
            };

            var attribute = new MicroLiteSessionActionFilterAttribute();
            attribute.OnActionExecuted(context);

            mockTransaction.VerifyAll();
        }

        [Test]
        public void OnActionExecutedDisposesSession()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Dispose());

            var controller = new Mock<MicroLiteController>().Object;
            controller.Session = mockSession.Object;

            var context = new ActionExecutedContext
            {
                Controller = controller
            };

            var attribute = new MicroLiteSessionActionFilterAttribute();
            attribute.OnActionExecuted(context);

            mockSession.VerifyAll();
        }

        [Test]
        public void OnActionExecutedRollsBackTransactionIfFilterContextException()
        {
            var mockTransaction = new Mock<ITransaction>();
            mockTransaction.Setup(x => x.Rollback());

            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Transaction).Returns(mockTransaction.Object);

            var controller = new Mock<MicroLiteController>().Object;
            controller.Session = mockSession.Object;

            var context = new ActionExecutedContext
            {
                Controller = controller,
                Exception = new Exception()
            };

            var attribute = new MicroLiteSessionActionFilterAttribute();
            attribute.OnActionExecuted(context);

            mockTransaction.VerifyAll();
        }

        [Test]
        public void OnActionExecutingOpensSessionAndBeginsTransaction()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.BeginTransaction()).Returns(new Mock<ITransaction>().Object);

            var session = mockSession.Object;

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.ConnectionName).Returns("Northwind");
            mockSessionFactory.Setup(x => x.OpenSession()).Returns(session);

            MicroLiteSessionActionFilterAttribute.SessionFactories = new[]
            {
                mockSessionFactory.Object
            };

            var mockController = new Mock<MicroLiteController>();

            var controller = mockController.Object;

            var context = new ActionExecutingContext
            {
                Controller = controller
            };

            var attribute = new MicroLiteSessionActionFilterAttribute("Northwind");
            attribute.OnActionExecuting(context);

            mockSessionFactory.VerifyAll();

            Assert.AreSame(session, controller.Session);

            mockSession.VerifyAll();
        }

        [Test]
        public void OnActionExecutingThrowsMicroLiteExceptionIfNoConnectionNameAndMultipleSessionFactories()
        {
            MicroLiteSessionActionFilterAttribute.SessionFactories = new[]
            {
                new Mock<ISessionFactory>().Object,
                new Mock<ISessionFactory>().Object
            };

            var context = new ActionExecutingContext
            {
                Controller = new Mock<MicroLiteController>().Object
            };

            var attribute = new MicroLiteSessionActionFilterAttribute();

            var exception = Assert.Throws<MicroLiteException>(() => attribute.OnActionExecuting(context));
            Assert.AreEqual(ExceptionMessages.NoConnectionNameMultipleSessionFactories, exception.Message);
        }

        [Test]
        public void OnActionExecutingThrowsMicroLiteExceptionIfNoSessionFactories()
        {
            MicroLiteSessionActionFilterAttribute.SessionFactories = null;

            var context = new ActionExecutingContext
            {
                Controller = new Mock<MicroLiteController>().Object
            };

            var attribute = new MicroLiteSessionActionFilterAttribute();

            var exception = Assert.Throws<MicroLiteException>(() => attribute.OnActionExecuting(context));
            Assert.AreEqual(ExceptionMessages.NoSessionFactoriesSet, exception.Message);
        }

        [Test]
        public void OnActionExecutingThrowsMicroLiteExceptionIfNoSessionFactoryFoundForConnectionName()
        {
            MicroLiteSessionActionFilterAttribute.SessionFactories = new[]
            {
                new Mock<ISessionFactory>().Object,
                new Mock<ISessionFactory>().Object
            };

            var context = new ActionExecutingContext
            {
                Controller = new Mock<MicroLiteController>().Object
            };

            var attribute = new MicroLiteSessionActionFilterAttribute("Northwind");

            var exception = Assert.Throws<MicroLiteException>(() => attribute.OnActionExecuting(context));
            Assert.AreEqual(string.Format(ExceptionMessages.NoSessionFactoryFoundForConnectionName, "Northwind"), exception.Message);
        }

        [Test]
        public void OnActionExecutingThrowsNotSupportedExceptionIfControllerIsNotMicroLiteController()
        {
            MicroLiteSessionActionFilterAttribute.SessionFactories = new[]
            {
                new Mock<ISessionFactory>().Object
            };

            var context = new ActionExecutingContext
            {
                Controller = new Mock<Controller>().Object
            };

            var attribute = new MicroLiteSessionActionFilterAttribute();

            var exception = Assert.Throws<NotSupportedException>(() => attribute.OnActionExecuting(context));
            Assert.AreEqual(ExceptionMessages.ControllerNotMicroLiteController, exception.Message);
        }
    }
}