#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Domain.Shared.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking.TrackingLog;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenGettingTrackingLogById(MsSqlTests fixture) : TrackingTestBase(fixture)
{
    [Fact]
    public async Task GettingTrackingLogByIdIsSuccessful()
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();

        HttpResponseMessage response = await HttpClient.GetTrackingLogById(createdTrackingLog.Id);
        ServiceResponse<TrackingLogGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().BeEquivalentTo(createdTrackingLog);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task GettingTrackingLogByIdIsFailedIfUserCannotGetTrackingLog()
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();
        string userName = Faker.Internet.UserName();
        string email = Faker.Internet.Email();
        string password = Faker.Internet.Password(length: 10);
        UserSignUpRequest signUpRequest = new()
                                          {
                                              UserName = userName, 
                                              Email = email,
                                              Password = password
                                          };

        await HttpClient.SignUp(signUpRequest);
        await HttpClient.IssueTokenByPasswordAndSetAuthorization(userName, 
                                                                 password);

        HttpResponseMessage response = await HttpClient.GetTrackingLogById(createdTrackingLog.Id);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(MessageResources.AccessDeniedMessage);
    }
}