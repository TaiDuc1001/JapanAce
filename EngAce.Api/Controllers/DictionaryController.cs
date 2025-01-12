using Events;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace JapanAce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController(IMemoryCache cache, ILogger<DictionaryController> logger) : ControllerBase
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ILogger<DictionaryController> _logger = logger;
        private readonly string _accessKey = HttpContextHelper.GetAccessKey();

        /// <summary>
        /// Searches for a given keyword within an optional context
        /// </summary>
        /// <param name="keyword">The keyword to search for (must be in Japanese).</param>
        /// <param name="context">The optional context for the search (must be in Japanese, contain the keyword, and have less than 100 words).</param>
        /// <param name="useJapaneseToExplain">Indicates whether the explanation should be in Japanese.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the search result as a string if the operation is successful,
        /// or an error response if validation fails or an exception occurs during the search.
        /// </returns>
        /// <response code="200">The search result from the cache if available.</response>
        /// <response code="201">The search result after performing the search successfully.</response>
        /// <response code="400">The error message if the input validation fails or if an error occurs during the search.</response>
        /// <response code="401">Invalid Access Key</response>
        [HttpGet("Search")]
        [ResponseCache(Duration = QuizScope.ThreeDaysAsCachingAge, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<ActionResult<string>> Search(string keyword, string? context, bool useJapaneseToExplain = false)
        {
            // Log the start of the search
            _logger.LogInformation("Starting search for keyword: {Keyword}", keyword);

            // Validate access key
            if (string.IsNullOrEmpty(_accessKey))
            {
                _logger.LogWarning("Invalid or missing access key.");
                return Unauthorized("Invalid Access Key");
            }

            // Validate keyword
            if (string.IsNullOrEmpty(keyword))
            {
                _logger.LogWarning("Keyword is empty or null.");
                return BadRequest("Không được để trống từ khóa");
            }

            context = string.IsNullOrEmpty(context) ? "" : context.Trim();
            keyword = keyword.ToLower().Trim();

            // Generate cache key
            var cacheKey = $"Search-{keyword}-{context.ToLower()}-{useJapaneseToExplain}";
            _logger.LogInformation("Cache key generated: {CacheKey}", cacheKey);

            // Check cache for existing result
            if (_cache.TryGetValue(cacheKey, out string cachedResult))
            {
                _logger.LogInformation("Returning cached result for keyword: {Keyword}", keyword);
                return Ok(cachedResult);
            }

            // Validate keyword length
            // if (GeneralHelper.GetTotalWords(keyword) > SearchScope.MaxKeywordTotalWords)
            // {
            //     _logger.LogWarning("Keyword exceeds maximum allowed words: {Keyword}", keyword);
            //     return BadRequest($"Nội dung tra cứu chỉ chứa tối đa {SearchScope.MaxKeywordTotalWords} từ");
            // }

            // Validate Japanese input for keyword
            if (!GeneralHelper.IsJapanese(keyword))
            {
                _logger.LogWarning("Keyword is not in Japanese: {Keyword}", keyword);
                return BadRequest("Từ khóa cần tra cứu phải là tiếng Nhật");
            }

            // Validate context
            if (!string.IsNullOrEmpty(context))
            {
                // Validate context length
                if (GeneralHelper.GetTotalWords(context) > SearchScope.MaxContextTotalWords)
                {
                    _logger.LogWarning("Context exceeds maximum allowed words: {Context}", context);
                    return BadRequest($"Ngữ cảnh chỉ chứa tối đa {SearchScope.MaxContextTotalWords} từ");
                }

                // Validate Japanese input for context
                if (!GeneralHelper.IsJapanese(context))
                {
                    _logger.LogWarning("Context is not in Japanese: {Context}", context);
                    return BadRequest("Ngữ cảnh phải là tiếng Nhật");
                }

                // Validate that context contains the keyword
                if (!context.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
                {
                    _logger.LogWarning("Context does not contain the keyword: {Keyword}", keyword);
                    return BadRequest("Ngữ cảnh phải chứa từ khóa cần tra");
                }
            }

            try
            {
                // Call the SearchScope.Search method
                _logger.LogInformation("Calling SearchScope.Search for keyword: {Keyword}", keyword);
                var result = await SearchScope.Search(_accessKey, useJapaneseToExplain, keyword, context);

                // Validate the result
                if (string.IsNullOrEmpty(result))
                {
                    _logger.LogWarning("SearchScope.Search returned an empty result for keyword: {Keyword}", keyword);
                    return BadRequest("Không thể tìm thấy kết quả phù hợp");
                }

                // Cache the result
                _cache.Set(cacheKey, result, TimeSpan.FromHours(1));
                _logger.LogInformation("Result cached successfully for keyword: {Keyword}", keyword);

                // Return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error occurred while searching for keyword: {Keyword}", keyword);

                // Return a friendly error message
                return Ok("## CẢNH BÁO\n JapanAce đang bận đi pha cà phê nên tạm thời vắng mặt. Bạn yêu vui lòng ngồi chơi 3 phút rồi tra lại thử nha.\nYêu bạn hiền nhiều lắm luôn á!");
            }
        }
    }
}