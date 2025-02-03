using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.Extensions.Configuration;
using ApiWebAuth.Controllers;
using ApiWebAuth.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using NUnit.Framework;

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
            var store = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            _mockConfiguration = new Mock<IConfiguration>();

            // Use a valid base64-encoded key of at least 32 bytes
            var validKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("this_is_a_valid_key_with_at_least_32_bytes_1234567890"));
            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns(validKey);
            _mockConfiguration.Setup(c => c["Jwt:ExpiryMinutes"]).Returns("30");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");

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
        public async Task Register_ValidUser_ReturnsOk()
        {
            // Arrange
            var model = new Register { Username = "testuser", Password = "Password123!" };
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Registration successful", okResult.Value);
        }

        [Test]
        public async Task Register_InvalidUser_ReturnsBadRequest()
        {
            // Arrange
            var model = new Register { Username = "testuser", Password = "Password123!" };
            var errors = new[] { new IdentityError { Description = "Error" } };
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult.Value);
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var model = new Login { Username = "testuser", Password = "Password123!" };
            var user = new IdentityUser { UserName = model.Username };
            _mockUserManager.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, model.Password)).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var model = new Login { Username = "testuser", Password = "WrongPassword" };
            _mockUserManager.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Logout_ValidUser_ReturnsOk()
        {
            // Arrange
            var username = "testuser";
            var user = new IdentityUser { UserName = username };
            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.RemoveAuthenticationTokenAsync(user, "JWT", "AccessToken"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Logout(username);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("User logged out successfully.", okResult.Value);
        }

        [Test]
        public async Task Logout_InvalidUser_ReturnsNotFound()
        {
            // Arrange
            var username = "nonexistentuser";
            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _controller.Logout(username);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User not found.", notFoundResult.Value);
        }

        
        [Test]
        public async Task VerifyToken_ValidToken_ReturnsOk()
        {
            // Arrange
            var token = GenerateJwtToken("testuser");
            var user = new IdentityUser { UserName = "testuser" };
            _mockUserManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetAuthenticationTokenAsync(user, "JWT", "AccessToken")).ReturnsAsync(token);

            // Act
            var result = await _controller.VerifyToken(token);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
    
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
        }

        
        
        [Test]
        public async Task VerifyToken_InvalidToken_BadRequestObjectResult()
        {
            // Arrange
            var token = "invalid.token.here";
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _controller.VerifyToken(token);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        
        [Test]
        public async Task VerifyToken_InvalidSignature_ReturnsBadRequest()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("different_invalid_key_12345678901234567890");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testuser") }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _mockConfiguration.Object["Jwt:Issuer"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var invalidToken = tokenHandler.WriteToken(token);
    
            // Act
            var result = await _controller.VerifyToken(invalidToken);
    
            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        
        [Test]
        public async Task VerifyToken_MissingClaims_ReturnsUnauthorized()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_mockConfiguration.Object["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(), // No Name or Sub claim
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _mockConfiguration.Object["Jwt:Issuer"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var missingClaimsToken = tokenHandler.WriteToken(token);
    
            // Act
            var result = await _controller.VerifyToken(missingClaimsToken);
    
            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.AreEqual("Invalid token claims.", unauthorizedResult.Value);
        }
        
        [Test]
        public async Task VerifyToken_TokenMismatch_ReturnsUnauthorized()
        {
            // Arrange
            var token = GenerateJwtToken("testuser");
            var user = new IdentityUser { UserName = "testuser" };
            _mockUserManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetAuthenticationTokenAsync(user, "JWT", "AccessToken")).ReturnsAsync("different_token");
    
            // Act
            var result = await _controller.VerifyToken(token);
    
            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.AreEqual("Token mismatch.", unauthorizedResult.Value);
        }

        
        private string GenerateJwtToken(string username, DateTime? expires = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_mockConfiguration.Object["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = expires ?? DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _mockConfiguration.Object["Jwt:Issuer"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}