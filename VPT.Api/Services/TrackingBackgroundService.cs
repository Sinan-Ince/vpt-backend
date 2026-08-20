using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VPT.Api.Data;
using VPT.Api.Models;

namespace VPT.Api.Services;

// Aktif (IsActive) VehicleTracking kayıtlarını periyodik olarak tarar:
// yeni mock ilan çeker, takip edilen aramalarla eşleştirir, yeni bir
// eşleşme bulursa VehicleMatch + Notification oluşturur.
public class TrackingBackgroundService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(2);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TrackingBackgroundService> _logger;

    public TrackingBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<TrackingBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckDueTrackingsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Canlı takip kontrolü sırasında hata oluştu.");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }

    private async Task CheckDueTrackingsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var listingSource = scope.ServiceProvider.GetRequiredService<IListingSource>();
        var matchingService = scope.ServiceProvider.GetRequiredService<IMatchingService>();

        var now = DateTime.UtcNow;

        var dueTrackings = await context.VehicleTrackings
            .Include(t => t.VehicleSearch)
            .Where(t => t.IsActive && (t.NextCheckAt == null || t.NextCheckAt <= now))
            .ToListAsync(cancellationToken);

        if (dueTrackings.Count == 0)
        {
            return;
        }

        var newListings = await listingSource.FetchListingsAsync();

        context.Listings.AddRange(newListings);
        await context.SaveChangesAsync(cancellationToken);

        foreach (var tracking in dueTrackings)
        {
            foreach (var listing in newListings)
            {
                if (!matchingService.IsCandidate(tracking.VehicleSearch, listing))
                {
                    continue;
                }

                var score = matchingService.CalculateMatchScore(tracking.VehicleSearch, listing);

                context.VehicleMatches.Add(new VehicleMatch
                {
                    VehicleSearchId = tracking.VehicleSearchId,
                    ListingId = listing.Id,
                    MatchScore = score,
                    MatchedAt = now
                });

                context.Notifications.Add(new Notification
                {
                    VehicleTrackingId = tracking.Id,
                    ListingId = listing.Id,
                    Message = $"Yeni eşleşme: {listing.Brand} {listing.Model} ({listing.Year}) - %{score}",
                    CreatedAt = now
                });
            }

            tracking.LastCheckedAt = now;
            tracking.NextCheckAt = now.Add(CheckInterval);
        }

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "{Count} aktif takip kontrol edildi, {ListingCount} yeni ilan tarandı.",
            dueTrackings.Count,
            newListings.Count);
    }
}
