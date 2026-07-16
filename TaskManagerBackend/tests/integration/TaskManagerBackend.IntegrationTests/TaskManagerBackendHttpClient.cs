#region Usings

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Features.Auth;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Json;

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
    
    public async Task<UserSignUpResponse> SignUpAndValidate(UserSignUpRequest request)
    {
        HttpResponseMessage signUpResponse = await SignUp(request);
        ServiceResponse<UserSignUpResponse>? signUpContent = 
            await signUpResponse.Content.ReadFromJsonAsync<ServiceResponse<UserSignUpResponse>>();
        signUpContent.Should().NotBeNull();
        signUpContent.Data.Should().NotBeNull();
        return signUpContent.Data;
    }
    
    public async Task<HttpResponseMessage> IssueTokenByPassword(string username,
                                                                string password)
    {
        IssueTokenRequest request = new()
                                    {
                                        GrantType = GrantTypes.PasswordGrantType,
                                        Username = username,
                                        Password = password
                                    };
        return await _httpClient.PostAsJsonAsync("api/auth/token", request);
    }
    
    public async Task<HttpResponseMessage> IssueTokenByRefreshToken(string refreshToken)
    {
        IssueTokenRequest request = new()
                                    {
                                        GrantType = GrantTypes.RefreshTokenGrantType,
                                        RefreshToken = refreshToken
                                    };
        return await _httpClient.PostAsJsonAsync("api/auth/token", request);
    }
    
    public async Task<IssueTokenResponse> IssueTokenByPasswordAndSetAuthorization(string username,
                                                                                  string password)
    {
        IssueTokenRequest request = new()
                                    {
                                        GrantType = GrantTypes.PasswordGrantType,
                                        Username = username,
                                        Password = password
                                    };
        HttpResponseMessage issueTokenResponse = await _httpClient.PostAsJsonAsync("api/auth/token", request);
        ServiceResponse<IssueTokenResponse>? issueTokenContent = 
            await issueTokenResponse.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();
        issueTokenContent.Should().NotBeNull();
        issueTokenContent.Data.Should().NotBeNull();
        SetAccessToken(issueTokenContent.Data.AccessToken);
        return issueTokenContent.Data;
    }
    
    public async Task<HttpResponseMessage> RevokeToken()
    {
        return await _httpClient.PostAsync("api/auth/revoke", null);
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
    
    public async Task<HttpResponseMessage> EditTrackingLog(TrackingLogEditRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/tracking/logs/edit",
                                                 request,
                                                 options: new JsonSerializerOptions
                                                          {
                                                              Converters = { new JsonOptionalConverter() },
                                                              DefaultIgnoreCondition =  System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                                                          });
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
    
    public async Task<HttpResponseMessage> EditTrackingLogEntry(TrackingLogEntryEditRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/tracking/log-entries/edit",
                                                 request,
                                                 options: new JsonSerializerOptions
                                                          {
                                                              Converters = { new JsonOptionalConverter() },
                                                              DefaultIgnoreCondition =  System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                                                          });
    }
    
    public async Task<HttpResponseMessage> MoveTrackingLogEntry(TrackingLogEntryMoveRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/tracking/log-entries/move",
                                                 request,
                                                 options: new JsonSerializerOptions
                                                          {
                                                              Converters = { new JsonOptionalConverter() },
                                                              DefaultIgnoreCondition =  System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                                                          });
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

    #region Users

    public async Task<HttpResponseMessage> GetCurrentUserData()
    {
        return await _httpClient.GetAsync("api/users/current");
    }
    
    public async Task<HttpResponseMessage> GetUserDataById(int id)
    {
        return await _httpClient.GetAsync($"api/users/{id}");
    }
    
    public async Task<HttpResponseMessage> UpdatePassword(UpdatePasswordRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/users/update-password", request);
    }

    #endregion
}