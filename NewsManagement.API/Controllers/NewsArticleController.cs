using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Requests;
using Repository.Responses;
using Service.Interface;

namespace NewsManagement.API.Controllers
{
    [Route("api/newsarticle")]
    [ApiController]
    [Authorize(Policy = "StaffOnly")]
    public class NewsArticleController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticleController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var articles = await _newsArticleService.GetAllNewsArticlesAsync();
                return Ok(ApiResponse<IEnumerable<NewsArticleResponse>>.Success(articles, "Fetched all articles successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var article = await _newsArticleService.GetNewsArticleByIdAsync(id);
                if (article == null)
                    return NotFound(ApiResponse<string>.Fail("News article not found", 404));

                return Ok(ApiResponse<NewsArticleResponse>.Success(article, "Fetched article successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewsArticle([FromBody] NewsArticleRequest request)
        {
            try
            {
                var userId = User.FindFirst("AccountId")?.Value;
                if (userId == null)
                    return Unauthorized(ApiResponse<string>.Fail("Missing AccountId in token", 401));

                var createdById = Guid.Parse(userId);
                var result = await _newsArticleService.CreateNewsArticleAsync(request, createdById);

                return Ok(ApiResponse<NewsArticleResponse>.Success(result, "News article created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateNewsArticleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.Fail("Invalid model state", 400));

                var userId = User.FindFirst("AccountId")?.Value;
                if (userId == null)
                    return Unauthorized(ApiResponse<string>.Fail("Missing AccountId in token", 401));

                var currentUserId = Guid.Parse(userId);
                var updated = await _newsArticleService.UpdateNewsArticleAsync(request, currentUserId);

                return Ok(ApiResponse<NewsArticleResponse>.Success(updated, "News article updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message, 404));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _newsArticleService.DeleteNewsArticleAsync(id);
                return Ok(ApiResponse<string>.Success("Article deleted successfully", "Success", 200));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message, 404));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(Guid categoryId)
        {
            try
            {
                var articles = await _newsArticleService.GetNewsArticlesByCategoryAsync(categoryId);
                return Ok(ApiResponse<IEnumerable<NewsArticleResponse>>.Success(articles, "Fetched articles by category"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        [HttpGet("tag/{tagId}")]
        public async Task<IActionResult> GetByTag(Guid tagId)
        {
            try
            {
                var articles = await _newsArticleService.GetNewsArticlesByTagAsync(tagId);
                return Ok(ApiResponse<IEnumerable<NewsArticleResponse>>.Success(articles, "Fetched articles by tag"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var articles = await _newsArticleService.GetActiveNewsArticlesAsync();
                return Ok(ApiResponse<IEnumerable<NewsArticleResponse>>.Success(articles, "Fetched active articles"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }
    }
}
