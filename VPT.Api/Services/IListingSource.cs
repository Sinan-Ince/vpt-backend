using VPT.Api.Models;

namespace VPT.Api.Services;

public interface IListingSource
{
    Task<List<Listing>> FetchListingsAsync();
}
