using Microsoft.AspNetCore.WebUtilities;
using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Api.ApiModels.User;
using Net.SimpleBlog.Api.Configurations.Policies;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Net.SimpleBlog.E2ETests.Base;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _defaultSerializerOptions;
    private string _token;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _defaultSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCasePolicy(),
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task AuthenticateAsync(string email, string password)
    {
        var loginPayload = new { Email = email, Password = password };
        var response = await _httpClient.PostAsync("/users/authenticate",
            new StringContent(
                JsonSerializer.Serialize(loginPayload, _defaultSerializerOptions),
                Encoding.UTF8,
                "application/json"));

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody, _defaultSerializerOptions);
        _token = authResponse.Token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(
        string route,
        object payload
        )
        where TOutput : class
    {
        var request = new HttpRequestMessage(HttpMethod.Post, route)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload, _defaultSerializerOptions),
                Encoding.UTF8,
                "application/json"
            )
        };

        if (!string.IsNullOrEmpty(_token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        var response = await _httpClient.SendAsync(request);

        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }


    public async Task<(HttpResponseMessage?, TOutupt?)> Get<TOutupt>(
        string route,
        object? queryStringParametersObject = null
    )
    where TOutupt : class
    {
        var url = PrepareGetRoute(route, queryStringParametersObject);
        var response = await _httpClient.GetAsync(
                url
            );

        var output = await GetOutput<TOutupt>(response);

        return (response, output);
    }

    private string PrepareGetRoute(
        string route, object?
        queryStringParametersObject
    )
    {
        if (queryStringParametersObject is null)
            return route;
        var parametersJson = JsonSerializer.Serialize(
            queryStringParametersObject,
            _defaultSerializerOptions
        );
        var parametersDictionary = Newtonsoft.Json.JsonConvert
            .DeserializeObject<Dictionary<string, string>>(parametersJson);

        return QueryHelpers.AddQueryString(route, parametersDictionary!);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(
        string route
    )
    where TOutput : class
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, route);

        if (!string.IsNullOrEmpty(_token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        var response = await _httpClient.SendAsync(request);

        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }


    public async Task<(HttpResponseMessage?, TOutupt?)> Put<TOutupt>(
        string route,
        object payload
    )
        where TOutupt : class
    {
        var response = await _httpClient.PutAsync(
                route,
                new StringContent(
                    JsonSerializer.Serialize(
                        payload,
                        _defaultSerializerOptions
                    ),
                    Encoding.UTF8,
                    "application/json"
                )
        );

        var output = await GetOutput<TOutupt>(response);

        return (response, output);
    }

    private async Task<TOutupt?> GetOutput<TOutupt>(HttpResponseMessage response)
        where TOutupt : class
    {
        var outputString = await response.Content.ReadAsStringAsync();
        TOutupt? output = null;

        if (!string.IsNullOrWhiteSpace(outputString))
        {
            output = JsonSerializer.Deserialize<TOutupt>(
                outputString,
                _defaultSerializerOptions
            );
        }

        return output;
    }
}
