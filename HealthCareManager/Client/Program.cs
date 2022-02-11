using Blazored.LocalStorage;
using HealthCareManager.Client;
using HealthCareManager.Client.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Refit;
using System.Net.Http.Headers;
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
// builder.Services.AddHttpClient("HealthCareManager.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
//   .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

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


// Supply HttpClient instances that include access tokens when making requests to the server project
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("HealthCareManager.ServerAPI"));



await builder.Build().RunAsync();

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorageService;

    public AuthHeaderHandler(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var auth = request.Headers.Authorization;

        //if (auth != null)
        //{
            if (await _localStorageService.ContainKeyAsync("authToken"))
            {
                string token = await _localStorageService.GetItemAsync<string>("authToken");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        //}

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}