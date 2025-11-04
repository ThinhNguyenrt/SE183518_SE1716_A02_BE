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
    public class NewsArticleService : INewsArticleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsArticleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NewsArticleResponse>> GetAllNewsArticlesAsync()
        {
            var newsArticles = await _unitOfWork.Repository<NewsArticle>().GetAllAsync();

            return await MapToResponseDtoListAsync(newsArticles);
        }

        public async Task<NewsArticleResponse> GetNewsArticleByIdAsync(Guid newsArticleId)
        {
            var newsArticle = await _unitOfWork.Repository<NewsArticle>().GetByIdAsync(newsArticleId);

            if (newsArticle == null)
                return null;

            return await MapToResponseDtoAsync(newsArticle);
        }

        public async Task<NewsArticleResponse> CreateNewsArticleAsync(NewsArticleRequest request, Guid createdById)
        {
            // Validate category exists
            var categoryExists = await _unitOfWork.Repository<Category>()
                .ExistsAsync(c => c.CategoryId == request.CategoryId);

            if (!categoryExists)
                throw new ArgumentException("Category does not exist");

            // Validate creator exists
            var creatorExists = await _unitOfWork.Repository<SystemAccount>()
                .ExistsAsync(u => u.AccountId == createdById);

            if (!creatorExists)
                throw new ArgumentException("Creator account does not exist");

            var newsArticle = new NewsArticle
            {
                NewsArticleId = Guid.NewGuid(),
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                NewsStatus = request.NewsStatus,
                CreatedById = createdById,
                CreatedDate = DateTime.Now
            };

            // Handle tags
            if (request.TagIds != null && request.TagIds.Any())
            {
                var tags = await _unitOfWork.Repository<Tag>()
                    .FindAsync(t => request.TagIds.Contains(t.TagId));

                newsArticle.Tags = tags.ToList();
            }

            await _unitOfWork.Repository<NewsArticle>().AddAsync(newsArticle);
            await _unitOfWork.SaveChangesAsync();

            return await MapToResponseDtoAsync(newsArticle);
        }

        public async Task<NewsArticleResponse> UpdateNewsArticleAsync(UpdateNewsArticleRequest request, Guid updatedById)
        {
            var newsArticle = await _unitOfWork.Repository<NewsArticle>().GetByIdAsync(request.NewsArticleId);

            if (newsArticle == null)
                throw new KeyNotFoundException("News article not found");

            // Validate category exists
            var categoryExists = await _unitOfWork.Repository<Category>()
                .ExistsAsync(c => c.CategoryId == request.CategoryId);

            if (!categoryExists)
                throw new ArgumentException("Category does not exist");

            // Validate updater exists
            var updaterExists = await _unitOfWork.Repository<SystemAccount>()
                .ExistsAsync(u => u.AccountId == updatedById);

            if (!updaterExists)
                throw new ArgumentException("Updater account does not exist");

            newsArticle.NewsTitle = request.NewsTitle;
            newsArticle.Headline = request.Headline;
            newsArticle.NewsContent = request.NewsContent;
            newsArticle.NewsSource = request.NewsSource;
            newsArticle.CategoryId = request.CategoryId;
            newsArticle.NewsStatus = request.NewsStatus;
            newsArticle.UpdatedById = updatedById;
            newsArticle.ModifiedDate = DateTime.Now;

            // Update tags
            if (request.TagIds != null)
            {
                var tags = await _unitOfWork.Repository<Tag>()
                    .FindAsync(t => request.TagIds.Contains(t.TagId));

                newsArticle.Tags = tags.ToList();
            }

            _unitOfWork.Repository<NewsArticle>().Update(newsArticle);
            await _unitOfWork.SaveChangesAsync();

            return await MapToResponseDtoAsync(newsArticle);
        }

        public async Task<bool> DeleteNewsArticleAsync(Guid newsArticleId)
        {
            var newsArticle = await _unitOfWork.Repository<NewsArticle>().GetByIdAsync(newsArticleId);

            if (newsArticle == null)
                throw new KeyNotFoundException("News article not found");

            _unitOfWork.Repository<NewsArticle>().Delete(newsArticle);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<NewsArticleResponse>> GetNewsArticlesByCategoryAsync(Guid categoryId)
        {
            var newsArticles = await _unitOfWork.Repository<NewsArticle>()
                .FindAsync(n => n.CategoryId == categoryId);

            return await MapToResponseDtoListAsync(newsArticles);
        }

        public async Task<IEnumerable<NewsArticleResponse>> GetNewsArticlesByTagAsync(Guid tagId)
        {
            var newsArticles = await _unitOfWork.Repository<NewsArticle>().GetAllAsync();
            var filtered = newsArticles.Where(n => n.Tags.Any(t => t.TagId == tagId));

            return await MapToResponseDtoListAsync(filtered);
        }

        public async Task<IEnumerable<NewsArticleResponse>> GetActiveNewsArticlesAsync()
        {
            var newsArticles = await _unitOfWork.Repository<NewsArticle>()
                .FindAsync(n => n.NewsStatus);

            return await MapToResponseDtoListAsync(newsArticles);
        }

        private async Task<NewsArticleResponse> MapToResponseDtoAsync(NewsArticle newsArticle)
        {
            var category = await _unitOfWork.Repository<Category>()
                .GetByIdAsync(newsArticle.CategoryId);

            var creator = await _unitOfWork.Repository<SystemAccount>()
                .GetByIdAsync(newsArticle.CreatedById);

            SystemAccount updater = null;
            if (newsArticle.UpdatedById.HasValue)
            {
                updater = await _unitOfWork.Repository<SystemAccount>()
                    .GetByIdAsync(newsArticle.UpdatedById.Value);
            }

            return new NewsArticleResponse
            {
                NewsArticleId = newsArticle.NewsArticleId,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                CreatedDate = newsArticle.CreatedDate,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                CategoryId = newsArticle.CategoryId,
                CategoryName = category?.CategoryName,
                NewsStatus = newsArticle.NewsStatus,
                CreatedById = newsArticle.CreatedById,
                CreatedByName = creator?.AccountName,
                UpdatedById = newsArticle.UpdatedById,
                UpdatedByName = updater?.AccountName,
                ModifiedDate = newsArticle.ModifiedDate,
                Tags = newsArticle.Tags?.Select(t => new TagResponseDto
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                    Note = t.Note
                }).ToList() ?? new List<TagResponseDto>()
            };
        }

        private async Task<List<NewsArticleResponse>> MapToResponseDtoListAsync(IEnumerable<NewsArticle> newsArticles)
        {
            var result = new List<NewsArticleResponse>();

            foreach (var article in newsArticles)
            {
                result.Add(await MapToResponseDtoAsync(article));
            }

            return result;
        }
    }
}
