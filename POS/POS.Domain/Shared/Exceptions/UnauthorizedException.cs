namespace POS.Domain.Shared.Exceptions;

public class UnauthorizedException(string message) : Exception(message);