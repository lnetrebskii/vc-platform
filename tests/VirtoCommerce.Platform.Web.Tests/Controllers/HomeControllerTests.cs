using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Pose;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Web.Controllers;
using VirtoCommerce.Platform.Web.Infrastructure;
using VirtoCommerce.Platform.Web.Model.Home;
using Xunit;

namespace VirtoCommerce.Platform.Web.Tests
{
    [Trait("Category", "Unit")]
    public class HomeControllerTests
    {
        [Fact]
        public void Index_Test()
        {
            // Arrange
            var platformOptions = new Mock<IOptions<PlatformOptions>>();
            platformOptions.SetupAllProperties();
            var webAnalyticsOptions = new Mock<IOptions<WebAnalyticsOptions>>();
            webAnalyticsOptions.SetupAllProperties();
            Core.Common.PlatformVersion.CurrentVersion = SemanticVersion.Parse("3.0.0");
            var controller = new HomeController(platformOptions.Object, webAnalyticsOptions.Object, null, null);
            var indexModelShim = Shim.Replace(() => Is.A<IndexModel>().DemoResetTime)
                .With((IndexModel @this) => String.Empty);
            var platformVersionShim = Shim.Replace(() => PlatformVersion.CurrentVersion).With(() => SemanticVersion.Parse("3.0.0"));

            var dateTimeShim = Shim.Replace(() => DateTime.Now).With(() => new DateTime(2004, 4, 4));

            ActionResult result = null;

            // Act
            PoseContext.Isolate(() =>
            {
                result = controller.Index();
            }, dateTimeShim);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<IndexModel>>(
                viewResult.ViewData.Model);
            Assert.Equal("33", model.First().DemoResetTime);
        }
    }
}
