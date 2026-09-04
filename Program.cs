using Crypto_Trivia.Components;
using Crypto_Trivia.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

// Register game services
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<TokenRewardService>();
builder.Services.AddScoped<RewardIssuerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapPost("/api/issuer/submit-score", async (RewardSubmissionRequest request, RewardIssuerService issuerService) =>
{
    var result = await issuerService.SubmitScoreAsync(request.WalletAddress, request.Score);
    return Results.Json(result, statusCode: result.Success ? 200 : 400);
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
