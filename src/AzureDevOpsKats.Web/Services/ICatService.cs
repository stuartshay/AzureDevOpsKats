using AzureDevOpsKats.Web.Data;

namespace AzureDevOpsKats.Web.Services;

public interface ICatService
{
    Task<List<Cat>> GetAllAsync();

    Task<Cat?> GetByIdAsync(long id);

    Task<Cat> CreateAsync(string name, string description, IFormFile? photo);

    Task<Cat?> UpdateAsync(long id, string name, string description, IFormFile? photo);

    Task<bool> DeleteAsync(long id);
}
