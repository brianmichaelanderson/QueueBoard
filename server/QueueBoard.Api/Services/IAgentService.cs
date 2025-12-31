using System;
using System.Threading.Tasks;
using QueueBoard.Api.DTOs;

namespace QueueBoard.Api.Services
{
    public interface IAgentService
    {
        Task<AgentDto> CreateAsync(CreateAgentDto dto);
        Task<AgentDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateAgentDto dto, string? ifMatch = null);
        Task DeleteAsync(Guid id, string? ifMatch = null);
    }
}
