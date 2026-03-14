using PulseWatch.Application.DTOs.ApiEndpoint;

namespace PulseWatch.Application.Interfaces;

public interface IApiEndpointService
{
    Task<List<ApiEndpointResponse>> GetAllAsync(Guid userId);
    Task<ApiEndpointResponse?> GetByIdAsync(Guid id, Guid userId);
    Task<ApiEndpointResponse> CreateAsync(CreateApiRequest request, Guid userId);
    Task<ApiEndpointResponse?> UpdateAsync(Guid id, UpdateApiRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
