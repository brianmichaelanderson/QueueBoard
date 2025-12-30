using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueBoard.Api.DTOs;
using QueueBoard.Api.Validators;

namespace QueueBoard.Api.Tests.Unit
{
    [TestClass]
    public class QueueCrossFieldValidatorsTests
    {
        [TestMethod]
        public void NameEqualsDescription_IsInvalid()
        {
            // Arrange: Name and Description intentionally identical to drive a cross-field validation failure
            var dto = new QueueDto(Guid.NewGuid(), "SameValue", "SameValue", true, DateTimeOffset.UtcNow);

            var validator = new QueueCrossFieldValidator();

            // Act
            var isValid = validator.Validate(dto, out var errors);

            // Assert: we expect validation to fail (TDD - red). Implement validator to make this pass.
            Assert.IsFalse(isValid, "Expected cross-field validator to mark identical Name and Description as invalid.");
            Assert.IsTrue(errors != null && errors.Any(), "Expected at least one validation error describing the cross-field violation.");
        }
    }
}
