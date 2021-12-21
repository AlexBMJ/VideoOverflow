using ISystemClock = Microsoft.AspNetCore.Authentication.ISystemClock;

namespace VideoOverflow.Server.Integration.Tests;


/* This code has directly been taken from: https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Server.Integration.Tests/TestAuthHandler.cs */
internal sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    /// <summary>
    /// Handles Authentication for our tests
    /// </summary>
    /// <returns> The result of the Authentication</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Test user"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}