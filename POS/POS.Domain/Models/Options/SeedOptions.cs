namespace POS.Domain.Models.Options;

public class SeedOptions
{
    public bool EnableSeeding { get; set; } = false;
    public bool DeleteDatabaseBeforeSeeding { get; set; } = false;
    public SeedDataCountOptions SeedDataCount { get; set; } = new();
}

public class SeedDataCountOptions
{
    public int Products { get; set; } = 20;
}