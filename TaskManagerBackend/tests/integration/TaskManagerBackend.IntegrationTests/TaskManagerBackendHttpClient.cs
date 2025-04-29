#region Usings

using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.IntegrationTests;

public class TaskManagerBackendHttpClient
{
    private readonly HttpClient _httpClient;

    private TaskManagerBackendHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public static implicit operator TaskManagerBackendHttpClient(HttpClient httpClient)
    {
        return new TaskManagerBackendHttpClient(httpClient);
    }
    
    public void SetAccessToken(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
    }

    #region Auth

    public async Task<HttpResponseMessage> SignUp(UserSignUpRequestDto request)
    {
        return await _httpClient.PostAsJsonAsync("api/auth/signup", request);
    }

    public async Task<HttpResponseMessage> LogIn(UserLogInRequestDto request)
    {
        return await _httpClient.PostAsJsonAsync("api/auth/login", request);
    }

    #endregion
}