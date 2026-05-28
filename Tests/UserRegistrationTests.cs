using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NEXT.Pages.Dashboards;
using NEXT.Tests;
using NEXT.Tests.Fakes;
using Xunit;

namespace Tests
{
    public class UserRegistrationTests
    {
        [Fact]
        public void OnPostRegisterUser_WithoutSession_ReturnsRedirectToLogin()
        {
            // Arrange
            var fakeUserRepo = new FakeUserRepository();
            var fakeEventRegRepo = new FakeEventRegistrationRepository();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            var pageModel = new UserDashboardModel(fakeEventRegRepo, fakeUserRepo)
            {
                PageContext = new PageContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            IActionResult result = pageModel.OnPostRegister(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Equal("/Auth/Login", redirectResult.PageName);
        }

        [Fact]
        public void OnPostRegisterUser_WithSession_RegistersUserForEventAndRedirects()
        {
            // Arrange
            var fakeUserRepo = new FakeUserRepository();
            var fakeEventRegRepo = new FakeEventRegistrationRepository();
            var pageModel = new UserDashboardModel(fakeEventRegRepo, fakeUserRepo);

            var session = new TestSession();
            session.SetInt32("UserId", 42);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = session;

            pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            int eventId = 1;

            // Act
            IActionResult result = pageModel.OnPostRegister(eventId);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.True(fakeEventRegRepo.RegisterUserForEventCalled);
            Assert.Equal(42, fakeEventRegRepo.RegisteredUserId);
            Assert.Equal(1, fakeEventRegRepo.RegisteredEventId);
        }

        [Fact]
        public void OnPostUnregisterUser_WithoutSession_ReturnsRedirectToLogin()
        {
            // Arrange
            var fakeUserRepo = new FakeUserRepository();
            var fakeEventRegRepo = new FakeEventRegistrationRepository();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            var pageModel = new UserDashboardModel(fakeEventRegRepo, fakeUserRepo)
            {
                PageContext = new PageContext
                {
                    HttpContext = httpContext
                }
            };
            // Act
            IActionResult result = pageModel.OnPostUnregister(1);
            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Equal("/Auth/Login", redirectResult.PageName);
        }

        [Fact]
        public void OnPostUnregisterUser_WithSession_UnregistersUserFromEventAndRedirects()
        {
            // Arrange
            var fakeUserRepo = new FakeUserRepository();
            var fakeEventRegRepo = new FakeEventRegistrationRepository();
            var pageModel = new UserDashboardModel(fakeEventRegRepo, fakeUserRepo);
            var session = new TestSession();
            session.SetInt32("UserId", 42);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session;
            pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
            int eventId = 1;
            // Act
            IActionResult result = pageModel.OnPostUnregister(eventId);
            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.True(fakeEventRegRepo.UnregisterUserFromEventCalled);
            Assert.Equal(42, fakeEventRegRepo.UnregisteredUserId);
            Assert.Equal(1, fakeEventRegRepo.UnregisteredEventId);
        }
    }
}
