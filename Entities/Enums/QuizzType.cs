using System.ComponentModel;

namespace Entities.Enums
{
    public enum QuizzType
    {
        [Name("Chọn từ thích hợp nhất"), Description("適切な単語を選択する (Chọn từ thích hợp nhất)")]
        WordChoice = 1,

        [Name("Chia động từ"), Description("動詞の活用 (Chia động từ)")]
        VerbConjugation = 2,

        [Name("Sử dụng trợ từ"), Description("助詞の使い方 (Sử dụng trợ từ)")]
        ParticleUsage = 3,

        [Name("Kính ngữ và thể lịch sự"), Description("敬語と丁寧語 (Kính ngữ và thể lịch sự)")]
        HonorificsAndKeigo = 4,

        [Name("Đọc và viết Kanji"), Description("漢字の読み書き (Đọc và viết Kanji)")]
        KanjiReadingAndWriting = 5,

        [Name("Cấu trúc câu"), Description("文の構造 (Cấu trúc câu)")]
        SentenceStructure = 6,

        [Name("Từ vựng"), Description("語彙 (Từ vựng)")]
        Vocabulary = 7,

        [Name("Đọc hiểu"), Description("読解 (Đọc hiểu)")]
        ReadingComprehension = 8,

        [Name("Ngữ pháp"), Description("文法 (Ngữ pháp)")]
        Grammar = 9,

        [Name("Thành ngữ"), Description("慣用句 (Thành ngữ)")]
        IdiomaticExpressions = 10
    }
}