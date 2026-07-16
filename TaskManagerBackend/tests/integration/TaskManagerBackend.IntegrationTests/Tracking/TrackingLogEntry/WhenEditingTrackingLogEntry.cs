#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Json;
using TaskManagerBackend.Domain.Shared.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking.TrackingLogEntry;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenEditingTrackingLogEntry(MsSqlTests fixture) : TrackingTestBase(fixture)
{
    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, true, false)]
    [InlineData(true, null, true)]
    [InlineData(true, null, false)]
    [InlineData(true, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    [InlineData(false, true, false)]
    [InlineData(false, null, true)]
    [InlineData(false, null, false)]
    [InlineData(false, false, true)]
    public async Task EditingTrackingLogEntryIsSuccessful(bool isTitleSet, bool? isDescriptionSet, bool isStatusSet)
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();
        TrackingLogEntryStatusGetResponse createdSourceTrackingLogEntryStatus =
            await CreateTrackingLogEntryStatusAndValidateResponse(createdTrackingLog.Id);
        TrackingLogEntryStatusGetResponse createdTargetTrackingLogEntryStatus =
            await CreateTrackingLogEntryStatusAndValidateResponse(createdTrackingLog.Id);
        TrackingLogEntryGetResponse createdTrackingLogEntry =
            await CreateTrackingLogEntryAndValidateResponse(createdTrackingLog.Id,
                                                            createdSourceTrackingLogEntryStatus.Id);
        Optional<string> title = isTitleSet ? new Optional<string>(Faker.Random.Words()) : new Optional<string>();
        
        Optional<int> status = isStatusSet ? new Optional<int>(createdTargetTrackingLogEntryStatus.Id) : new Optional<int>();
        Optional<string> description = isDescriptionSet switch
                                        {
                                            true => new Optional<string>(Faker.Random.Words()),
                                            false => new Optional<string>(),
                                            var _ => new Optional<string>(null)
                                        };

        HttpResponseMessage response = await EditTrackingLogEntry(createdTrackingLogEntry.Id,
                                                                  title,
                                                                  status,
                                                                  description);
        ServiceResponse<TrackingLogEntryGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Id.Should().Be(createdTrackingLogEntry.Id);
        content.Data.Title.Should().Be(isTitleSet ? title.Value : createdTrackingLogEntry.Title);
        content.Data.Description.Should().Be(isDescriptionSet is true or null 
                                                 ? description.Value
                                                 : createdTrackingLogEntry.Description);
        content.Data.TrackingLogId.Should().Be(createdTrackingLogEntry.TrackingLogId);
        content.Data.Status.Should().BeEquivalentTo(isStatusSet ? createdTargetTrackingLogEntryStatus : createdSourceTrackingLogEntryStatus);
        content.Data.CreatedBy.UserName.Should().Be(UserName);
        content.Data.CreatedAt.Should().Be(createdTrackingLogEntry.CreatedAt);
        content.Data.UpdatedBy.UserName.Should().Be(UserName);
        content.Data.UpdatedAt.Should().BeAfter(createdTrackingLogEntry.UpdatedAt);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task EditingTrackingLogEntryIsFailedIfNoAttributeIsSet()
    {
        TrackingLogEntryGetResponse createdTrackingLog = await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                                                                         DefaultTrackingLogEntryStatus!.Id);

        HttpResponseMessage response = await EditTrackingLogEntry(createdTrackingLog.Id,
                                                                  new Optional<string>(),
                                                                  new Optional<int>(),
                                                                  new Optional<string>());
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Detail.Should().Be(MessageResources.ValidationErrorTitle);
    }
    
    [Fact]
    public async Task EditingTrackingLogEntryIsFailedIfTrackingLogEntryDoesNotExist()
    {
        HttpResponseMessage response = await EditTrackingLogEntry(Faker.Random.Int(100),
                                                                  new Optional<string>(Faker.Random.Words()),
                                                                  new Optional<int>(),
                                                                  new Optional<string>(Faker.Random.Words()));
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.NotFound);
        content.Detail.Should().Be(MessageResources.ResourceDoesNotExist);
    }
    
    [Fact]
    public async Task EditingTrackingLogEntryIsFailedIfUserCannotEditTrackingLogEntry()
    {
        TrackingLogEntryGetResponse created = await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
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
        
        HttpResponseMessage response = await EditTrackingLogEntry(created.Id,
                                                                  new Optional<string>(Faker.Random.Words()),
                                                                  new Optional<int>(),
                                                                  new Optional<string>(Faker.Random.Words()));
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(MessageResources.AccessDeniedMessage);
    }
    
    [Fact]
    public async Task EditingTrackingLogEntryIsFailedIfUserSetsTitleToNull()
    {
        TrackingLogEntryGetResponse created = await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                                                              DefaultTrackingLogEntryStatus!.Id);
        
        HttpResponseMessage response = await EditTrackingLogEntry(created.Id,
                                                                  new Optional<string>(null),
                                                                  new Optional<int>(),
                                                                  new Optional<string>(Faker.Random.Words()));
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Detail.Should().Be(MessageResources.ValidationErrorTitle);
    }
}