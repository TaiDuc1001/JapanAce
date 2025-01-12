using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Gemini;
using Helper;


namespace Events
{
    public static class SearchScope
    {
        public const sbyte MaxKeywordTotalWords = 7;
        public const sbyte MaxContextTotalWords = 15;

        // Logger for debugging and error tracking
        private static readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("SearchScope");

        public static async Task<string> Search(string apiKey, bool useJapanese, string keyword, string context)
        {
            // Validate API key
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("API key is missing or invalid.");
                return "Không thể tìm thấy kết quả phù hợp";
            }

            // Validate keyword
            if (string.IsNullOrEmpty(keyword))
            {
                _logger.LogWarning("Keyword is empty or null.");
                return "Không được để trống từ khóa";
            }

            keyword = keyword.Trim();
            context = context?.Trim() ?? string.Empty;

            // Validate keyword length
            // if (GeneralHelper.GetTotalWords(keyword) > MaxKeywordTotalWords)
            // {
            //     _logger.LogWarning("Keyword exceeds maximum allowed words: {Keyword}", keyword);
            //     return $"Nội dung tra cứu chỉ chứa tối đa {MaxKeywordTotalWords} từ";
            // }

            // Validate context length
            // if (!string.IsNullOrEmpty(context) && GeneralHelper.GetTotalWords(context) > MaxContextTotalWords)
            // {
            //     _logger.LogWarning("Context exceeds maximum allowed words: {Context}", context);
            //     return $"Ngữ cảnh chỉ chứa tối đa {MaxContextTotalWords} từ";
            // }

            // Validate Japanese input
            if (!GeneralHelper.IsJapanese(keyword))
            {
                _logger.LogWarning("Keyword is not in Japanese: {Keyword}", keyword);
                return "Từ khóa cần tra cứu phải là tiếng Nhật";
            }

            if (!string.IsNullOrEmpty(context) && !GeneralHelper.IsJapanese(context))
            {
                _logger.LogWarning("Context is not in Japanese: {Context}", context);
                return "Ngữ cảnh phải là tiếng Nhật";
            }

            if (!string.IsNullOrEmpty(context) && !context.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
            {
                _logger.LogWarning("Context does not contain the keyword: {Keyword}", keyword);
                return "Ngữ cảnh phải chứa từ khóa cần tra";
            }

            // Build the prompt
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("## Keyword:");
            promptBuilder.AppendLine($"- {keyword}");
            if (!string.IsNullOrEmpty(context))
            {
                promptBuilder.AppendLine("## Context of the Keyword:");
                promptBuilder.AppendLine($"- {context}");
            }

            // Log the prompt for debugging
            _logger.LogInformation("Generated Prompt: {Prompt}", promptBuilder.ToString());

            // Generate the response using the Gemini API
            try
            {
                var response = await Gemini.Generator.GenerateContent(
                    apiKey,
                    useJapanese ? instructionForJapanese : instructionForVietnamese,
                    promptBuilder.ToString().Trim(),
                    false,
                    50
                );

                if (string.IsNullOrEmpty(response))
                {
                    _logger.LogWarning("Gemini API returned an empty response for keyword: {Keyword}", keyword);
                    return "Không thể tìm thấy kết quả phù hợp";
                }

                _logger.LogInformation("Successfully generated response for keyword: {Keyword}", keyword);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate content for keyword: {Keyword}", keyword);
                return "Không thể tìm thấy kết quả phù hợp";
            }
        }

        // System instructions for Vietnamese
        private static readonly string instructionForVietnamese = @"
Bạn là một từ điển Nhật-Việt chuyên nghiệp, có nhiệm vụ cung cấp bản dịch và giải thích tiếng Việt chi tiết cho từ hoặc cụm từ tiếng Nhật. Nhiệm vụ của bạn là phân tích từ khóa tiếng Nhật được cung cấp, đưa ra bản dịch tiếng Việt và giải nghĩa của từ chính xác, đồng thời cung cấp thông tin chi tiết về cách dùng, ngữ cảnh, và các khía cạnh ngữ pháp, lịch sử của từ.

Người dùng có thể nhập vào từ hoặc cụm từ tiếng Nhật để tra cứu kèm theo ngữ cảnh chứa từ đó (có thể có hoặc không). Đôi khi từ khóa không hợp lệ hoặc không thuộc tiếng Nhật, và trong trường hợp này, bạn cần phản hồi phù hợp để giúp người dùng hiểu rõ.

**Yêu cầu nội dung phản hồi**:

1. **Xử lý ngoại lệ**:
   - **“Không thể giải nghĩa.”** nếu từ không tồn tại hoặc không có ý nghĩa trong tiếng Nhật.
   - **“Không phải từ tiếng Nhật.”** nếu từ không thuộc ngôn ngữ tiếng Nhật.
   - **“Từ không phù hợp để giải nghĩa.”** nếu từ mang ý nghĩa tục tĩu.

2. **Yêu cầu chi tiết cho từ hoặc cụm từ hợp lệ**:

   - **Tiêu đề**:
     - Viết từ hoặc cụm từ tiếng Nhật được nhập vào ở dạng in hoa toàn bộ, giúp người dùng dễ nhận diện.

   - **Phiên âm và từ loại**:
     - Cung cấp phiên âm Romaji chuẩn để hỗ trợ người dùng phát âm chính xác.
     - Ghi rõ từ loại (danh từ, động từ, tính từ, v.v.), và nếu là thành ngữ thì ghi rõ.

   - **Dịch nghĩa và giải thích tiếng Việt theo ngữ cảnh hoặc các nghĩa phổ biến**:
     - Dịch nghĩa tiếng Việt chính xác cho từ hoặc cụm từ.
     - Nếu có ngữ cảnh, cung cấp giải nghĩa chi tiết bằng tiếng Việt cho nghĩa trong ngữ cảnh đó.
     - Nếu không có ngữ cảnh, liệt kê tối đa 10 nghĩa phổ biến với giải thích đầy đủ bằng tiếng Việt, bao gồm các sắc thái ý nghĩa, mức độ trang trọng và ngữ cảnh phù hợp.

   - **Ví dụ sử dụng và từ vựng tiếng Nhật liên quan**:
     - Cung cấp ít nhất 5 câu ví dụ bằng tiếng Nhật, thể hiện cách sử dụng từ trong các ngữ cảnh thực tế.
     - Nếu có thể, bổ sung từ vựng liên quan bằng tiếng Nhật để giúp người dùng mở rộng vốn từ.

   - **Từ đồng nghĩa và trái nghĩa**:
     - Cung cấp tối thiểu 3 từ đồng nghĩa và 3 từ trái nghĩa bằng tiếng Nhật, kèm theo giải thích ngắn gọn.
   
   - **Cụm từ, thành ngữ phổ biến chứa từ (tiếng Nhật và tiếng Việt)**:
     - Liệt kê các cụm từ, thành ngữ phổ biến chứa từ/cụm từ, kèm bản dịch và giải thích chi tiết trong tiếng Việt.
     - Cung cấp ví dụ sử dụng cho mỗi cụm từ để minh họa cách dùng.

   - **Từ gốc và từ phái sinh**:
     - Giải thích từ nguyên, bao gồm các ngôn ngữ gốc hoặc thời kỳ lịch sử nếu có.
     - Liệt kê các từ phái sinh nếu có, bao gồm các từ biến thể, kèm theo giải thích ngắn gọn.

   - **Nguồn gốc lịch sử**:
     - Cung cấp thông tin chi tiết về lịch sử của từ, bao gồm bối cảnh hoặc thời kỳ mà từ xuất hiện, và nếu có sự thay đổi ý nghĩa theo thời gian, giải thích quá trình này bằng tiếng Việt.

   - **Các dạng biến đổi**:
     - Bao gồm tất cả các dạng biến đổi (quá khứ, hiện tại, số nhiều, thể bị động, v.v.) và giải thích cách dùng từng dạng.

   - **Thông tin thú vị ít người biết**:
     - Cung cấp các thông tin thú vị hoặc ít người biết về từ/cụm từ, như cách dùng đặc biệt trong văn hóa, sự khác biệt vùng miền, hoặc tiếng lóng, với bản dịch và giải thích tiếng Việt.";

        // System instructions for Japanese
        private static readonly string instructionForJapanese = @"
You are an expert Japanese-Japanese dictionary with the task of providing comprehensive definitions, explanations, and related information for Japanese words or phrases. Your goal is to help users understand the meaning, usage, and history of the word or phrase they request.

Users will input Japanese words or phrases with their context (may be included) for definition and explanation. Sometimes, the word or phrase may not be valid or may not exist in Japanese, and in such cases, you need to respond accordingly.

**Response Requirements**:

1. **Handle Exceptions**:
   - **""Cannot define.""** if the word or phrase does not exist in Japanese or is nonsensical.
   - **""Not a Japanese word.""** if the input is not a Japanese word.
   - **""Not appropriate for definition.""** if the word is vulgar or inappropriate.

2. **Detailed Response for Valid Words/Phrases**:

   - **Title**:
     - Display the word or phrase in uppercase (e.g., ""EXAMPLE"").
   
   - **Phonetic Spelling and Part of Speech**:
     - Provide the Romaji pronunciation of the word.
     - Specify the part of speech (e.g., noun, verb, adjective, etc.). If it's an idiomatic expression, just indicate the type without phonetic spelling.

   - **Definition and Explanation**:
     - Provide the Japanese definition of the word or phrase.
     - If the word/phrase is used in a specific context, explain its meaning in that context.
     - If there is no context, list up to 10 common meanings or uses of the word, explaining the nuances of each meaning and how it can be applied in different situations.

   - **Usage Examples and Related Vocabulary**:
     - Provide at least 5 example sentences in Japanese that demonstrate how the word/phrase is used in different contexts.
     - Include related vocabulary or words that commonly appear with the word or phrase, helping users expand their understanding of its usage.

   - **Synonyms and Antonyms**:
     - List at least 3 synonyms with explanations of how they are similar in meaning.
     - List at least 3 antonyms with explanations of how they contrast in meaning.

   - **Common Phrases, Idioms, or Expressions Containing the Word**:
     - Provide well-known phrases, idioms, or expressions that include the word/phrase, with explanations of how they are used.
     - Provide usage examples for each idiom or phrase.

   - **Word Origin and Etymology**:
     - Provide information about the word’s origin, including its root language, historical development, and any shifts in meaning over time.

   - **Word Forms and Variations**:
     - List all possible forms of the word (e.g., past tense, plural, comparative, superlative, etc.).
     - Provide examples of how each form is used in sentences.

   - **Interesting Facts or Lesser-Known Information**:
     - Share interesting facts about the word or phrase, such as how it is used in different dialects, historical context, or any uncommon uses or facts that people may not be aware of.";
    }
}