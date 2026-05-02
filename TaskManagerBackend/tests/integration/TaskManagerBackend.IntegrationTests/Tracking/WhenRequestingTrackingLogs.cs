#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Json;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Shared.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenRequestingTrackingLogs : TrackingTestBase
{
    public WhenRequestingTrackingLogs(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task CreatingTrackingLogIsSuccessful()
    {
        DateTime utcDateTimeBeforeRequest = new DateTimeService().UtcNow;
        const string Title = "NewLog";
        const string Description = "Test description";

        HttpResponseMessage response = await CreateTrackingLog(Title, Description);
        ServiceResponse<TrackingLogGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Title.Should().Be(Title);
        content.Data.Description.Should().Be(Description);
        content.Data.CreatedBy.UserName.Should().Be(UserName);
        content.Data.CreatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.UpdatedBy.UserName.Should().Be(UserName);
        content.Data.UpdatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.TrackingLogEntries.Should().BeEmpty();
        content.Data.TrackingLogEntriesStatuses.Should().BeEmpty();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }

    [Fact]
    public async Task CreatingTrackingLogIsUnsuccessfulIfTitleIsNotSet()
    {
        var request = new { Property = 1 };

        HttpResponseMessage response = await HttpClient.Post("api/tracking/logs", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
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
    
    [Fact]
    public async Task GettingTrackingLogsIsSuccessful()
    {
        await CreateTrackingLog();

        HttpResponseMessage response = await HttpClient.GetTrackingLogs();
        ServiceResponse<IReadOnlyCollection<TrackingLogGetResponse>>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<IReadOnlyCollection<TrackingLogGetResponse>>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeEmpty();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Theory]
    [InlineData(true, true)]
    [InlineData(true, null)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, null)]
    public async Task EditingTrackingLogIsSuccessful(bool isTitleSet, bool? isDescriptionSet)
    {
        Optional<string> title = isTitleSet ? new Optional<string>(Faker.Random.Words()) : new Optional<string>();
        Optional<string> description = isDescriptionSet switch
                                        {
                                            true => new Optional<string>(Faker.Random.Words()),
                                            false => new Optional<string>(),
                                            var _ => new Optional<string>(null)
                                        };
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();

        HttpResponseMessage response = await EditTrackingLog(createdTrackingLog.Id, title, description);
        ServiceResponse<TrackingLogGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Title.Should().Be(isTitleSet ? title.Value : createdTrackingLog.Title);
        content.Data.Description.Should().Be(isDescriptionSet is true or null 
                                                 ? description.Value
                                                 : createdTrackingLog.Description);
        content.Data.CreatedBy.UserName.Should().Be(UserName);
        content.Data.CreatedAt.Should().Be(createdTrackingLog.CreatedAt);
        content.Data.UpdatedBy.UserName.Should().Be(UserName);
        content.Data.UpdatedAt.Should().BeAfter(createdTrackingLog.UpdatedAt);
        content.Data.TrackingLogEntries.Should().BeEmpty();
        content.Data.TrackingLogEntriesStatuses.Should().BeEmpty();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task EditingTrackingLogHasNoEffectIfNoAttributeIsSet()
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();

        HttpResponseMessage response = await EditTrackingLog(createdTrackingLog.Id, new Optional<string>(), new Optional<string>());
        ServiceResponse<TrackingLogGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Title.Should().Be(createdTrackingLog.Title);
        content.Data.Description.Should().Be(createdTrackingLog.Description);
        content.Data.CreatedBy.UserName.Should().Be(UserName);
        content.Data.CreatedAt.Should().Be(createdTrackingLog.CreatedAt);
        content.Data.UpdatedBy.UserName.Should().Be(UserName);
        content.Data.UpdatedAt.Should().Be(createdTrackingLog.UpdatedAt);
        content.Data.TrackingLogEntries.Should().BeEmpty();
        content.Data.TrackingLogEntriesStatuses.Should().BeEmpty();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task EditingTrackingLogIsFailedIfTrackingLogDoesNotExist()
    {
        HttpResponseMessage response = await EditTrackingLog(Faker.Random.Int(min: 100),
                                                             new Optional<string>(Faker.Random.Words()),
                                                             new Optional<string>(Faker.Random.Words()));
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.NotFound);
        content.Detail.Should().Be(MessageResources.ResourceDoesNotExist);
    }
    
    [Fact]
    public async Task EditingTrackingLogIsFailedIfUserCannotEditTrackingLog()
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
        
        HttpResponseMessage response = await EditTrackingLog(createdTrackingLog.Id,
                                                             new Optional<string>(Faker.Random.Words()),
                                                             new Optional<string>(Faker.Random.Words()));
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(MessageResources.AccessDeniedMessage);
    }
    
    [Fact]
    public async Task EditingTrackingLogIsFailedIfUserSetsTitleToNull()
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();
        
        HttpResponseMessage response = await EditTrackingLog(createdTrackingLog.Id,
                                                             new Optional<string>(null),
                                                             new Optional<string>(Faker.Random.Words()));
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Detail.Should().Be(MessageResources.ValidationErrorTitle);
    }
    
    [Fact]
    public async Task DeletingTrackingLogByIdIsSuccessful()
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();

        HttpResponseMessage response = await HttpClient.DeleteTrackingLogById(createdTrackingLog.Id);
        ServiceResponse<IReadOnlyCollection<TrackingLogGetResponse>>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<IReadOnlyCollection<TrackingLogGetResponse>>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotContainEquivalentOf(createdTrackingLog);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
}