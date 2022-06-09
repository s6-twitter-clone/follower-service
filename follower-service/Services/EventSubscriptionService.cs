using follower_service.Interfaces;
using follower_service.Models.Events;

namespace follower_service.Services;

public class EventSubscriptionService : BackgroundService
{
    private readonly IServiceProvider services;
    private readonly IEventService eventService;

    public EventSubscriptionService(IServiceProvider services, IEventService eventService)
    {
        this.services = services;
        this.eventService = eventService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        eventService.subscribe<AddUserEvent>(
            exchange: "user-exchange", queue: "user-added-follower", topic: "user-added", onUserAdded
        );

        eventService.subscribe<UpdateUserEvent>(
            exchange: "user-exchange", queue: "user-updated-follower", topic: "user-updated", onUserUpdated
        );

        return Task.CompletedTask;
    }

    private void onUserAdded(AddUserEvent user)
    {
        using var scope = services.CreateScope();

        var userService =
            scope.ServiceProvider
                .GetRequiredService<UserService>();

        userService.Add(user.Id, user.DisplayName);
    }

    private void onUserUpdated(UpdateUserEvent user)
    {
        using var scope = services.CreateScope();

        var userService =
            scope.ServiceProvider
                .GetRequiredService<UserService>();

        userService.Update(user.Id, user.DisplayName);
    }
}
