using Repository.Requests;
using Repository.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface INewsArticleService
    {
        Task<IEnumerable<NewsArticleResponse>> GetAllNewsArticlesAsync();
        Task<NewsArticleResponse> GetNewsArticleByIdAsync(Guid newsArticleId);
        Task<NewsArticleResponse> CreateNewsArticleAsync(NewsArticleRequest request, Guid createdById);
        Task<NewsArticleResponse> UpdateNewsArticleAsync(UpdateNewsArticleRequest request, Guid updatedById);
        Task<bool> DeleteNewsArticleAsync(Guid newsArticleId);
        Task<IEnumerable<NewsArticleResponse>> GetNewsArticlesByCategoryAsync(Guid categoryId);
        Task<IEnumerable<NewsArticleResponse>> GetActiveNewsArticlesAsync();
        Task<IEnumerable<NewsArticleResponse>> GetNewsArticlesByTagAsync(Guid tagId);
    }
}
