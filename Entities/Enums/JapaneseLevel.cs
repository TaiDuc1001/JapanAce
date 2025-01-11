using System.ComponentModel;

namespace Entities.Enums
{
    public enum JapaneseLevels
    {
        [Description("N5 - Cơ bản: Có thể hiểu một số cụm từ cơ bản, từ vựng và câu đơn giản trong tiếng Nhật")]
        N5_Beginner = 1,

        [Description("N4 - Sơ cấp: Có thể hiểu các đoạn hội thoại ngắn về chủ đề quen thuộc và đọc văn bản đơn giản")]
        N4_Elementary = 2,

        [Description("N3 - Trung cấp: Có thể hiểu các văn bản và hội thoại về các chủ đề hàng ngày, và giao tiếp ở mức độ cơ bản")]
        N3_Intermediate = 3,

        [Description("N2 - Trung cao cấp: Có thể hiểu các văn bản phức tạp hơn như bài báo, tạp chí và giao tiếp trôi chảy trong nhiều tình huống")]
        N2_UpperIntermediate = 4,

        [Description("N1 - Cao cấp: Có thể hiểu và sử dụng tiếng Nhật trong các tình huống học thuật, chuyên môn và giao tiếp như người bản xứ")]
        N1_Advanced = 5,
    }
}