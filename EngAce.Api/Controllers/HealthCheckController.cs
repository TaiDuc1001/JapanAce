﻿using Microsoft.AspNetCore.Mvc;

namespace EngAce.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public string HealthCheck()
        {
            return $"It is {DateTime.Now.ToString("hh:MM:ss - dd/MM/yyyy")} now. EngAce still works well.";
        }
    }
}
