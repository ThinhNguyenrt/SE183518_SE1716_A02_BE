using Repository.Models;
using Repository.Repository.Interface;
using Repository.Requests;
using Repository.Responses;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

            return categories.Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDescription = c.CategoryDescription,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive,
                NewsArticleCount = c.NewsArticles?.Count ?? 0
            }).ToList();
        }

        public async Task<CategoryDetailResponseDto> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId);

            if (category == null)
                return null;

            var allCategories = await _unitOfWork.Repository<Category>().GetAllAsync();
            var subCategories = allCategories.Where(c => c.ParentCategoryId == categoryId);
            var parentCategory = category.ParentCategoryId.HasValue
                ? allCategories.FirstOrDefault(c => c.CategoryId == category.ParentCategoryId.Value)
                : null;

            return new CategoryDetailResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = parentCategory?.CategoryName,
                IsActive = category.IsActive,
                NewsArticleCount = category.NewsArticles?.Count ?? 0,
                SubCategories = subCategories.Select(sc => new CategoryResponse
                {
                    CategoryId = sc.CategoryId,
                    CategoryName = sc.CategoryName,
                    CategoryDescription = sc.CategoryDescription,
                    ParentCategoryId = sc.ParentCategoryId,
                    IsActive = sc.IsActive,
                    NewsArticleCount = sc.NewsArticles?.Count ?? 0
                }).ToList()
            };
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            // Validate parent category if provided
            if (request.ParentCategoryId.HasValue)
            {
                var parentExists = await _unitOfWork.Repository<Category>()
                    .ExistsAsync(c => c.CategoryId == request.ParentCategoryId.Value);

                if (!parentExists)
                    throw new ArgumentException("Parent category does not exist");
            }

            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = request.CategoryName,
                CategoryDescription = request.CategoryDescription,
                ParentCategoryId = request.ParentCategoryId,
                IsActive = request.IsActive
            };

            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription,
                ParentCategoryId = category.ParentCategoryId,
                IsActive = category.IsActive,
                NewsArticleCount = 0
            };
        }

        public async Task<CategoryResponse> UpdateCategoryAsync(UpdateCategoryRequest request)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.CategoryId);

            if (category == null)
                throw new KeyNotFoundException("Category not found");

            // Validate parent category if provided
            if (request.ParentCategoryId.HasValue)
            {
                // Prevent self-reference
                if (request.ParentCategoryId.Value == request.CategoryId)
                    throw new ArgumentException("Category cannot be its own parent");

                var parentExists = await _unitOfWork.Repository<Category>()
                    .ExistsAsync(c => c.CategoryId == request.ParentCategoryId.Value);

                if (!parentExists)
                    throw new ArgumentException("Parent category does not exist");
            }

            category.CategoryName = request.CategoryName;
            category.CategoryDescription = request.CategoryDescription;
            category.ParentCategoryId = request.ParentCategoryId;
            category.IsActive = request.IsActive;

            _unitOfWork.Repository<Category>().Update(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription,
                ParentCategoryId = category.ParentCategoryId,
                IsActive = category.IsActive,
                NewsArticleCount = category.NewsArticles?.Count ?? 0
            };
        }

        public async Task<bool> DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId);

            if (category == null)
                throw new KeyNotFoundException("Category not found");

            // Check if category has any news articles
            var hasNewsArticles = await _unitOfWork.Repository<NewsArticle>()
                .ExistsAsync(n => n.CategoryId == categoryId);

            if (hasNewsArticles)
                throw new InvalidOperationException("Cannot delete category that has associated news articles");

            // Check if category has sub-categories
            var hasSubCategories = await _unitOfWork.Repository<Category>()
                .ExistsAsync(c => c.ParentCategoryId == categoryId);

            if (hasSubCategories)
                throw new InvalidOperationException("Cannot delete category that has sub-categories");

            _unitOfWork.Repository<Category>().Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CategoryResponse>> GetActiveCategoriesAsync()
        {
            var categories = await _unitOfWork.Repository<Category>()
                .FindAsync(c => c.IsActive);

            return categories.Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDescription = c.CategoryDescription,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive,
                NewsArticleCount = c.NewsArticles?.Count ?? 0
            }).ToList();
        }

        public async Task<IEnumerable<CategoryResponse>> GetSubCategoriesAsync(Guid? parentCategoryId)
        {
            var categories = await _unitOfWork.Repository<Category>()
                .FindAsync(c => c.ParentCategoryId == parentCategoryId);

            return categories.Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDescription = c.CategoryDescription,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive,
                NewsArticleCount = c.NewsArticles?.Count ?? 0
            }).ToList();
        }
    }
}
