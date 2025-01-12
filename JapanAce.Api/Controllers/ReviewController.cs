using Entities;
using Entities.Enums;
using Events;
using Helper;
using Microsoft.AspNetCore.Mvc;

namespace JapanAce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController() : ControllerBase
    {
        private readonly string _accessKey = HttpContextHelper.GetAccessKey();

        /// <summary>
        /// Generates the review based on the provided content and Japanese level
        /// </summary>
        /// <param name="content">The content for which to generate a comment.</param>
        /// <param name="JapaneseLevels">The Japanese level for the generated comment. Default is N3 (Intermediate).</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the generated comment if the operation is successful,
        /// or an error response if the access key is invalid or an exception occurs during generation.
        /// </returns>
        /// <response code="200">The generated comment.</response>
        /// <response code="400">The error message if an error occurs during generation.</response>
        /// <response code="401">The error message if the access key is invalid.</response>
        [HttpPost("Generate")]
        [ResponseCache(Duration = ReviewScope.OneHourAsCachingAge, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<ActionResult<Comment>> Generate([FromBody] string content, JapaneseLevels JapaneseLevels = JapaneseLevels.N3_Intermediate)
        {
            if (string.IsNullOrEmpty(_accessKey))
            {
                return Unauthorized("Invalid Access Key");
            }
            content = content.Trim();

            if (!GeneralHelper.IsJapanese(content))
            {
                return BadRequest("Nội dung phải là tiếng Nhật");
            }

            if (GeneralHelper.GetTotalWords(content) < ReviewScope.MinTotalWords)
            {
                return BadRequest($"Bài viết phải dài tối thiểu {ReviewScope.MinTotalWords} từ.");
            }

            if (GeneralHelper.GetTotalWords(content) > ReviewScope.MaxTotalWords)
            {
                return BadRequest($"Bài viết không được dài hơn {ReviewScope.MaxTotalWords} từ.");
            }

            try
            {
                var result = await ReviewScope.GenerateReview(_accessKey, JapaneseLevels, content);
                return Ok(result);
            }
            catch
            {
                return Created("Success", "## CẢNH BÁO\n JapanAce đang bận đi pha trà nên tạm thời vắng mặt. Bé yêu vui lòng ngồi chơi 3 phút rồi gửi lại cho JapanAce nhận xét nha.\nYêu bé yêu nhiều lắm luôn á!");
            }
        }
    }
}