﻿using Events;
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
        /// <param name="useEnglishToExplain">Indicates whether the explanation should be in English.</param>
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
        public async Task<ActionResult<string>> Search(string keyword, string? context, bool useEnglishToExplain = false)
        {
            if (string.IsNullOrEmpty(_accessKey))
            {
                return Unauthorized("Invalid Access Key");
            }

            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Không được để trống từ khóa");
            }

            context = string.IsNullOrEmpty(context) ? "" : context.Trim();
            keyword = keyword.ToLower().Trim();

            var cacheKey = $"Search-{keyword}-{context.ToLower()}-{useEnglishToExplain}";
            if (_cache.TryGetValue(cacheKey, out string cachedResult))
            {
                return Ok(cachedResult);
            }

            if (GeneralHelper.GetTotalWords(keyword) > SearchScope.MaxKeywordTotalWords)
            {
                return BadRequest($"Nội dung tra cứu chỉ chứa tối đa {SearchScope.MaxKeywordTotalWords} từ");
            }

            if (!GeneralHelper.IsJapanese(keyword))
            {
                return BadRequest("Từ khóa cần tra cứu phải là tiếng Nhật");
            }

            if (!string.IsNullOrEmpty(context))
            {
                if (GeneralHelper.GetTotalWords(context) > SearchScope.MaxContextTotalWords)
                {
                    return BadRequest($"Ngữ cảnh chỉ chứa tối đa {SearchScope.MaxContextTotalWords} từ");
                }

                if (!GeneralHelper.IsJapanese(context))
                {
                    return BadRequest("Ngữ cảnh phải là tiếng Nhật");
                }

                if (!context.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
                {
                    return BadRequest("Ngữ cảnh phải chứa từ khóa cần tra");
                }
            }

            try
            {
                // Simulate search logic (replace with actual logic)
                var result = await Task.FromResult($"## Kết quả tra cứu\n- Từ khóa: {keyword}\n- Ngữ cảnh: {context}\n- Giải thích: Đây là kết quả mẫu.");

                _cache.Set(cacheKey, result, TimeSpan.FromHours(1));

                _logger.LogInformation("{_accessKey} searched: {Keyword} - Context: {Context}", _accessKey[..10], keyword, context);
                return Created("Success", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot search for the explanation of '{Keyword}' in the context '{Context}'", keyword, context);
                return Created("Success", "## CẢNH BÁO\n JapanAce đang bận đi pha cà phê nên tạm thời vắng mặt. Bạn yêu vui lòng ngồi chơi 3 phút rồi tra lại thử nha.\nYêu bạn hiền nhiều lắm luôn á!");
            }
        }
    }
}