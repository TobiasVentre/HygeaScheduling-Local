namespace SchedulingMS.Domain.Utilities;

public static class ArgentinaDateTime
{
    private static readonly Lazy<TimeZoneInfo> ArgentinaTimeZone = new(ResolveTimeZone);

    public static DateTime NormalizeToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), ArgentinaTimeZone.Value)
        };
    }

    private static TimeZoneInfo ResolveTimeZone()
    {
        foreach (var id in new[] { "Argentina Standard Time", "America/Argentina/Buenos_Aires" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.CreateCustomTimeZone(
            "America/Argentina/Buenos_Aires",
            TimeSpan.FromHours(-3),
            "(UTC-03:00) Buenos Aires",
            "(UTC-03:00) Buenos Aires");
    }
}
