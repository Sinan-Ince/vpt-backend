using VPT.Api.Models;

namespace VPT.Api.Services;

public class MatchingService : IMatchingService
{
    public bool IsCandidate(
        VehicleSearch search,
        Listing listing)
    {
        // Brand ve Model kesinlikle uyuşmalı; uyuşmuyorsa listing aday bile sayılmaz.
        bool brandMatches = string.Equals(
            search.Brand,
            listing.Brand,
            StringComparison.OrdinalIgnoreCase);

        bool modelMatches = string.Equals(
            search.Model,
            listing.Model,
            StringComparison.OrdinalIgnoreCase);

        // Aranan yıl aralığının dışında kalan ilanlar aday bile sayılmaz.
        bool yearInRange = listing.Year >= search.MinYear && listing.Year <= search.MaxYear;

        // Girilen maksimum kilometreyi aşan ilanlar da aday sayılmaz —
        // MaxMileage belirtilmemişse (null) bu bir kısıtlama getirmiyor.
        bool mileageOk = !search.MaxMileage.HasValue ||
            listing.Mileage <= search.MaxMileage.Value;

        return brandMatches && modelMatches && yearInRange && mileageOk;
    }

    public decimal CalculateMatchScore(
        VehicleSearch search,
        Listing listing)
    {
        if (!IsCandidate(search, listing))
        {
            return 0m;
        }

        // Year ve Mileage zaten IsCandidate'te sert birer filtre (aralık
        // dışı/sınırı aşan ilanlar hiç buraya gelmiyor), bu yüzden Year
        // puanlamaya dahil edilmiyor. Mileage yine de puanlanıyor —
        // aday olmak için sınırı aşmaması yetiyor, ama sınıra ne kadar
        // yakın olduğu (ne kadar azsa o kadar iyi) skoru etkilemeye
        // devam ediyor; Price'ta da aynı sürekli (0-100 kademeli) mantık.
        decimal mileageScore = CalculateProximityScore(listing.Mileage, search.MaxMileage);
        decimal priceScore = CalculateProximityScore(listing.Price, search.MaxPrice);

        // Fiyat, çoğu alıcı için kilometreden daha belirleyici olduğu
        // için eşit ağırlık yerine %70 fiyat / %30 km ağırlıklandırması
        // kullanılıyor.
        const decimal priceWeight = 0.7m;
        const decimal mileageWeight = 0.3m;

        return Math.Round(
            priceScore * priceWeight + mileageScore * mileageWeight,
            2);
    }

    // Bir değerin (km/fiyat), kullanıcının belirlediği üst sınıra göre
    // ne kadar iyi durduğunu 0-100 arası bir puana çeviriyor. Üst sınırı
    // karşılayan (<=max) her değer en az %50'den başlar — "maksimum X"
    // dediğinde X'e tam eşit bir ilan hâlâ kriteri karşılıyor, %0 almaması
    // gerekiyor — ne kadar ucuzsa/azsa %100'e o kadar yaklaşır. Sınırı
    // aşan değerler 0 alır; sınır hiç belirtilmemişse (kullanıcı bir
    // kısıtlama koymamış demektir) kriter tam puan alır.
    private static decimal CalculateProximityScore(decimal actual, decimal? max)
    {
        if (!max.HasValue)
        {
            return 100m;
        }

        if (max.Value <= 0)
        {
            return actual <= 0 ? 100m : 0m;
        }

        if (actual > max.Value)
        {
            return 0m;
        }

        return 50m + (max.Value - actual) / max.Value * 50m;
    }
}