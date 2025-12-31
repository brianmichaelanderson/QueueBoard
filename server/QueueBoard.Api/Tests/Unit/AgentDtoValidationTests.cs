using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueBoard.Api.DTOs;

namespace QueueBoard.Api.Tests.Unit
{
    [TestClass]
    public class AgentDtoValidationTests
    {
        private static IList<ValidationResult> Validate(object model)
        {
            var ctx = new ValidationContext(model);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, ctx, results, true);
            return results;
        }

        [TestMethod]
        public void ValidAgent_PassesValidation()
        {
            var dto = new AgentDto(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", true, DateTimeOffset.UtcNow, null);
            var results = Validate(dto);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void MissingFirstName_FailsRequired()
        {
            var dto = new AgentDto(Guid.NewGuid(), "", "Doe", "john.doe@example.com", true, DateTimeOffset.UtcNow, null);
            var results = Validate(dto);
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("FirstName") || (r.ErrorMessage ?? string.Empty).IndexOf("FirstName", StringComparison.OrdinalIgnoreCase) >= 0));
        }

        [TestMethod]
        public void InvalidEmail_FailsEmailAttribute()
        {
            var dto = new AgentDto(Guid.NewGuid(), "John", "Doe", "not-an-email", true, DateTimeOffset.UtcNow, null);
            var results = Validate(dto);
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email") || (r.ErrorMessage ?? string.Empty).IndexOf("email", StringComparison.OrdinalIgnoreCase) >= 0));
        }

        [TestMethod]
        public void LongFirstName_FailsStringLength()
        {
            var longName = new string('A', 101);
            var dto = new AgentDto(Guid.NewGuid(), longName, "Doe", "john.doe@example.com", true, DateTimeOffset.UtcNow, null);
            var results = Validate(dto);
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("FirstName") || (r.ErrorMessage ?? string.Empty).IndexOf("FirstName", StringComparison.OrdinalIgnoreCase) >= 0));
        }
    }
}
