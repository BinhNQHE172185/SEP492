using LMCM_BE.DTOs.ContractValueItemDtos;

namespace LMCM_BE.Utilities
{
    public static class ContractValueItemSeedData
    {
        public static readonly List<ContractValueItemDto> Items = new List<ContractValueItemDto>
         {
         new ContractValueItemDto
                {
                    Category = "Đề xuất và đánh giá giáo trình",
                    MeasurementUnit = "Tối thiểu 3 quyển/môn",
                    StandardRate = 0.5M,
                    QualityRequirements = "Việc chọn, đánh giá giáo trình, thực hiện theo đúng biểu mẫu quy định.",
                    ContractValue = 800000
                },
                new ContractValueItemDto
                {
                    Category = "Đề cương",
                    MeasurementUnit = "Đề cương/Môn",
                    StandardRate = 0.5M,
                    QualityRequirements = "Sử dụng đúng biểu mẫu đề cương do PTCT cung cấp.  \r\nĐiền đầy đủ, chi tiết các mục trong mẫu, đảm bảo tính chuyên môn, tính logic, chính xác, khoa học. \r\nNội dung bám sát chuẩn đầu ra của môn học, mô đun và nội dung chính của giáo trình. ",
                    ContractValue = 1500000
                },
                new ContractValueItemDto
                {
                    Category = "Slide bài giảng truyền thống hoặc online",
                    MeasurementUnit = "Tối thiểu 30 trang/bài 2 giờ",
                    StandardRate = 0.5M,
                    QualityRequirements = "Về nội dung: bám sát chuẩn đầu ra, nội dung chính của giáo trình và đề cương môn học/mô đun, đảm bảo kiến thức chuyên môn, trình bày khoa học, dễ hiểu.  \r\nVề hình thức: đảm bảo về thiết kế đồ họa, kiểu chữ, cỡ chữ, yêu cầu về soạn thảo, công thức, biểu mẫu do PTCT quy định. \r\nYêu cầu cụ thể: - Slide bài giảng truyền thống: có phần Note dưới slide cho tất cả các trang để hướng dẫn giảng viên triển khai trên lớp.  - Slide bài học online: dùng để ghi hình bài giảng online, bao gồm cả các câu hỏi trắc nghiệm để củng cố kiến thức cho sinh viên. ",
                    ContractValue = 1000000
                },
                new ContractValueItemDto
                {
                    Category = "Slide bài trên lớp với môn blended",
                    MeasurementUnit = "Bài 2 giờ",
                    StandardRate = 0.5M,
                    QualityRequirements = "Slide kịch bản đứng lớp, đi kèm là lesson plan cung cấp các hoạt động, bài tập nhỏ, các tình huống gắn với nội dung bài học online nhằm giúp sinh viên vận dụng kiến thức. Lesson plan là kế hoạch triển khai cho từng buổi học, hướng dẫn GV cách thức triển khai cũng như quy định thời lượng cho từng hoạt động trên lớp.",
                    ContractValue = 650000
                },
                new ContractValueItemDto
                {
                    Category = "Ghi hình bài giảng online",
                    MeasurementUnit = "Tối thiểu 20 phút/Bài 2 giờ",
                    StandardRate = 0.5M,
                    QualityRequirements = "GV giảng theo kịch bản slide online, có thể cho thêm các tình huống minh họa hoặc bài tập để sinh viên thực hành thêm, đảm bảo tính chuyên môn. \r\nTrình bày ngắn gọn, súc tích, dễ hiểu. \r\nTrang phục, tác phong phù hợp. Chất giọng tốt, không được nói ngọng, nói lắp.",
                    ContractValue = 650000
                },
                new ContractValueItemDto
                {
                    Category = "Bài tập lớn/Dự án môn học",
                    MeasurementUnit = "Cho cả môn học",
                    StandardRate = 0.5M,
                    QualityRequirements = "Bài tập lớn/ Dự án môn học cần đảm bảo vận dụng kiến thức môn học vào ứng dụng trong thực tiễn, phát huy sự sáng tạo và kĩ năng nghề nghiệp của sinh viên.  \r\nMục tiêu Bài tập lớn/ Dự án đáp ứng mục tiêu môn học, mô đun. \r\nĐưa ra thang đánh giá chi tiết cho từng giai đoạn.  \r\nSử dụng đúng biểu mẫu do PTCT cung cấp",
                    ContractValue = 900000
                },
                new ContractValueItemDto
                {
                    Category = "Casestudy",
                    MeasurementUnit = "Case(s) cho 2 giờ",
                    StandardRate = 0.5M,
                    QualityRequirements = "Là những dạng bài tập, trao đổi, vấn đề liên quan đến môn học có đáp án nhằm giúp sinh viên giải quyết vấn đề thực tế trong công việc. \r\nSử dụng đúng mẫu do PTCT cung cấp. \r\nTối thiểu 4 bài tập kèm đáp án cho 1 casestudy. ",
                    ContractValue = 650000
                },
                new ContractValueItemDto
                {
                    Category = "Lab",
                    MeasurementUnit = "Lab(s) cho 2 giờ",
                    StandardRate = 0.5M,
                    QualityRequirements = "Hướng dẫn và yêu cầu thực hành cho một buổi dạy thực hành. \r\nNội dung bài Lab bám sát nội dung chính của Slide bài giảng, đảm bảo các dạng bài tập thực hành phù hợp với phần lý thuyết đã dạy ở buổi trước. \r\nSử dụng đúng mẫu do PTCT cung cấp. ",
                    ContractValue = 650000
                },
                new ContractValueItemDto
                {
                    Category = "Câu hỏi trắc nghiệm thông thường",
                    MeasurementUnit = "Câu/tối đa 2 phút",
                    StandardRate = 0.5M,
                    QualityRequirements = "Câu hỏi bám sát chuẩn đầu ra, nội dung giáo trình, slide và các tài liệu khác của môn học, mô đun. \r\nĐảm bảo tính chuyên môn, văn phạm và thời lượng. \r\nKhông sai lỗi chính tả. \r\nSử dụng đúng mẫu do PTCT cung cấp. \r\nĐưa ra đáp án chính xác cho từng câu hỏi. ",
                    ContractValue = 10000
                },
                new ContractValueItemDto
                {
                    Category = "Câu hỏi trắc nghiệm phức tạp",
                    MeasurementUnit = "Câu/trên 2 phút",
                    StandardRate = 0.5M,
                    QualityRequirements = "Câu hỏi bám sát chuẩn đầu ra, nội dung giáo trình, slide và các tài liệu khác của môn học, mô đun. \r\nĐảm bảo tính chuyên môn, văn phạm và thời lượng. \r\nKhông sai lỗi chính tả. \r\nSử dụng đúng mẫu do PTCT cung cấp. \r\nĐưa ra đáp án chính xác cho từng câu hỏi.",
                    ContractValue = 20000
                },
                new ContractValueItemDto
                {
                    Category = "Đề kiểm tra/thi tự luận/thực hành",
                    MeasurementUnit = "Đề/60 phút",
                    StandardRate = 0.5M,
                    QualityRequirements = "Đề kiểm tra gồm 2-3 câu hỏi mang tính vận dụng và thực hành bám sát chuẩn đầu ra và nội dung theo mô tả trong đề cương môn học, môn đun, có barem và đáp án chấm điểm đi kèm. \r\nĐề kiểm tra đảm bảo tính chuyên môn, văn phạm và thời lượng. \r\nThang điểm tổng là 10 điểm. \r\nMức điểm thấp nhất trong barem chấm là 0.25 điểm. \r\nLàm theo đúng mẫu do PTCT cung cấp. ",
                    ContractValue = 200000
                },
                new ContractValueItemDto
                {
                    Category = "Test (Làm thử) đề thi",
                    MeasurementUnit = "Đề/60 phút",
                    StandardRate = 0.5M,
                    QualityRequirements = "GV/Chuyên gia sẽ làm thử bài thi như một sinh viên để đánh giá sự chính xác của đề thi về dữ liệu đề thi, thời gian phù hợp để sinh viên có thể hoàn thành bài thi. \r\nPhát hiện kịp thời những bất cập về nội dung (cấu trúc, độ khó, đáp án…) của đề thi và tính toàn vẹn của đề thi. \r\nĐề thi sau khi được test sẽ phải được chỉnh sửa thành đề thi hoàn chỉnh.",
                    ContractValue = 130000
                },
                new ContractValueItemDto
                {
                    Category = "Thiết kế đồ họa cho bài slide",
                    MeasurementUnit = "Bài 2 giờ",
                    StandardRate = 0.5M,
                    QualityRequirements = "Thiết kế đồ họa đẹp, sinh động, lựa chọn hình ảnh phù hợp với nội dung bài học, đảm bảo về tính thẩm mỹ và chuyên môn. ",
                    ContractValue = 130000
                },
                new ContractValueItemDto
                {
                    Category = "Chương trình đào tạo",
                    MeasurementUnit = "Ngành cao đẳng",
                    StandardRate = 0.5M,
                    QualityRequirements = "Theo mẫu quy định của Cơ quan chủ quản \r\n- Nội dung phải đảm bảo quy định về khối lượng kiến thức tối thiểu, yêu cầu về năng lực mà người học đạt được sau khi tốt nghiệp. \r\n- Chương trình đào tạo phải xác định được danh mục và thời lượng của từng môn học, mô đun tương ứng với phương thức đào tạo; thời gian học lý thuyết và thời gian học thực hành, thực tập. \r\n- Bảo đảm tính khoa học, hệ thống, thực tiễn và linh hoạt, đáp ứng sự thay đổi của kỹ thuật công nghệ và thị trường lao động. \r\n- Phân bổ thời gian, trình tự thực hiện các môn học, mô đun để đảm bảo thực hiện được mục tiêu chương trình. \r\n- Nêu rõ các môn học, mô đun tiên quyết. \r\n- Quy định những yêu cầu tối thiểu về cơ sở vật chất, đội ngũ giáo viên để triển khai thực hiện chương trình đào tạo nhằm đảm bảo chất lượng đào tạo. \r\n- Quy định phương pháp đánh giá kết quả học tập, xác định mức độ đạt yêu cầu về năng lực của người học sau khi học xong các môn học, mô đun của chương trình đào tạo. \r\n- Nội dung chương trình đào tạo phải phù hợp với yêu cầu phát triển của ngành, địa phương và đất nước, phù hợp với kỹ thuật công nghệ trong sản xuất, dịch vụ. \r\n- Mô tả nội dung của mỗi học phần (khoảng 5-10 câu). \r\n- Bảo đảm tính hiện đại và hội nhập quốc tế, có xu hướng tiếp cận với trình độ đào tạo nghề nghiệp tiên tiến của khu vực và thế giới. \r\n- Bảo đảm việc liên thông giữa các trình độ đào tạo trong hệ thống giáo dục quốc dân. \r\n- Chuẩn đầu ra và ma trận chuẩn đầu ra với các môn học, mô đun. ",
                    ContractValue = 13000000
                }
        };
    }
}
