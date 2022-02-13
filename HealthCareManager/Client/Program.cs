using Blazored.LocalStorage;
using HealthCareManager.Client;
using HealthCareManager.Client.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using MudBlazor.Services;
using Refit;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var settings = new RefitSettings();
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};
settings.ContentSerializer = new SystemTextJsonContentSerializer(options);

builder.Services.AddRefitClient<IServerApi>(settings)
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddSingleton<HubConnection>(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
      .WithUrl(navigationManager.ToAbsoluteUri("/notify"))
      .WithAutomaticReconnect()
      .Build();
});

builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthStateProvider>());

builder.Services
    .AddOptions()
    .AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
});

await builder.Build().RunAsync();