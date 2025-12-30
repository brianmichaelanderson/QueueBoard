using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueBoard.Api.DTOs;

namespace QueueBoard.Api.Tests.Unit;

[TestClass]
public class QueueDtoValidationTests
{
    [TestMethod]
    public void Name_IsRequired()
    {
        // Arrange: create a QueueDto with an empty Name (should be invalid once attributes are added)
        var dto = new QueueDto(Guid.NewGuid(), string.Empty, "desc", true, DateTimeOffset.UtcNow, null);

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);

        // Assert: we expect validation to fail because Name should be required (TDD: this test drives adding [Required])
        Assert.IsFalse(isValid, "Expected validation to fail when Name is empty. Add [Required] to QueueDto.Name to make this pass.");
    }
}
