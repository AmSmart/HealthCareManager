using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareManager.Client;
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorageService;

    public AuthHeaderHandler(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (await _localStorageService.ContainKeyAsync("authToken"))
        {
            string token = await _localStorageService.GetItemAsync<string>("authToken");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
