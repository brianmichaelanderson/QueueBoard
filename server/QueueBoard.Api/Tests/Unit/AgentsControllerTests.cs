using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;

namespace QueueBoard.Api.Tests.Unit
{
    [TestClass]
    public class AgentsControllerTests
    {
        [TestMethod]
        public async System.Threading.Tasks.Task Create_ReturnsCreated_WithETag()
        {
            // Arrange: in-memory DB and controller
            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString()).Options;
            var db = new QueueBoard.Api.QueueBoardDbContext(options);
            var controller = new QueueBoard.Api.Controllers.AgentsController(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<QueueBoard.Api.Controllers.AgentsController>.Instance);
            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() };

            var dto = new QueueBoard.Api.DTOs.CreateAgentDto("Jane", "Doe", "jane.doe@example.com", true);

            // Act: call Create via reflection if implemented, otherwise fail the test to drive TDD
            var method = typeof(QueueBoard.Api.Controllers.AgentsController).GetMethod("Create", new[] { typeof(QueueBoard.Api.DTOs.CreateAgentDto) });
            if (method is null)
            {
                Assert.Fail("AgentsController.Create(CreateAgentDto) is not implemented yet.");
                return;
            }

            var invokeResult = method.Invoke(controller, new object[] { dto });
            var task = (System.Threading.Tasks.Task)invokeResult!;
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            var actionResult = resultProperty?.GetValue(task) as Microsoft.AspNetCore.Mvc.IActionResult;

            // Assert: expect CreatedAtActionResult and an ETag header set on the response
            Assert.IsNotNull(actionResult, "Create did not return an IActionResult");
            Assert.IsInstanceOfType(actionResult, typeof(Microsoft.AspNetCore.Mvc.CreatedAtActionResult));
            Assert.IsTrue(controller.Response.Headers.ContainsKey("ETag"), "Response did not include ETag header");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetById_ReturnsDto_OrNotFound()
        {
            // Arrange: in-memory DB with a seeded agent
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString()).Options;
            var db = new QueueBoard.Api.QueueBoardDbContext(options);
            var agent = new QueueBoard.Api.Entities.Agent {
                Id = System.Guid.NewGuid(),
                FirstName = "Sam",
                LastName = "Agent",
                Email = "sam.agent@example.com",
                IsActive = true,
                CreatedAt = System.DateTimeOffset.UtcNow,
                UpdatedAt = System.DateTimeOffset.UtcNow
            };
            db.Agents.Add(agent);
            await db.SaveChangesAsync();

            var controller = new QueueBoard.Api.Controllers.AgentsController(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<QueueBoard.Api.Controllers.AgentsController>.Instance);
            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() };

            // Act: call existing agent
            var resultExisting = await controller.GetById(agent.Id);

            // Assert existing found
            Assert.IsInstanceOfType(resultExisting, typeof(Microsoft.AspNetCore.Mvc.OkObjectResult));
            Assert.IsTrue(controller.Response.Headers.ContainsKey("ETag"), "ETag header should be present for GET by id");

            // Act: call missing agent
            var resultMissing = await controller.GetById(System.Guid.NewGuid());
            Assert.IsInstanceOfType(resultMissing, typeof(Microsoft.AspNetCore.Mvc.NotFoundResult));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Update_WithStaleRowVersion_ReturnsConflict()
        {
            // Arrange: in-memory DB and controller
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString()).Options;
            var db = new QueueBoard.Api.QueueBoardDbContext(options);
            var agent = new QueueBoard.Api.Entities.Agent
            {
                Id = System.Guid.NewGuid(),
                FirstName = "Old",
                LastName = "Agent",
                Email = "old.agent@example.com",
                IsActive = true,
                CreatedAt = System.DateTimeOffset.UtcNow,
                UpdatedAt = System.DateTimeOffset.UtcNow
            };
            db.Agents.Add(agent);
            await db.SaveChangesAsync();

            var controller = new QueueBoard.Api.Controllers.AgentsController(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<QueueBoard.Api.Controllers.AgentsController>.Instance);
            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() };

            // Get current RowVersion token
            var getResult = await controller.GetById(agent.Id) as Microsoft.AspNetCore.Mvc.OkObjectResult;
            Assert.IsNotNull(getResult, "Expected GetById to return Ok");
            var dto = getResult!.Value as QueueBoard.Api.DTOs.AgentDto;
            Assert.IsNotNull(dto, "GetById did not return AgentDto");
            var originalToken = dto!.RowVersion;

            // First update with current token should succeed
            var updateDto = new QueueBoard.Api.DTOs.UpdateAgentDto("New", "Agent", "new@example.com", true, originalToken);
            var firstResponse = await controller.Update(agent.Id, updateDto);
            Assert.IsInstanceOfType(firstResponse, typeof(Microsoft.AspNetCore.Mvc.NoContentResult));

            // Second update using the stale token (originalToken) should result in a concurrency exception
            controller.Request.Headers["If-Match"] = $"\"{originalToken}\"";
            var staleUpdate = new QueueBoard.Api.DTOs.UpdateAgentDto("Stale", "Update", "stale@example.com", true, originalToken);
            await Assert.ThrowsExceptionAsync<Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException>(async () =>
            {
                await controller.Update(agent.Id, staleUpdate);
            });
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Delete_Idempotent_ReturnsNoContent()
        {
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString()).Options;
            var db = new QueueBoard.Api.QueueBoardDbContext(options);
            var agent = new QueueBoard.Api.Entities.Agent
            {
                Id = System.Guid.NewGuid(),
                FirstName = "ToDelete",
                LastName = "Agent",
                Email = "del.agent@example.com",
                IsActive = true,
                CreatedAt = System.DateTimeOffset.UtcNow,
                UpdatedAt = System.DateTimeOffset.UtcNow
            };
            db.Agents.Add(agent);
            await db.SaveChangesAsync();

            var controller = new QueueBoard.Api.Controllers.AgentsController(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<QueueBoard.Api.Controllers.AgentsController>.Instance);
            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() };

            // First delete should remove the entity
            var first = await controller.Delete(agent.Id);
            Assert.IsInstanceOfType(first, typeof(Microsoft.AspNetCore.Mvc.NoContentResult));

            // Second delete should still return NoContent (idempotent)
            var second = await controller.Delete(agent.Id);
            Assert.IsInstanceOfType(second, typeof(Microsoft.AspNetCore.Mvc.NoContentResult));
        }
    }
}
