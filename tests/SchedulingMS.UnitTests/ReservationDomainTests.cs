using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;
using Xunit;

namespace SchedulingMS.UnitTests;

public class ReservationDomainTests
{
    [Fact]
    public void Constructor_SetsCreatedStatus()
    {
        var reservation = new Reservation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        Assert.Equal(ReservationStatus.Created, reservation.Status);
    }

    [Fact]
    public void ChangeStatus_UpdatesStatus()
    {
        var reservation = new Reservation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        reservation.ChangeStatus(ReservationStatus.Confirmed);
        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
    }
}
