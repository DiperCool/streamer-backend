namespace Shared.Seeds;

public interface IDataSeeder
{
    Task SeedAllAsync();
    int Order { get; }
}
