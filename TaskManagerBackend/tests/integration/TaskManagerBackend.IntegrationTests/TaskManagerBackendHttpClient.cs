#region Usings

using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
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
    
    public async Task<HttpResponseMessage> Post(string endpoint, object request)
    {
        return await _httpClient.PostAsJsonAsync(endpoint, request);
    }

    #region Auth

    public async Task<HttpResponseMessage> SignUp(UserSignUpRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/auth/signup", request);
    }

    public async Task<HttpResponseMessage> LogIn(UserLogInRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/auth/login", request);
    }

    #endregion

    #region Tracking
    
    public async Task<HttpResponseMessage> CreateTrackingLog(TrackingLogCreateRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/tracking/logs", request);
    }

    public async Task<HttpResponseMessage> GetTrackingLogById(int id)
    {
        return await _httpClient.GetAsync($"api/tracking/logs/{id}");
    }

    public async Task<HttpResponseMessage> GetTrackingLogs()
    {
        return await _httpClient.GetAsync("api/tracking/logs");
    }

    public async Task<HttpResponseMessage> DeleteTrackingLogById(int id)
    {
        return await _httpClient.DeleteAsync($"api/tracking/logs/{id}");
    }
    
    public async Task<HttpResponseMessage> CreateTrackingLogEntry(TrackingLogEntryCreateRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/tracking/log-entries", request);
    }
    
    public async Task<HttpResponseMessage> GetTrackingLogEntryById(int id)
    {
        return await _httpClient.GetAsync($"api/tracking/log-entries/{id}");
    }

    public async Task<HttpResponseMessage> GetTrackingLogEntries()
    {
        return await _httpClient.GetAsync("api/tracking/log-entries");
    }
    
    public async Task<HttpResponseMessage> UpdateTrackingLogEntry(int id, 
                                                                  UpdateTrackingLogEntryRequest request)
    {
        return await _httpClient.PutAsJsonAsync($"api/tracking/log-entries/{id}", request);
    }

    public async Task<HttpResponseMessage> DeleteTrackingLogEntryById(int id)
    {
        return await _httpClient.DeleteAsync($"api/tracking/log-entries/{id}");
    }

    public async Task<HttpResponseMessage> CreateTrackingLogEntryStatus(TrackingLogEntryStatusCreateRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/tracking/statuses", request);
    }
    
    public async Task<HttpResponseMessage> DeleteTrackingLogEntryStatus(int id)
    {
        return await _httpClient.DeleteAsync($"api/tracking/statuses/{id}");
    }

    #endregion
}