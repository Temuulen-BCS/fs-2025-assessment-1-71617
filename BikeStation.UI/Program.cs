using BikeStation.UI.Components;
using BikeStation.UI.Services;

var builder = WebApplication.CreateBuilder(args);

var apiBaseAddress = new Uri(builder.Configuration["ApiBaseAddress"] ?? "https://localhost:7058/");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<StationsApiClient>(client =>
{
    client.BaseAddress = apiBaseAddress;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
