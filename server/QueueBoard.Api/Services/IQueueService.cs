using System;
using System.Threading.Tasks;

namespace QueueBoard.Api.Services
{
    public interface IQueueService
    {
        Task DeleteAsync(Guid id);
    }
}
