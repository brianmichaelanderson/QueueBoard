using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueBoard.Api.DTOs;

namespace QueueBoard.Api.Services
{
    public class AgentService : IAgentService
    {
        private readonly QueueBoardDbContext _db;
        private readonly ILogger<AgentService> _logger;

        public AgentService(QueueBoardDbContext db, ILogger<AgentService> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger;
        }

        public async Task<AgentDto> CreateAsync(CreateAgentDto dto)
        {
            var entity = new Entities.Agent
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                IsActive = dto.IsActive,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _db.Agents.Add(entity);
            await _db.SaveChangesAsync();

            var token = Convert.ToBase64String(BitConverter.GetBytes(entity.UpdatedAt.UtcTicks));
            return new AgentDto(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.IsActive, entity.CreatedAt, token);
        }

        public async Task<AgentDto?> GetByIdAsync(Guid id)
        {
            var dto = await _db.Agents.AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new { a.Id, a.FirstName, a.LastName, a.Email, a.IsActive, a.CreatedAt, a.UpdatedAt })
                .FirstOrDefaultAsync();

            if (dto is null) return null;
            var token = Convert.ToBase64String(BitConverter.GetBytes(dto.UpdatedAt.UtcTicks));
            return new AgentDto(dto.Id, dto.FirstName, dto.LastName, dto.Email, dto.IsActive, dto.CreatedAt, token);
        }

        public async Task UpdateAsync(Guid id, UpdateAgentDto dto, string? ifMatch = null)
        {
            var entity = await _db.Agents.FindAsync(id);
            if (entity is null) throw new KeyNotFoundException("Agent not found");

            var tokenSource = ifMatch ?? dto.RowVersion;
            if (string.IsNullOrWhiteSpace(tokenSource)) throw new ArgumentException("RowVersion/If-Match required");

            byte[] tokenBytes;
            try { tokenBytes = Convert.FromBase64String(tokenSource); } catch { throw new ArgumentException("Invalid RowVersion token"); }
            if (tokenBytes.Length < 8) throw new ArgumentException("Invalid RowVersion token");

            var providedTicks = BitConverter.ToInt64(tokenBytes, 0);
            if (providedTicks != entity.UpdatedAt.UtcTicks)
            {
                throw new DbUpdateConcurrencyException();
            }

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, string? ifMatch = null)
        {
            if (!string.IsNullOrWhiteSpace(ifMatch))
            {
                byte[] tokenBytes;
                try { tokenBytes = Convert.FromBase64String(ifMatch); } catch { throw new ArgumentException("Invalid If-Match token"); }
                if (tokenBytes.Length < 8) throw new ArgumentException("Invalid If-Match token");

                var providedTicks = BitConverter.ToInt64(tokenBytes, 0);
                var existingForCheck = await _db.Agents.FindAsync(id);
                if (existingForCheck is null) return; // idempotent
                if (providedTicks != existingForCheck.UpdatedAt.UtcTicks) throw new DbUpdateConcurrencyException();
            }

            var existing = await _db.Agents.FindAsync(id);
            if (existing is null) return; // idempotent

            _db.Agents.Remove(existing);
            await _db.SaveChangesAsync();
        }
    }
}
