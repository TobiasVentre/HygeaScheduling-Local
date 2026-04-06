namespace SchedulingMS.Api.Common;

public sealed record ApiError(
    string Code,
    string Message,
    int Status,
    string TraceId,
    DateTime TimestampUtc);
