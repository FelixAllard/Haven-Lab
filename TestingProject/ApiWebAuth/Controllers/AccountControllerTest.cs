using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.Extensions.Configuration;
using ApiWebAuth.Controllers;
using ApiWebAuth.Models;

namespace TestingProject.ApiWebAuth.Controllers
{
    [TestFixture]
    public class AccountControllerTest
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<IConfiguration> _mockConfiguration;
        private AccountController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _mockConfiguration = new Mock<IConfiguration>();
            
            _controller = new AccountController(_mockUserManager.Object, _mockConfiguration.Object);
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller is IDisposable disposableController)
            {
                disposableController.Dispose();
            }
            
            _controller = null;
        }
        [Test]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerModel = new Register { Username = "newuser", Password = "Password123" };
            var identityResult = IdentityResult.Success;
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            
            var message = okResult.Value as string;
            Assert.IsNotNull(message);
            Assert.AreEqual("Registration successful", message);
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var registerModel = new Register { Username = "existinguser", Password = "Password123" };
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Username already exists" });
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "Password123" };
            var user = new IdentityUser { UserName = "testuser" };
            var roles = new List<string> { "Admin" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>())).ReturnsAsync(roles);
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("issuer");
            _mockConfiguration.Setup(x => x["Jwt:ExpiryMinutes"]).Returns("30");

            // There is a minimum key size
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("supersecretkey123!12312321312312312312312321");

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            
            var token = okResult.Value.ToString();
            Assert.IsNotNull(token);
        }


        [Test]
        public async Task Login_ReturnsUnauthorized_WhenLoginFails()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "WrongPassword" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        
        [Test]
        public void Register_ModelHasValidProperties()
        {
            // Arrange
            var registerModel = new Register
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123"
            };

            // Act & Assert
            Assert.AreEqual("newuser", registerModel.Username);
            Assert.AreEqual("newuser@example.com", registerModel.Email);
            Assert.AreEqual("Password123", registerModel.Password);
        }
    }
}
