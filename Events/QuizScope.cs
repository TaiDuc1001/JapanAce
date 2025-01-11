using Entities;
using Entities.Enums;
using Gemini;
using Helper;
using Newtonsoft.Json;
using System.Text;

namespace Events
{
    public static class QuizScope
    {
        public const sbyte MaxTotalWordsOfTopic = 10;
        public const sbyte MinTotalQuestions = 10;
        public const sbyte MaxTotalQuestions = 100;
        public const int ThreeDaysAsCachingAge = 259200;
        public const int MaxTimeAsCachingAge = int.MaxValue;

        public const string Instruction = @"
You are an expert Japanese teacher with over 20 years of teaching experience, and you have spent more than 10 years teaching Japanese to Vietnamese students. Your deep understanding of language learning challenges allows you to create highly effective, engaging, and pedagogically sound multiple-choice questions. Below are the detailed requirements for the question set generation:

### 1. **Japanese Proficiency Level**:
   - I will provide my Japanese proficiency level according to the JLPT (Japanese-Language Proficiency Test), which will fall into one of the following categories:
     - **N5**: Beginner (basic understanding of hiragana, katakana, and simple phrases).
     - **N4**: Elementary (can understand basic sentences and everyday conversations).
     - **N3**: Intermediate (can understand main points of clear standard input on familiar topics).
     - **N2**: Upper-intermediate (can understand complex texts and conversations on a variety of topics).
     - **N1**: Advanced (can understand highly detailed and complex texts, near-native fluency).
   
   - Based on the level I provide, your task is to **generate questions appropriate to that level**. For example:
     - **N5**: Short, simple questions with basic vocabulary and hiragana/katakana.
     - **N2**: Complex questions involving kanji, grammar, and topics that require deeper understanding.

### 2. **Question Creation Guidelines**:
   - **Clarity and Precision**: Your questions should be **clear, direct, and unambiguous**. Avoid using unnecessarily complicated language. Each question should be grammatically correct and easy to understand for the given proficiency level.
   - **Question Types**: Focus on practical, real-world scenarios. Examples of types of questions:
     - **Vocabulary**: Asking for meanings of common words or phrases.
     - **Grammar**: Correct usage of particles, verb conjugations, etc.
     - **Contextual Understanding**: Questions that involve understanding the main ideas of simple or complex texts.
     - **Practical Situations**: Everyday conversation topics (e.g., ordering food, asking for directions, etc.).
   
   - **Choices**: For each question, provide **4 unique choices**. One choice must be the **correct** answer, and the remaining three should be plausible but incorrect answers.
     - The choices should be logically consistent and should not introduce ambiguity.
     - Ensure that the incorrect options are not obvious mistakes but are reasonable distractors based on common learner errors.
   
   - **Correct Answer**: Ensure the correct answer is indisputable. Do not make the question too easy or too tricky.

### 3. **Explanation of Correct Answer**:
   - After each question, provide a **brief explanation in Vietnamese** for why the correct answer is right. The explanation should be:
     - **Clear and concise**, suitable for the proficiency level.
     - **Avoid overwhelming details**; focus on the key learning points.
     - Provide **examples or context** if needed to make the explanation clearer. For instance, if the correct answer involves a specific grammar point, explain that with a simple example.
   - If the explanation requires a specific language rule, be sure to give a short rule or exception (e.g., usage of particles like 'は' vs. 'が').

### 4. **Priority in Question Creation**:
   - **Engagement**: Questions should be engaging and reflect real-world scenarios that are interesting and useful for language learners. For example, instead of asking about random vocabulary, relate it to daily life (e.g., “What do you say when ordering food in a restaurant?”).
   - **Clarity and Consistency**: The explanations, choices, and the reasoning behind the correct answers should all be **consistent** and **easy to follow**.
   - **Motivation**: Keep the questions positive and encouraging. If the question or explanation is too difficult, adjust the difficulty to motivate further learning.

## Output Format:
### Structured in JSON Format:
   - Return your response in a **valid JSON array**, each object containing the following fields:
     - `Question`: The question text in Japanese. Ensure it is grammatically correct and clearly stated for the given level.
     - `Options`: A list of 4 unique choices, where one is the correct answer. Each choice should be a valid option in the context of the question.
     - `RightOptionIndex`: The **index (0-3)** of the correct answer in the `Options` list. Ensure this index is correct based on the correct choice.
     - `ExplanationInVietnamese`: A **brief explanation** of why the correct answer is correct, written in simple, clear Vietnamese.
   
   - Ensure that the **JSON structure is properly formatted** and valid, adhering to JSON syntax conventions.";

        public static async Task<List<Quiz>> GenerateQuizes(string apiKey, string topic, List<QuizzType> quizzTypes, JapaneseLevels level, short questionsCount)
        {
            if (questionsCount < MinTotalQuestions || questionsCount > MaxTotalQuestions)
            {
                throw new ArgumentOutOfRangeException(nameof(questionsCount), $"Questions count must be between {MinTotalQuestions} and {MaxTotalQuestions}.");
            }

            if (quizzTypes == null || quizzTypes.Count == 0)
            {
                throw new ArgumentException("At least one quiz type must be specified.", nameof(quizzTypes));
            }

            if (questionsCount <= 15)
            {
                var results = await GenerateQuizesForLessThan15(apiKey, topic, quizzTypes, level, questionsCount);

                if (results == null || results.Count == 0)
                {
                    throw new InvalidOperationException("Failed to generate quizzes. Please check the input parameters and try again.");
                }

                return results
                    .Take(questionsCount)
                    .Select(q => new Quiz
                    {
                        Question = q.Question?.Replace("**", "'") ?? string.Empty,
                        Options = q.Options?.Select(o => o?.Replace("**", "'") ?? string.Empty).ToList() ?? new List<string>(),
                        RightOptionIndex = q.RightOptionIndex,
                        ExplanationInVietnamese = q.ExplanationInVietnamese?.Replace("**", "'") ?? string.Empty,
                    })
                    .ToList();
            }
            else
            {
                var quizes = new List<Quiz>();
                var quizTypeQuestionCount = GeneralHelper.GenerateRandomNumbers(quizzTypes.Count, questionsCount);
                var tasks = new List<Task<List<Quiz>>>();

                for (int i = 0; i < quizTypeQuestionCount.Count; i++)
                {
                    tasks.Add(GenerateQuizesByType(apiKey, topic, (QuizzType)(i + 1), level, quizTypeQuestionCount[i]));
                }

                var results = await Task.WhenAll(tasks);

                foreach (var result in results)
                {
                    if (result != null && result.Count != 0)
                    {
                        quizes.AddRange(result);
                    }
                }

                var random = new Random();

                return quizes.Count == 0
                    ? throw new InvalidOperationException("No quizzes were generated. Please check the input parameters and try again.")
                    : quizes
                        .AsParallel()
                        .OrderBy(x => random.Next())
                        .Select(q => new Quiz
                        {
                            Question = q.Question?.Replace("**", "'") ?? string.Empty,
                            Options = q.Options?.Select(o => o?.Replace("**", "'") ?? string.Empty).ToList() ?? new List<string>(),
                            RightOptionIndex = q.RightOptionIndex,
                            ExplanationInVietnamese = q.ExplanationInVietnamese?.Replace("**", "'") ?? string.Empty,
                        })
                        .Take(questionsCount)
                        .ToList();
            }
        }

        private static async Task<List<Quiz>> GenerateQuizesForLessThan15(string apiKey, string topic, List<QuizzType> quizzTypes, JapaneseLevels level, int questionsCount)
        {
            try
            {
                var userLevel = GeneralHelper.GetEnumDescription(level);
                var types = string.Join(", ", quizzTypes.Select(t => GeneralHelper.GetEnumDescription(t)).ToList());
                var promptBuilder = new StringBuilder();

                promptBuilder.AppendLine($"I am a Vietnamese learner with the Japanese proficiency level of `{userLevel}` according to the JLPT standard.");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("## The description of my level according to the JLPT standard:");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine(GetLevelDescription(level));
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("## Your task:");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine($"Generate a set of multiple-choice Japanese questions consisting of {questionsCount} to {questionsCount + 5} questions related to the topic '{topic.Trim()}' for me to practice, the quiz should be of the types: {types}");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("The generated questions should be of the types:");
                foreach (var type in quizzTypes)
                {
                    promptBuilder.AppendLine($"- {GeneralHelper.GetEnumDescription(type)}");
                }

                var response = await Generator.GenerateContent(apiKey, Instruction, promptBuilder.ToString(), true, 30);
                return JsonConvert.DeserializeObject<List<Quiz>>(response) ?? new List<Quiz>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate quizzes. Please check the input parameters and try again.", ex);
            }
        }

        private static async Task<List<Quiz>> GenerateQuizesByType(string apiKey, string topic, QuizzType quizzType, JapaneseLevels level, int questionsCount)
        {
            try
            {
                var promptBuilder = new StringBuilder();
                var userLevel = GeneralHelper.GetEnumDescription(level);
                var type = GeneralHelper.GetEnumDescription(quizzType);

                promptBuilder.AppendLine($"I am a Vietnamese learner with the Japanese proficiency level of `{userLevel}` according to the JLPT standard.");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("## The description of my level according to the JLPT standard:");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine(GetLevelDescription(level));
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("## Your task:");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine($"Generate a set of multiple-choice Japanese questions consisting of {questionsCount} to {questionsCount + 5} questions related to the topic '{topic.Trim()}' for me to practice, the type of the questions must be: {type}");

                var response = await Generator.GenerateContent(apiKey, Instruction, promptBuilder.ToString(), true, 40);

                return JsonConvert.DeserializeObject<List<Quiz>>(response)
                    ?.Take(questionsCount)
                    .Select(quiz =>
                    {
                        quiz.Question = $"({NameAttribute.GetEnumName(quizzType)}) {quiz.Question}";
                        return quiz;
                    })
                    .ToList() ?? new List<Quiz>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to generate quizzes for type {quizzType}. Please check the input parameters and try again.", ex);
            }
        }

        public static async Task<List<string>> SuggestTopics(string apiKey, JapaneseLevels level)
        {
            try
            {
                var promptBuilder = new StringBuilder();
                var userLevel = GeneralHelper.GetEnumDescription(level);

                var instruction = "You are an experienced Japanese teacher with over 20 years of experience, currently teaching in Vietnam. I am looking for a list of interesting and engaging topics that match my current Japanese proficiency level, as well as topics that can help me stay motivated in my learning journey.";
                promptBuilder.AppendLine($"My current Japanese proficiency level is '{userLevel}' according to the JLPT standard.");
                promptBuilder.AppendLine("This is the description of my Japanese proficiency according to the JLPT standard:");
                promptBuilder.AppendLine(GetLevelDescription(level));
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("Please suggest at least 20 completely different topics, each containing fewer than 5 words, that you think are most suitable and interesting for practicing Japanese and match the description of my JLPT level as mentioned above.");
                promptBuilder.AppendLine("The topics should cover a variety of themes, such as daily life, culture, education, environment, travel, etc., to keep the practice diverse and engaging.");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("The list of suggested topics should be returned as a JSON array corresponding to the List<string> data type in C# programming language.");
                promptBuilder.AppendLine("To make the format clear, here's an example of the expected output:");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("```json");
                promptBuilder.AppendLine("[");
                promptBuilder.AppendLine("  \"日本の伝統文化\",");
                promptBuilder.AppendLine("  \"現代技術\",");
                promptBuilder.AppendLine("  \"旅行体験\",");
                promptBuilder.AppendLine("  \"環境問題\"");
                promptBuilder.AppendLine("]");
                promptBuilder.AppendLine("```");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("Make sure that each topic is unique, concise, and relevant for practicing Japanese at different levels, especially intermediate to advanced.");

                var response = await Generator.GenerateContent(apiKey, instruction, promptBuilder.ToString(), true, 75);
                return JsonConvert.DeserializeObject<List<string>>(response) ?? new List<string>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to suggest topics. Please check the input parameters and try again.", ex);
            }
        }

        private static string GetLevelDescription(JapaneseLevels level)
        {
            return level switch
            {
                JapaneseLevels.N5_Beginner => @"
Level N5 (Beginner)
- **Hiragana and Katakana**: Can read and write basic hiragana and katakana.
- **Basic Vocabulary**: Understands simple words and phrases related to daily life.
- **Simple Sentences**: Can form and understand basic sentences (e.g., 'これはペンです').
- **Grammar**: Understands basic particles (は, を, に) and verb conjugations (ます form).
- **Topics**: Greetings, numbers, time, family, and daily routines.",

                JapaneseLevels.N4_Elementary => @"
Level N4 (Elementary)
- **Kanji**: Can read and write around 100-200 basic kanji.
- **Vocabulary**: Understands common words and phrases used in everyday situations.
- **Conversations**: Can understand and participate in simple conversations.
- **Grammar**: Understands basic sentence structures, past tense, and polite forms.
- **Topics**: Shopping, travel, hobbies, and basic descriptions.",

                JapaneseLevels.N3_Intermediate => @"
Level N3 (Intermediate)
- **Kanji**: Can read and write around 300-600 kanji.
- **Vocabulary**: Understands a wider range of vocabulary for daily and some specialized topics.
- **Reading**: Can understand the main points of short texts and articles.
- **Grammar**: Understands more complex sentence structures, conditionals, and indirect speech.
- **Topics**: Work, school, news, and personal experiences.",

                JapaneseLevels.N2_UpperIntermediate => @"
Level N2 (Upper-Intermediate)
- **Kanji**: Can read and write around 1000 kanji.
- **Vocabulary**: Understands a broad range of vocabulary for academic and professional contexts.
- **Reading**: Can understand complex texts, such as newspapers and essays.
- **Grammar**: Understands advanced grammar, including passive and causative forms.
- **Topics**: Politics, culture, technology, and abstract concepts.",

                JapaneseLevels.N1_Advanced => @"
Level N1 (Advanced)
- **Kanji**: Can read and write around 2000 kanji.
- **Vocabulary**: Understands highly specialized and nuanced vocabulary.
- **Reading**: Can understand highly complex texts, such as literature and academic papers.
- **Grammar**: Mastery of advanced grammar, including formal and informal speech.
- **Topics**: Philosophy, advanced literature, and professional discussions.",

                _ => string.Empty,
            };
        }
    }
}
