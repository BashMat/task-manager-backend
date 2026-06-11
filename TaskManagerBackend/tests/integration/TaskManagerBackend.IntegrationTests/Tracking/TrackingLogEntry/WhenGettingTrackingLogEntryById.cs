#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Domain.Shared.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking.TrackingLogEntry;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenGettingTrackingLogEntryById(MsSqlTests fixture) : TrackingTestBase(fixture)
{
    [Fact]
    public async Task GettingTrackingLogEntryByIdIsSuccessful()
    {
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                            DefaultTrackingLogEntryStatus!.Id);

        HttpResponseMessage response = await HttpClient.GetTrackingLogEntryById(createdTrackingLogEntry.Id);
        ServiceResponse<TrackingLogEntryGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().BeEquivalentTo(createdTrackingLogEntry);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task GettingTrackingLogEntryByIdIsFailedIfUserCannotGetTrackingLogEntry()
    {
        TrackingLogEntryGetResponse createdTrackingLogEntry = await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id, 
                                                                                                              DefaultTrackingLogEntryStatus!.Id);
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

        HttpResponseMessage response = await HttpClient.GetTrackingLogEntryById(createdTrackingLogEntry.Id);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(MessageResources.AccessDeniedMessage);
    }
}