using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace VacationRental.Api.Tests
{
    [CollectionDefinition("Integration")]
    public sealed class IntegrationFixture : IDisposable, ICollectionFixture<IntegrationFixture>
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;

        public HttpClient Client { get; }

        public IntegrationFixture()
        {
            _webApplicationFactory = new WebApplicationFactory<Program>();

            Client = _webApplicationFactory.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            _webApplicationFactory.Dispose();
        }
    }
}
