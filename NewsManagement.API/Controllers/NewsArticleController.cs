using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Requests;
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
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var article = await _newsArticleService.GetNewsArticleByIdAsync(id);
                if (article == null)
                    return NotFound(new { message = "News article not found" });

                return Ok(article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewsArticleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get current user ID from authentication context
                var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());

                var article = await _newsArticleService.CreateNewsArticleAsync(request, currentUserId);
                return CreatedAtAction(nameof(GetById), new { id = article.NewsArticleId }, article);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateNewsArticleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get current user ID from authentication context
                var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());

                var article = await _newsArticleService.UpdateNewsArticleAsync(request, currentUserId);
                return Ok(article);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _newsArticleService.DeleteNewsArticleAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(Guid categoryId)
        {
            try
            {
                var articles = await _newsArticleService.GetNewsArticlesByCategoryAsync(categoryId);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("tag/{tagId}")]
        public async Task<IActionResult> GetByTag(Guid tagId)
        {
            try
            {
                var articles = await _newsArticleService.GetNewsArticlesByTagAsync(tagId);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var articles = await _newsArticleService.GetActiveNewsArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
