using Microsoft.EntityFrameworkCore;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;
using SchedulingMS.Infrastructure.Persistence;
using SchedulingMS.Infrastructure.Queries.Reservations;
using Xunit;

namespace SchedulingMS.UnitTests;

public class FinalizedReservationClosureQueryTests
{
    [Fact]
    public async Task GetFinalizedPendingClosureAsync_UsesFinalizedTransitionTimestamp()
    {
        await using var dbContext = CreateDbContext();
        var thresholdUtc = DateTime.UtcNow.AddHours(-24);
        var finalizedAtUtc = DateTime.UtcNow.AddHours(-25);

        var reservation = new Reservation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2));

        reservation.ChangeStatus(ReservationStatus.Finalized);

        dbContext.Reservations.Add(reservation);
        dbContext.ReservationStatusHistory.Add(new ReservationStatusHistory(
            reservation.Id,
            ReservationStatus.InProgress,
            ReservationStatus.Finalized,
            finalizedAtUtc,
            null,
            "Finalized for retention test."));

        await dbContext.SaveChangesAsync();

        var query = new ReservationQuery(dbContext);
        var results = await query.GetFinalizedPendingClosureAsync(thresholdUtc, 20);

        Assert.Contains(results, candidate => candidate.Id == reservation.Id);
    }

    private static SchedulingDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SchedulingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new SchedulingDbContext(options);
    }
}
