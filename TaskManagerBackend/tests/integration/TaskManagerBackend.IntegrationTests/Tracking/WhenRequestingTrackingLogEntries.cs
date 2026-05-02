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
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Shared.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenRequestingTrackingLogEntries : TrackingTestBase
{
    public WhenRequestingTrackingLogEntries(MsSqlTests fixture) : base(fixture) { }

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
    
    [Fact]
    public async Task GettingTrackingLogEntriesIsSuccessful()
    {
        await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                        DefaultTrackingLogEntryStatus!.Id);

        HttpResponseMessage response = await HttpClient.GetTrackingLogEntries();
        ServiceResponse<IReadOnlyCollection<TrackingLogEntryGetResponse>>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<IReadOnlyCollection<TrackingLogEntryGetResponse>>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeEmpty();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task UpdatingTrackingLogEntryByIdIsSuccessful()
    {
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                            DefaultTrackingLogEntryStatus!.Id);
        const string Title = "NewLogEntry";
        const string Description = "Test description";
        DateTime utcDateTimeBeforeRequest = new DateTimeService().UtcNow;
        UpdateTrackingLogEntryRequest request = new()
                                                {
                                                    Title = Title,
                                                    Description = Description,
                                                    TrackingLogId = createdTrackingLogEntry.TrackingLogId,
                                                    StatusId = createdTrackingLogEntry.Status.Id,
                                                    OrderIndex = createdTrackingLogEntry.OrderIndex,
                                                    Priority = createdTrackingLogEntry.Priority,
                                                    UpdatedAt = createdTrackingLogEntry.UpdatedAt
                                                };

        HttpResponseMessage response = await HttpClient.UpdateTrackingLogEntry(createdTrackingLogEntry.Id,
                                                                               request);
        ServiceResponse<TrackingLogEntryGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Id.Should().Be(createdTrackingLogEntry.Id);
        content.Data.Title.Should().Be(Title);
        content.Data.Description.Should().Be(Description);
        content.Data.CreatedBy.UserName.Should().Be(createdTrackingLogEntry.CreatedBy.UserName);
        content.Data.CreatedAt.Should().Be(createdTrackingLogEntry.CreatedAt);
        content.Data.UpdatedBy.UserName.Should().Be(UserName);
        content.Data.UpdatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.UpdatedAt.Should().NotBe(createdTrackingLogEntry.UpdatedAt);
        content.Data.TrackingLogId.Should().Be(createdTrackingLogEntry.TrackingLogId);
        content.Data.Status.Should().BeEquivalentTo(createdTrackingLogEntry.Status);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task UpdatingTrackingLogEntryByIdIsUnsuccessfulIfTrackingLogEntryDoesNotExist()
    {
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                            DefaultTrackingLogEntryStatus!.Id);
        const string Title = "NewLogEntry";
        const string Description = "Test description";
        UpdateTrackingLogEntryRequest request = new()
                                                {
                                                    Title = Title,
                                                    Description = Description,
                                                    TrackingLogId = createdTrackingLogEntry.TrackingLogId,
                                                    StatusId = createdTrackingLogEntry.Status.Id,
                                                    OrderIndex = createdTrackingLogEntry.OrderIndex,
                                                    Priority = createdTrackingLogEntry.Priority,
                                                    UpdatedAt = createdTrackingLogEntry.UpdatedAt
                                                };

        HttpResponseMessage response = await HttpClient.UpdateTrackingLogEntry(Faker.Random.Int(100_000_000),
                                                                               request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.NotFound);
        content.Detail.Should().Be(MessageResources.ResourceDoesNotExist);
    }
    
    [Fact]
    public async Task UpdatingTrackingLogEntryByIdIsUnsuccessfulIfUserCannotEditTargetTrackingLogEntry()
    {
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
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
        const string Title = "NewLogEntry";
        const string Description = "Test description";
        UpdateTrackingLogEntryRequest request = new()
                                                {
                                                    Title = Title,
                                                    Description = Description,
                                                    TrackingLogId = createdTrackingLogEntry.TrackingLogId,
                                                    StatusId = createdTrackingLogEntry.Status.Id,
                                                    OrderIndex = createdTrackingLogEntry.OrderIndex,
                                                    Priority = createdTrackingLogEntry.Priority,
                                                    UpdatedAt = createdTrackingLogEntry.UpdatedAt
                                                };

        HttpResponseMessage response = await HttpClient.UpdateTrackingLogEntry(createdTrackingLogEntry.Id,
                                                                               request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(MessageResources.AccessDeniedMessage);
    }
    
    [Fact]
    public async Task UpdatingTrackingLogEntryByIdIsUnsuccessfulIfUserMovesItToTrackingLogThatDoesNotExist()
    {
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                            DefaultTrackingLogEntryStatus!.Id);
        const string Title = "NewLogEntry";
        const string Description = "Test description";
        UpdateTrackingLogEntryRequest request = new()
                                                {
                                                    Title = Title,
                                                    Description = Description,
                                                    TrackingLogId = Faker.Random.Int(100),
                                                    StatusId = createdTrackingLogEntry.Status.Id,
                                                    OrderIndex = createdTrackingLogEntry.OrderIndex,
                                                    Priority = createdTrackingLogEntry.Priority,
                                                    UpdatedAt = createdTrackingLogEntry.UpdatedAt
                                                };

        HttpResponseMessage response = await HttpClient.UpdateTrackingLogEntry(createdTrackingLogEntry.Id,
                                                                               request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Detail.Should().Be(MessageResources.ValidationErrorTitle);
    }
    
    [Fact]
    public async Task UpdatingTrackingLogEntryByIdIsUnsuccessfulIfUserMovesItToTrackingLogThatUserCannotEdit()
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
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();
        TrackingLogEntryStatusGetResponse createdTrackingLogEntryStatus = await CreateTrackingLogEntryStatusAndValidateResponse(createdTrackingLog.Id);
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(createdTrackingLog.Id,
                                                            createdTrackingLogEntryStatus.Id);
        const string Title = "NewLogEntry";
        const string Description = "Test description";
        UpdateTrackingLogEntryRequest request = new()
                                                {
                                                    Title = Title,
                                                    Description = Description,
                                                    TrackingLogId = DefaultTrackingLog!.Id,
                                                    StatusId = createdTrackingLogEntry.Status.Id,
                                                    OrderIndex = createdTrackingLogEntry.OrderIndex,
                                                    Priority = createdTrackingLogEntry.Priority,
                                                    UpdatedAt = createdTrackingLogEntry.UpdatedAt
                                                };

        HttpResponseMessage response = await HttpClient.UpdateTrackingLogEntry(createdTrackingLogEntry.Id,
                                                                               request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(MessageResources.AccessDeniedMessage);
    }
    
    [Fact]
    public async Task DeletingTrackingLogEntryByIdIsSuccessful()
    {
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                            DefaultTrackingLogEntryStatus!.Id);

        HttpResponseMessage response = await HttpClient.DeleteTrackingLogEntryById(createdTrackingLogEntry.Id);
        ServiceResponse<IReadOnlyCollection<TrackingLogEntryGetResponse>>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<IReadOnlyCollection<TrackingLogEntryGetResponse>>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotContainEquivalentOf(createdTrackingLogEntry);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
}