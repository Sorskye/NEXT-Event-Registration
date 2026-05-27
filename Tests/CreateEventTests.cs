using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NEXT.Pages.Events;
using NEXT.Tests;
using NEXT.Tests.Fakes;
using Xunit;

namespace Tests
{
    public class CreateEventTests
    {
        [Fact]
        public void OnPostCreateEvent_WithoutSession_ReturnsRedirectToLogin()
        {
            // Arrange
            var fakeEventRepo = new FakeEventRepository();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            var pageModel = new CreateEventModel(fakeEventRepo)
            {
                PageContext = new PageContext
                {
                    HttpContext = httpContext
                },
                Title = "Test Event",
                Description = "Beschrijving",
                Date = new DateTime(2025, 6, 1),
                Location = "Amsterdam",
                MaxParticipants = 100,
                Cost = 10,
                LotteryPrize = 50
            };
            // Act
            IActionResult result = pageModel.OnPost();
            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Equal("/Auth/Login", redirectResult.PageName);
        }

        [Fact]
        public void OnPostCreateEvent_WithUserButNoAdmin_RedirectsToHomepage()
        {
            // Arrange
            var fakeEventRepo = new FakeEventRepository();

            var session = new TestSession();
            session.SetInt32("UserId", 1);
            session.SetString("UserRole", "User");
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session;

            var pageModel = new CreateEventModel(fakeEventRepo)
            {
                PageContext = new PageContext
                {
                    HttpContext = httpContext
                },
                Title = "Test Event",
                Description = "Beschrijving",
                Date = new DateTime(2025, 6, 1),
                Location = "Amsterdam",
                MaxParticipants = 100,
                Cost = 10,
                LotteryPrize = 50
            };

            // Act
            IActionResult result = pageModel.OnPost();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Equal("/Home/Homepage", redirectResult.PageName);
        }

        [Fact]
        public void OnPostCreateEvent_WithAdminUser_CreatesEventAndRedirects()
        {
            // Arrange
            var fakeEventRepo = new FakeEventRepository();
            var pageModel = new CreateEventModel(fakeEventRepo)
            {
                Title = "Test Event",
                Description = "Beschrijving",
                Date = new DateTime(2025, 6, 1),
                Location = "Amsterdam",
                MaxParticipants = 100,
                Cost = 10,
                LotteryPrize = 50
            };
            var session = new TestSession();
            session.SetInt32("UserId", 1);
            session.SetString("UserRole", "Admin");
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session;
            pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            // Act
            IActionResult result = pageModel.OnPost();
            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Equal("/Home/Homepage", redirectResult.PageName);
            Assert.True(fakeEventRepo.CreateEventCalled);
            Assert.NotNull(fakeEventRepo.CreatedEvent);
            Assert.Equal("Test Event", fakeEventRepo.CreatedEvent.Name);
            Assert.Equal(1, fakeEventRepo.CreatedByUserId);
        }
    }
}
