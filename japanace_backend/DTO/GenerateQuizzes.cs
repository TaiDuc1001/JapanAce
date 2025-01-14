﻿using Entities.Enums;

namespace JapanAce.Api.DTO
{
    public class GenerateQuizzes
    {
        public required string Topic { get; set; }
        public required List<QuizzType> QuizzTypes { get; set; }
    }
}
