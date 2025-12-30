using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace QueueBoard.Api.Tests.Integration.TestHelpers
{
    public static class ReadinessHelper
    {
        public static async Task WaitForApiAsync(string baseUrl, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(60);
            var deadline = DateTime.UtcNow + timeout.Value;
            using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var resp = await client.GetAsync("/health");
                    if (resp.IsSuccessStatusCode) return;
                }
                catch
                {
                    // ignore and retry
                }
                await Task.Delay(1000);
            }
            throw new TimeoutException($"API did not become ready at {baseUrl} within {timeout}");
        }
    }
}
