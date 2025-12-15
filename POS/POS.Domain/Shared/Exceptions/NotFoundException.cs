namespace POS.Domain.Shared.Exceptions;

public class NotFoundException(string message) : Exception(message);