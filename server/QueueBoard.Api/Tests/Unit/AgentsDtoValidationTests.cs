using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueBoard.Api.DTOs;

namespace QueueBoard.Api.Tests.Unit
{
    [TestClass]
    public class AgentsDtoValidationTests
    {
        [TestMethod]
        public void CreateAgentDto_HasRequiredAndStringLengthOnFirstName_AndEmailAttributes()
        {
            var ctors = typeof(CreateAgentDto).GetConstructors();
            Assert.IsTrue(ctors.Length > 0, "CreateAgentDto should have at least one constructor");
            var ctor = ctors[0];
            var firstParam = ctor.GetParameters().FirstOrDefault(p => p.Name != null && p.Name.Equals("FirstName", StringComparison.OrdinalIgnoreCase));
            var emailParam = ctor.GetParameters().FirstOrDefault(p => p.Name != null && p.Name.Equals("Email", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(firstParam, "Constructor parameter 'FirstName' not found");
            Assert.IsNotNull(emailParam, "Constructor parameter 'Email' not found");

            var reqOnFirst = firstParam.GetCustomAttributes(typeof(RequiredAttribute), inherit: false).Any();
            var lenOnFirst = firstParam.GetCustomAttributes(typeof(StringLengthAttribute), inherit: false).Any();
            var reqOnEmail = emailParam.GetCustomAttributes(typeof(RequiredAttribute), inherit: false).Any();
            var lenOnEmail = emailParam.GetCustomAttributes(typeof(StringLengthAttribute), inherit: false).Any();
            var emailAttr = emailParam.GetCustomAttributes(typeof(EmailAddressAttribute), inherit: false).Any();

            Assert.IsTrue(reqOnFirst, "FirstName should have [Required]");
            Assert.IsTrue(lenOnFirst, "FirstName should have [StringLength]");
            Assert.IsTrue(reqOnEmail, "Email should have [Required]");
            Assert.IsTrue(lenOnEmail, "Email should have [StringLength]");
            Assert.IsTrue(emailAttr, "Email should have [EmailAddress]");
        }

        [TestMethod]
        public void UpdateAgentDto_HasValidationAttributes_OnConstructorParameters()
        {
            var ctors = typeof(UpdateAgentDto).GetConstructors();
            Assert.IsTrue(ctors.Length > 0, "UpdateAgentDto should have at least one constructor");
            var ctor = ctors[0];
            var firstParam = ctor.GetParameters().FirstOrDefault(p => p.Name != null && p.Name.Equals("FirstName", StringComparison.OrdinalIgnoreCase));
            var emailParam = ctor.GetParameters().FirstOrDefault(p => p.Name != null && p.Name.Equals("Email", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(firstParam, "Constructor parameter 'FirstName' not found on UpdateAgentDto");
            Assert.IsNotNull(emailParam, "Constructor parameter 'Email' not found on UpdateAgentDto");

            var reqOnFirst = firstParam.GetCustomAttributes(typeof(RequiredAttribute), inherit: false).Any();
            var lenOnFirst = firstParam.GetCustomAttributes(typeof(StringLengthAttribute), inherit: false).Any();
            var reqOnEmail = emailParam.GetCustomAttributes(typeof(RequiredAttribute), inherit: false).Any();
            var lenOnEmail = emailParam.GetCustomAttributes(typeof(StringLengthAttribute), inherit: false).Any();
            var emailAttr = emailParam.GetCustomAttributes(typeof(EmailAddressAttribute), inherit: false).Any();

            Assert.IsTrue(reqOnFirst, "UpdateAgentDto.FirstName should have [Required]");
            Assert.IsTrue(lenOnFirst, "UpdateAgentDto.FirstName should have [StringLength]");
            Assert.IsTrue(reqOnEmail, "UpdateAgentDto.Email should have [Required]");
            Assert.IsTrue(lenOnEmail, "UpdateAgentDto.Email should have [StringLength]");
            Assert.IsTrue(emailAttr, "UpdateAgentDto.Email should have [EmailAddress]");
        }
    }
}
