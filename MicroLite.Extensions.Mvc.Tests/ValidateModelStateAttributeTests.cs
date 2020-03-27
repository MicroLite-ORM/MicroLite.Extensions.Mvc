using System.Web.Mvc;
using Moq;
using Xunit;

namespace MicroLite.Extensions.Mvc.Tests
{
    public class ValidateModelStateAttributeTests
    {
        public class WhenCallingOnActionExecuting_AndTheModelStateDoesNotContainErrors
        {
            private readonly ValidateModelStateAttribute _attribute = new ValidateModelStateAttribute();

            [Fact]
            public void TheResultShoulNotBeSet()
            {
                ControllerBase controller = new Mock<ControllerBase>().Object;
                controller.ViewData = new ViewDataDictionary();

                var filterContext = new ActionExecutingContext
                {
                    Controller = controller
                };

                _attribute.OnActionExecuting(filterContext);

                Assert.Null(filterContext.Result);
            }
        }

        public class WhenCallingOnActionExecuting_TheModelStateContainsErrors_AndSkipValidationIsFalse
        {
            private readonly ValidateModelStateAttribute _attribute = new ValidateModelStateAttribute
            {
                SkipValidation = false
            };

            [Fact]
            public void TheResultShouldBeSetToAViewResult()
            {
                ControllerBase controller = new Mock<ControllerBase>().Object;
                controller.TempData = new TempDataDictionary();
                controller.ViewData = new ViewDataDictionary();
                controller.ViewData.ModelState.AddModelError("Foo", "Error");

                var filterContext = new ActionExecutingContext
                {
                    Controller = controller
                };

                _attribute.OnActionExecuting(filterContext);

                Assert.NotNull(filterContext.Result);
                Assert.IsType<ViewResult>(filterContext.Result);

                Assert.Equal(controller.TempData, ((ViewResult)filterContext.Result).TempData);
                Assert.Equal(controller.ViewData, ((ViewResult)filterContext.Result).ViewData);
            }
        }

        public class WhenCallingOnActionExecuting_TheModelStateContainsErrors_AndSkipValidationIsTrue
        {
            private readonly ValidateModelStateAttribute _attribute = new ValidateModelStateAttribute
            {
                SkipValidation = true
            };

            [Fact]
            public void TheResultShoulNotBeSet()
            {
                var controller = new Mock<ControllerBase>().Object;
                controller.TempData = new TempDataDictionary();
                controller.ViewData = new ViewDataDictionary();
                controller.ViewData.ModelState.AddModelError("Foo", "Error");

                var filterContext = new ActionExecutingContext
                {
                    Controller = controller
                };

                _attribute.OnActionExecuting(filterContext);

                Assert.Null(filterContext.Result);
            }
        }
    }
}
