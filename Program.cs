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
builder.Services.AddSingleton<RewardIssuerService>();

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

app.MapStaticAssets();

app.MapGet("/api/issuer/health", () => Results.Ok(new
{
    status = "ok",
    issuer = "Crypto Trivia Issuer"
}));

app.MapPost("/api/issuer/submit-score", (RewardIssuerService issuer, RewardSubmissionRequest request) =>
{
    var result = issuer.SubmitReward(request);
    return result.Success
        ? Results.Ok(result)
        : Results.BadRequest(result);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
