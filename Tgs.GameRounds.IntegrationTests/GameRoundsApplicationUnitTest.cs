using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tgs.GameRounds.IntegrationTests;

internal class GameRoundsApplicationUnitTest(Action<IWebHostBuilder>? webHostConfigure = null) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => webHostConfigure?.Invoke(builder);
}
