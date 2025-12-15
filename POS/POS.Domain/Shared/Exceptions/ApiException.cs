namespace POS.Domain.Shared.Exceptions;

public class ApiException(string message) : Exception(message);