using Repository.Requests;
using Repository.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryDetailResponseDto> GetCategoryByIdAsync(Guid categoryId);
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryResponse> UpdateCategoryAsync(UpdateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(Guid categoryId);
        Task<IEnumerable<CategoryResponse>> GetActiveCategoriesAsync();
        Task<IEnumerable<CategoryResponse>> GetSubCategoriesAsync(Guid? parentCategoryId);
    }
}
