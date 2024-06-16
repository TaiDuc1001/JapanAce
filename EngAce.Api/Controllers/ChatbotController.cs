﻿using Entities;
using Functions;
using Helper;
using Microsoft.AspNetCore.Mvc;

namespace EngAce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ILogger<DictionaryController> _logger;
        private readonly string _apiKey;
        public ChatbotController(ILogger<DictionaryController> logger)
        {
            _logger = logger;
            _apiKey = HttpContextHelper.GetAccessKey();
        }

        [HttpPost("GenerateAnswer")]
        public async Task<ActionResult<string>> GenerateAnswer([FromBody] Chat request)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                return Unauthorized("Missing Gemini API Key or Access Token");
            }

            if (request == null)
            {
                return BadRequest("Invalid Request");
            }

            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("The question must not be empty");
            }

            return Ok(ChatbotScope.GenerateAnswer(_apiKey, request));
        }
    }
}