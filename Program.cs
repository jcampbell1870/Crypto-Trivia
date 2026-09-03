using Crypto_Trivia.Components;
using Crypto_Trivia.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register game services
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<TokenRewardService>();
builder.Services.AddSingleton<RewardIssuerService>();
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("issuer", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseRateLimiter();

app.MapPost("/api/issuer/submit-score", async (
    RewardClaimRequest request,
    RewardIssuerService issuer,
    CancellationToken cancellationToken) =>
{
    try
    {
        return Results.Ok(await issuer.SubmitAsync(request, cancellationToken));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Problem(ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
}).RequireRateLimiting("issuer");

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
