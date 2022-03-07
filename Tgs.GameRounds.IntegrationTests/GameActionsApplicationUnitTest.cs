using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tgs.GameRounds.IntegrationTests
{
    internal class GameActionsApplicationUnitTest : WebApplicationFactory<Program>
    {
        private readonly Action<IWebHostBuilder>? _webHostConfigure;

        public GameActionsApplicationUnitTest(Action<IWebHostBuilder>? webHostConfigure = null)
        {
            _webHostConfigure = webHostConfigure;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
            => _webHostConfigure?.Invoke(builder);
    }
}
