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
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Shared.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking.TrackingLogEntry;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenMovingTrackingLogEntry(MsSqlTests fixture) : TrackingTestBase(fixture)
{
    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, true, false)]
    [InlineData(false, true, true)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public async Task MovingTrackingLogEntryByIdIsSuccessful(bool shouldMoveByTrackingLog,
                                                             bool shouldMoveByTrackingLogEntryStatus,
                                                             bool shouldMoveByOrderIndex)
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();
        TrackingLogEntryStatusGetResponse createdTrackingLogEntryStatus = await CreateTrackingLogEntryStatusAndValidateResponse(createdTrackingLog.Id);
        TrackingLogEntryStatusGetResponse newDefaultTrackingLogEntryStatus = await CreateTrackingLogEntryStatusAndValidateResponse(DefaultTrackingLog!.Id);
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                            DefaultTrackingLogEntryStatus!.Id);
        decimal newOrderIndex = createdTrackingLogEntry.OrderIndex * Faker.Random.Decimal(0.1m, 0.9m);
        DateTime utcDateTimeBeforeRequest = new DateTimeService().UtcNow;
        TrackingLogEntryMoveRequest request = new()
                                                {
                                                    Id = createdTrackingLogEntry.Id,
                                                    TrackingLogEntryStatusId = shouldMoveByTrackingLogEntryStatus
                                                                                   ? new Optional<int>(shouldMoveByTrackingLog
                                                                                                           ? createdTrackingLogEntryStatus.Id
                                                                                                           : newDefaultTrackingLogEntryStatus.Id)
                                                                                   : new Optional<int>(),
                                                    OrderIndex = shouldMoveByOrderIndex
                                                                     ? new Optional<decimal>(newOrderIndex)
                                                                     : new Optional<decimal>()
                                                };

        HttpResponseMessage response = await HttpClient.MoveTrackingLogEntry(request);
        ServiceResponse<TrackingLogEntryGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Id.Should().Be(createdTrackingLogEntry.Id);
        content.Data.Title.Should().Be(createdTrackingLogEntry.Title);
        content.Data.Description.Should().Be(createdTrackingLogEntry.Description);
        content.Data.CreatedBy.UserName.Should().Be(createdTrackingLogEntry.CreatedBy.UserName);
        content.Data.CreatedAt.Should().Be(createdTrackingLogEntry.CreatedAt);
        content.Data.UpdatedBy.UserName.Should().Be(UserName);
        content.Data.UpdatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.UpdatedAt.Should().NotBe(createdTrackingLogEntry.UpdatedAt);
        content.Data.TrackingLogId.Should().Be(shouldMoveByTrackingLog
                                                   ? createdTrackingLog.Id
                                                   : createdTrackingLogEntry.TrackingLogId);
        content.Data.Status.Id.Should().Be(shouldMoveByTrackingLogEntryStatus
                                               ? shouldMoveByTrackingLog
                                                     ? createdTrackingLogEntryStatus.Id
                                                     : newDefaultTrackingLogEntryStatus.Id
                                               : createdTrackingLogEntry.Status.Id);
        content.Data.OrderIndex.Should().Be(shouldMoveByOrderIndex
                                                ? newOrderIndex
                                                : createdTrackingLogEntry.OrderIndex);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task MovingTrackingLogEntryByIdIsUnsuccessfulIfEveryOptionalAttributeIsEmpty()
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();
        await CreateTrackingLogEntryStatusAndValidateResponse(createdTrackingLog.Id);
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                            DefaultTrackingLogEntryStatus!.Id);
        TrackingLogEntryMoveRequest request = new()
                                                {
                                                    Id = createdTrackingLogEntry.Id,
                                                    TrackingLogEntryStatusId = new Optional<int>(),
                                                    OrderIndex = new Optional<decimal>()
                                                };

        HttpResponseMessage response = await HttpClient.MoveTrackingLogEntry(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Detail.Should().Be(MessageResources.ValidationErrorTitle);
    }
    
    [Fact]
    public async Task MovingTrackingLogEntryByIdIsUnsuccessfulIfTrackingLogEntryDoesNotExist()
    {
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                            DefaultTrackingLogEntryStatus!.Id);
        TrackingLogEntryMoveRequest request = new()
                                              {
                                                  Id = Faker.Random.Int(100),
                                                  TrackingLogEntryStatusId = createdTrackingLogEntry.Status.Id,
                                                  OrderIndex = createdTrackingLogEntry.OrderIndex
                                              };

        HttpResponseMessage response = await HttpClient.MoveTrackingLogEntry(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.NotFound);
        content.Detail.Should().Be(MessageResources.ResourceDoesNotExist);
    }
    
    [Fact]
    public async Task MovingTrackingLogEntryByIdIsUnsuccessfulIfUserCannotEditTargetTrackingLogEntry()
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
        TrackingLogEntryMoveRequest request = new()
                                              {
                                                  Id = createdTrackingLogEntry.TrackingLogId,
                                                  TrackingLogEntryStatusId = createdTrackingLogEntry.Status.Id,
                                                  OrderIndex = createdTrackingLogEntry.OrderIndex
                                              };

        HttpResponseMessage response = await HttpClient.MoveTrackingLogEntry(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(MessageResources.AccessDeniedMessage);
    }
    
    [Fact]
    public async Task MovingTrackingLogEntryByIdIsUnsuccessfulIfUserMovesItToTrackingLogEntryStatusThatDoesNotExist()
    {
        TrackingLogEntryGetResponse createdTrackingLogEntry = 
            await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                            DefaultTrackingLogEntryStatus!.Id);
        TrackingLogEntryMoveRequest request = new()
                                                {
                                                    Id = createdTrackingLogEntry.TrackingLogId,
                                                    TrackingLogEntryStatusId = Faker.Random.Int(100),
                                                    OrderIndex = createdTrackingLogEntry.OrderIndex
                                                };

        HttpResponseMessage response = await HttpClient.MoveTrackingLogEntry(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Detail.Should().Be(MessageResources.ValidationErrorTitle);
    }
    
    [Fact]
    public async Task MovingTrackingLogEntryByIdIsUnsuccessfulIfUserMovesItToTrackingLogThatUserCannotEdit()
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
        TrackingLogEntryMoveRequest request = new()
                                              {
                                                  Id = createdTrackingLogEntry.Id,
                                                  TrackingLogEntryStatusId = DefaultTrackingLogEntryStatus!.Id,
                                                  OrderIndex = createdTrackingLogEntry.OrderIndex
                                              };

        HttpResponseMessage response = await HttpClient.MoveTrackingLogEntry(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(MessageResources.AccessDeniedMessage);
    }
}