using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using QueueBoard.Api.Services;

namespace QueueBoard.Api.Tests.Unit
{
    [TestClass]
    public class AgentsServiceTests
    {
        [TestMethod]
        public async System.Threading.Tasks.Task Create_SavesEntity_ReturnsDto()
        {
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var db = new QueueBoard.Api.QueueBoardDbContext(options);
            var svc = new AgentService(db, NullLogger<AgentService>.Instance);

            var dto = new QueueBoard.Api.DTOs.CreateAgentDto("Jane", "Doe", "jane.svc@example.com", true);
            var created = await svc.CreateAsync(dto);

            Assert.IsNotNull(created);
            Assert.IsFalse(string.IsNullOrWhiteSpace(created.RowVersion));

            var persisted = await db.Agents.FindAsync(created.Id);
            Assert.IsNotNull(persisted);
            Assert.AreEqual("Jane", persisted.FirstName);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetById_ReturnsEntityOrNull()
        {
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var db = new QueueBoard.Api.QueueBoardDbContext(options);
            var svc = new AgentService(db, NullLogger<AgentService>.Instance);

            var agent = new QueueBoard.Api.Entities.Agent { Id = Guid.NewGuid(), FirstName = "Sam", LastName = "Agent", Email = "sam@example.com", IsActive = true, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
            db.Agents.Add(agent);
            await db.SaveChangesAsync();

            var found = await svc.GetByIdAsync(agent.Id);
            Assert.IsNotNull(found);
            Assert.AreEqual(agent.Email, found!.Email);

            var missing = await svc.GetByIdAsync(Guid.NewGuid());
            Assert.IsNull(missing);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Update_StaleToken_ThrowsDbUpdateConcurrencyException()
        {
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var db = new QueueBoard.Api.QueueBoardDbContext(options);
            var svc = new AgentService(db, NullLogger<AgentService>.Instance);

            var agent = new QueueBoard.Api.Entities.Agent { Id = Guid.NewGuid(), FirstName = "Old", LastName = "Agent", Email = "old@example.com", IsActive = true, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
            db.Agents.Add(agent);
            await db.SaveChangesAsync();

            // capture token
            var token = Convert.ToBase64String(BitConverter.GetBytes(agent.UpdatedAt.UtcTicks));

            // first update succeeds
            var update1 = new QueueBoard.Api.DTOs.UpdateAgentDto("New", "Agent", "new@example.com", true, token);
            await svc.UpdateAsync(agent.Id, update1);

            // attempt stale update using original token
            var stale = new QueueBoard.Api.DTOs.UpdateAgentDto("Stale", "Agent", "stale@example.com", true, token);
            await Assert.ThrowsExceptionAsync<Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException>(async () => await svc.UpdateAsync(agent.Id, stale));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Delete_Idempotent_ReturnsNoContent()
        {
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var db = new QueueBoard.Api.QueueBoardDbContext(options);
            var svc = new AgentService(db, NullLogger<AgentService>.Instance);

            var agent = new QueueBoard.Api.Entities.Agent { Id = Guid.NewGuid(), FirstName = "ToDelete", LastName = "Agent", Email = "d@example.com", IsActive = true, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
            db.Agents.Add(agent);
            await db.SaveChangesAsync();

            // first delete
            await svc.DeleteAsync(agent.Id);
            var after = await db.Agents.FindAsync(agent.Id);
            Assert.IsNull(after);

            // second delete should not throw
            await svc.DeleteAsync(agent.Id);
        }
    }
}
