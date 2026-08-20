using VPT.Api.Models;

namespace VPT.Api.Services;

public class MockListingSource : IListingSource
{
    private static readonly (string Brand, string Model)[] Catalog =
    {
        ("Toyota", "Corolla"),
        ("Volkswagen", "Golf"),
        ("Renault", "Clio"),
        ("Fiat", "Egea"),
        ("Ford", "Focus"),
        ("Honda", "Civic"),
        ("Hyundai", "i20"),
        ("Opel", "Astra"),
    };

    private readonly Random _random = new();

    public Task<List<Listing>> FetchListingsAsync()
    {
        var count = _random.Next(5, 11);

        var listings = Enumerable.Range(0, count)
            .Select(_ =>
            {
                var (brand, model) = Catalog[_random.Next(Catalog.Length)];

                return new Listing
                {
                    Brand = brand,
                    Model = model,
                    Year = _random.Next(2010, 2025),
                    Mileage = _random.Next(0, 250_000),
                    Price = _random.Next(150_000, 1_500_000),
                    Url = $"https://mock-source.local/listing/{Guid.NewGuid()}"
                };
            })
            .ToList();

        return Task.FromResult(listings);
    }
}
