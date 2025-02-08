using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Api_Gateway.Annotations;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace TestingProject.Api_Gateway.Attributes
{
    [TestFixture]
    public class RequireAuthAttributeTest
    {
        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";

        private Mock<ServiceAuthController> _mockServiceAuthController;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private RequireAuthAttribute _requireAuthAttribute;

        [SetUp]
        public void SetUp()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockServiceAuthController = new Mock<ServiceAuthController>(_mockHttpClientFactory.Object);
            _requireAuthAttribute = new RequireAuthAttribute();
        }

        [Test]
        public async Task OnAuthorizationAsync_ValidToken_AuthorizesSuccessfully()
        {
            // Arrange
            var token = "validtoken";
            var authHeader = $"{BEARER_PREFIX}{token}";

            // Mock the IServiceProvider to return the ServiceAuthController mock
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceAuthController)))
                .Returns(_mockServiceAuthController.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };
            httpContext.Request.Headers[AUTH_HEADER_NAME] = authHeader;

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            // Set up the ServiceAuthController mock method to return a successful result
            _mockServiceAuthController.Setup(s => s.VerifyTokenAsync(token))
                .ReturnsAsync(new ObjectResult(null) { StatusCode = 200 });

            // Act
            await _requireAuthAttribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsNull(context.Result); // No result indicates successful authorization
        }

        [Test]
        public async Task OnAuthorizationAsync_MissingAuthHeader_ReturnsUnauthorized()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            // Mock IServiceProvider to return the ServiceAuthController
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceAuthController)))
                .Returns(_mockServiceAuthController.Object);

            httpContext.RequestServices = serviceProviderMock.Object;

            // Act
            await _requireAuthAttribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(context.Result);
            var result = (UnauthorizedObjectResult)context.Result;
            var serializer = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var expectedJson = JsonSerializer.Serialize(new { Message = "Missing Authorization Header" }, serializer);
            var actualJson = JsonSerializer.Serialize(result.Value, serializer);
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task OnAuthorizationAsync_InvalidAuthHeaderFormat_ReturnsUnauthorized()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[AUTH_HEADER_NAME] = "Token invalidtoken";

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            // Mock IServiceProvider to return the ServiceAuthController
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceAuthController)))
                .Returns(_mockServiceAuthController.Object);

            httpContext.RequestServices = serviceProviderMock.Object;

            // Act
            await _requireAuthAttribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(context.Result);
            var result = (UnauthorizedObjectResult)context.Result;
            var serializer = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var expectedJson = JsonSerializer.Serialize(new { Message = "Invalid Authorization format. Expected 'Bearer <token>'" }, serializer);
            var actualJson = JsonSerializer.Serialize(result.Value, serializer);
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task OnAuthorizationAsync_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var token = "invalidtoken";
            var authHeader = $"{BEARER_PREFIX}{token}";

            // Mock the IServiceProvider to return the ServiceAuthController mock
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceAuthController)))
                .Returns(_mockServiceAuthController.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };
            httpContext.Request.Headers[AUTH_HEADER_NAME] = authHeader;

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            _mockServiceAuthController.Setup(s => s.VerifyTokenAsync(token))
                .ReturnsAsync(new ObjectResult(null) { StatusCode = 401 });

            // Act
            await _requireAuthAttribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(context.Result);
            var result = (UnauthorizedObjectResult)context.Result;
            var serializer = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var expectedJson = JsonSerializer.Serialize(new { Message = "Unauthorized" }, serializer);
            var actualJson = JsonSerializer.Serialize(result.Value, serializer);
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task OnAuthorizationAsync_ServiceUnavailable_Returns503()
        {
            // Arrange
            var token = "validtoken";
            var authHeader = $"{BEARER_PREFIX}{token}";

            // Mock the IServiceProvider to return the ServiceAuthController mock
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceAuthController)))
                .Returns(_mockServiceAuthController.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };
            httpContext.Request.Headers[AUTH_HEADER_NAME] = authHeader;

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            _mockServiceAuthController.Setup(s => s.VerifyTokenAsync(token))
                .ReturnsAsync(new ObjectResult(null) { StatusCode = 503 });

            // Act
            await _requireAuthAttribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsInstanceOf<StatusCodeResult>(context.Result);
            var result = (StatusCodeResult)context.Result;
            Assert.That(result.StatusCode, Is.EqualTo(503));
        }

        [Test]
        public async Task OnAuthorizationAsync_AuthServiceUnavailable_Returns503()
        {
            // Arrange
            var token = "validtoken";
            var authHeader = $"{BEARER_PREFIX}{token}";

            // Mock the IServiceProvider to return the ServiceAuthController mock
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceAuthController)))
                .Returns(_mockServiceAuthController.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };
            httpContext.Request.Headers[AUTH_HEADER_NAME] = authHeader;

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            _mockServiceAuthController.Setup(s => s.VerifyTokenAsync(token))
                .ThrowsAsync(new HttpRequestException("Service unavailable"));

            // Act
            await _requireAuthAttribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(context.Result);
            var result = (ObjectResult)context.Result;
            Assert.That(result.StatusCode, Is.EqualTo(503));
            var serializer = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var expectedJson = JsonSerializer.Serialize(new { Message = "Authentication service is currently unavailable" }, serializer);
            var actualJson = JsonSerializer.Serialize(result.Value, serializer);
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task OnAuthorizationAsync_AuthServiceTimeout_Returns503()
        {
            // Arrange
            var token = "validtoken";
            var authHeader = $"{BEARER_PREFIX}{token}";

            // Mock the IServiceProvider to return the ServiceAuthController mock
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceAuthController)))
                .Returns(_mockServiceAuthController.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };
            httpContext.Request.Headers[AUTH_HEADER_NAME] = authHeader;

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            _mockServiceAuthController.Setup(s => s.VerifyTokenAsync(token))
                .ThrowsAsync(new TimeoutException("Timeout occurred"));

            // Act
            await _requireAuthAttribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(context.Result);
            var result = (ObjectResult)context.Result;
            Assert.That(result.StatusCode, Is.EqualTo(503));
            var serializer = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var expectedJson = JsonSerializer.Serialize(new { Message = "Authentication service timed out" }, serializer);
            var actualJson = JsonSerializer.Serialize(result.Value, serializer);
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task OnAuthorizationAsync_GeneralException_Returns500()
        {
            // Arrange
            var token = "validtoken";
            var authHeader = $"{BEARER_PREFIX}{token}";

            // Mock the IServiceProvider to return the ServiceAuthController mock
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceAuthController)))
                .Returns(_mockServiceAuthController.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };
            httpContext.Request.Headers[AUTH_HEADER_NAME] = authHeader;

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            _mockServiceAuthController.Setup(s => s.VerifyTokenAsync(token))
                .ThrowsAsync(new Exception("Some error"));

            // Act
            await _requireAuthAttribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(context.Result);
            var result = (ObjectResult)context.Result;
            Assert.That(result.StatusCode, Is.EqualTo(500));
            var serializer = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var expectedJson = JsonSerializer.Serialize(new { Message = "Token validation failed: Some error" }, serializer);
            var actualJson = JsonSerializer.Serialize(result.Value, serializer);
            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task OnAuthorizationAsync_AuthorizationHeaderIncludesKeyInValue_ProcessesCorrectly()
        {
            // Arrange
            var tokenValue = "Authorization: Bearer validtoken";
            var expectedToken = "validtoken";

            // Mock the IServiceProvider to return the ServiceAuthController mock
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceAuthController)))
                .Returns(_mockServiceAuthController.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };
            httpContext.Request.Headers[AUTH_HEADER_NAME] = tokenValue;

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            _mockServiceAuthController.Setup(s => s.VerifyTokenAsync(expectedToken))
                .ReturnsAsync(new ObjectResult(null) { StatusCode = 200 });

            // Act
            await _requireAuthAttribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsNull(context.Result);
        }
    }
}