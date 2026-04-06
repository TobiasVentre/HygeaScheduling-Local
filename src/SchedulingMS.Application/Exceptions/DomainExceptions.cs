namespace SchedulingMS.Application.Exceptions;

public class ValidationException(string message) : Exception(message);
public class ConflictException(string message) : Exception(message);
public class NotFoundException(string message) : Exception(message);


