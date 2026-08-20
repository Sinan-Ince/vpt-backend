using VPT.Api.DTOs;

namespace VPT.Api.Services;

public interface IVehicleSearchService
{
    Task<VehicleSearchDto> CreateAsync(CreateVehicleSearchDto dto);

    Task<VehicleSearchDto?> GetByIdAsync(int id);

    Task<List<VehicleSearchDto>> GetByUserIdAsync(int userId);

    Task<bool> UpdateAsync(int id, CreateVehicleSearchDto dto);

    Task<bool> DeleteAsync(int id);
}