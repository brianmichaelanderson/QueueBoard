using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Integration
{
    [TestCategory("Integration")]
    [TestClass]
    public class AgentsIntegrationTests
    {
        [TestMethod]
        public async System.Threading.Tasks.Task Create_Get_Update_Delete_HappyPath()
        {
            // Arrange
            var apiBase = System.Environment.GetEnvironmentVariable("TEST_API_BASE_URL") ?? "http://localhost:8080";
            using var client = new System.Net.Http.HttpClient { BaseAddress = new System.Uri(apiBase) };

            var createPayload = new { firstName = "Int", lastName = "Tester", email = $"int+{System.Guid.NewGuid()}@example.com", isActive = true };

            // Act: create
            var createResp = await client.PostAsJsonAsync("/agents", createPayload);
            createResp.EnsureSuccessStatusCode();
            Assert.AreEqual(System.Net.HttpStatusCode.Created, createResp.StatusCode);

            // Get location and ETag
            var location = createResp.Headers.Location?.ToString() ?? throw new System.Exception("Location header missing");
            createResp.Headers.TryGetValues("ETag", out var etagVals);
            var etag = etagVals is null ? null : System.Linq.Enumerable.FirstOrDefault(etagVals);
            Assert.IsNotNull(etag, "ETag header expected on create");

            // Act: GET created resource
            var getResp = await client.GetAsync(location);
            getResp.EnsureSuccessStatusCode();

            // Act: DELETE with If-Match header
            var req = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Delete, location);
            if (!string.IsNullOrWhiteSpace(etag)) req.Headers.TryAddWithoutValidation("If-Match", etag);
            var delResp = await client.SendAsync(req);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, delResp.StatusCode);

            // Act: GET again should be 404
            var getAfter = await client.GetAsync(location);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, getAfter.StatusCode);
        }

        [TestMethod]
        public void Update_With_Stale_ETag_Returns_409()
        {
            // Arrange - use the running API
            var apiBase = System.Environment.GetEnvironmentVariable("TEST_API_BASE_URL") ?? "http://localhost:8080";
            using var client = new System.Net.Http.HttpClient { BaseAddress = new System.Uri(apiBase) };

            var createPayload = new { firstName = "Int", lastName = "Stale", email = $"int+{System.Guid.NewGuid()}@example.com", isActive = true };
            var createResp = client.PostAsJsonAsync("/agents", createPayload).GetAwaiter().GetResult();
            createResp.EnsureSuccessStatusCode();
            var location = createResp.Headers.Location?.ToString() ?? throw new System.Exception("Location header missing");
            createResp.Headers.TryGetValues("ETag", out var etagVals);
            var originalEtag = etagVals is null ? null : System.Linq.Enumerable.FirstOrDefault(etagVals);
            Assert.IsNotNull(originalEtag, "ETag expected on create");

            // Act: perform a successful update using the current ETag to advance the RowVersion
            var updatePayload = new { firstName = "Int2", lastName = "StaleUpdated", email = createPayload.email, isActive = true };
            var updateReq = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Put, location)
            {
                Content = System.Net.Http.Json.JsonContent.Create(updatePayload)
            };
            updateReq.Headers.TryAddWithoutValidation("If-Match", originalEtag);
            var updateResp = client.SendAsync(updateReq).GetAwaiter().GetResult();
            // Update should succeed
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, updateResp.StatusCode);

            // Act: attempt another update using the stale original ETag - should result in 409 Conflict
            var staleUpdatePayload = new { firstName = "Int3", lastName = "StaleAttempt", email = createPayload.email, isActive = true };
            var staleReq = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Put, location)
            {
                Content = System.Net.Http.Json.JsonContent.Create(staleUpdatePayload)
            };
            staleReq.Headers.TryAddWithoutValidation("If-Match", originalEtag);
            var staleResp = client.SendAsync(staleReq).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.Conflict, staleResp.StatusCode);
        }
    }
}
