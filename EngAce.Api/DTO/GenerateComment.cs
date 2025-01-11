using Entities.Enums;

namespace JapanAce.Api.DTO
{
    public class GenerateComment
    {
        public string Content { get; set; }
        public JapaneseLevels UserLevel { get; set; }
    }
}
