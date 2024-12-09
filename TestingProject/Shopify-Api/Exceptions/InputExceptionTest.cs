using Shopify_Api.Exceptions;



using NUnit.Framework;
using System;

namespace TestingProject.Shopify_Api.Exceptions
{
    [TestFixture]
    public class InputExceptionTest
    {
        [Test]
        public void InputException_DefaultConstructor_CreatesInstance()
        {
            // Act
            var exception = new InputException();

            // Assert
            Assert.NotNull(exception);
            Assert.IsInstanceOf<InputException>(exception);
            Assert.AreEqual("Exception of type 'Shopify_Api.Exceptions.InputException' was thrown.", exception.Message);
        }

        [Test]
        public void InputException_ConstructorWithMessage_CreatesInstanceWithMessage()
        {
            // Arrange
            string message = "This is a custom error message";

            // Act
            var exception = new InputException(message);

            // Assert
            Assert.NotNull(exception);
            Assert.IsInstanceOf<InputException>(exception);
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void InputException_ConstructorWithMessageAndInnerException_CreatesInstanceWithMessageAndInnerException()
        {
            // Arrange
            string message = "This is a custom error message";
            var innerException = new Exception("This is an inner exception");

            // Act
            var exception = new InputException(message, innerException);

            // Assert
            Assert.NotNull(exception);
            Assert.IsInstanceOf<InputException>(exception);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [Test]
        public void InputException_WithInnerException_ThrowsCorrectly()
        {
            // Arrange
            var innerException = new Exception("This is an inner exception");

            // Act & Assert
            var ex = Assert.Throws<InputException>(() => throw new InputException("An error occurred", innerException));
            Assert.AreEqual("An error occurred", ex.Message);
            Assert.AreEqual(innerException, ex.InnerException);
        }
    }
}
