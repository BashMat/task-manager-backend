#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Shared.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking.TrackingLogEntry;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenCreatingTrackingLogEntry(MsSqlTests fixture) : TrackingTestBase(fixture)
{
    [Fact]
    public async Task CreatingTrackingLogEntryIsSuccessful()
    {
        const string Title = "NewLogEntry";
        const string Description = "Test description";
        DateTime utcDateTimeBeforeRequest = new DateTimeService().UtcNow;

        HttpResponseMessage response = await CreateTrackingLogEntry(DefaultTrackingLog!.Id,
                                                                    DefaultTrackingLogEntryStatus!.Id,
                                                                    Title,
                                                                    Description);
        ServiceResponse<TrackingLogEntryGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Title.Should().Be(Title);
        content.Data.Description.Should().Be(Description);
        content.Data.CreatedBy.UserName.Should().Be(UserName);
        content.Data.CreatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.UpdatedBy.UserName.Should().Be(UserName);
        content.Data.UpdatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.TrackingLogId.Should().Be(DefaultTrackingLog.Id);
        content.Data.Status.Should().BeEquivalentTo(DefaultTrackingLogEntryStatus);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task CreatingTrackingLogEntryIsUnsuccessfulIfTrackingLogOrTrackingLogEntryStatusDoesNotExist(bool logExists,
                                                                                                              bool statusExists)
    {
        const string Title = "NewLogEntry";
        const string Description = "Test description";

        HttpResponseMessage response = await CreateTrackingLogEntry(logExists ? DefaultTrackingLog!.Id : Faker.Random.Int(min: 100),
                                                                    statusExists ? DefaultTrackingLogEntryStatus!.Id : Faker.Random.Int(min: 100),
                                                                    Title,
                                                                    Description);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Detail.Should().Be(MessageResources.CouldNotCreateMessage);
    }

    [Fact]
    public async Task CreatingTrackingLogEntryIsUnsuccessfulIfTitleIsNotSet()
    {
        var request = new { Property = 1 };

        HttpResponseMessage response = await HttpClient.Post("api/tracking/log-entries", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreatingTrackingLogEntryIsUnsuccessfulIfUserCannotEditTrackingLog()
    {
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
        const string Title = "NewLogEntry";
        const string Description = "Test description";

        HttpResponseMessage response = await CreateTrackingLogEntry(DefaultTrackingLog!.Id,
                                                                    DefaultTrackingLogEntryStatus!.Id,
                                                                    Title,
                                                                    Description);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(MessageResources.AccessDeniedMessage);
    }
}