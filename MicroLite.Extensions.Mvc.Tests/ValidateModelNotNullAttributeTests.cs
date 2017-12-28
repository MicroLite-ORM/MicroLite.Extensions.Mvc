namespace MicroLite.Extensions.Mvc.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using MicroLite.Extensions.Mvc;
    using Xunit;

    public class ValidateModelNotNullAttributeTests
    {
        public class WhenCallingOnActionExecuting_AndTheActionArgumentsDoesNotContainNull
        {
            private readonly ValidateModelNotNullAttribute attribute = new ValidateModelNotNullAttribute();

            [Fact]
            public void NoExceptionShouldBeThrown()
            {
                var actionContext = new ActionExecutingContext
                {
                    ActionParameters = new Dictionary<string, object>
                    {
                        { "model", new object() }
                    }
                };

                attribute.OnActionExecuting(actionContext);
            }
        }

        public class WhenCallingOnActionExecuting_TheActionArgumentsContainNull_AndSkipValidationIsFalse
        {
            private readonly ValidateModelNotNullAttribute attribute = new ValidateModelNotNullAttribute
            {
                SkipValidation = false
            };

            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var actionContext = new ActionExecutingContext
                {
                    ActionParameters = new Dictionary<string, object>
                    {
                        { "model", null }
                    }
                };

                var exception = Assert.Throws<ArgumentNullException>(() => attribute.OnActionExecuting(actionContext));

                Assert.Equal("model", exception.ParamName);
            }
        }

        public class WhenCallingOnActionExecuting_TheActionArgumentsContainNull_AndSkipValidationIsTrue
        {
            private readonly ValidateModelNotNullAttribute attribute = new ValidateModelNotNullAttribute
            {
                SkipValidation = true
            };

            [Fact]
            public void NoExceptionShouldBeThrown()
            {
                var actionContext = new ActionExecutingContext
                {
                    ActionParameters = new Dictionary<string, object>
                    {
                        { "model", null }
                    }
                };

                attribute.OnActionExecuting(actionContext);
            }
        }
    }
}