namespace image_processing.Models
{
    // 이미지 처리 행동 하나하나를 담는 구조체
    public struct ImageLog
    {
        public string FileName { get; set; }     // 파일명 (a.jpg)
        public string ProcessType { get; set; }  // 처리 종류 (blur, grayscale 등)
        public string Parameter { get; set; }    // 수치나 옵션값 (강도, 색상명 등)
        public DateTime Timestamp { get; set; }  // 처리 시각

        public ImageLog(string fileName, string processType, string parameter)
        {
            FileName = fileName;
            ProcessType = processType;
            Parameter = parameter;
            Timestamp = DateTime.Now;
        }

        // CSV 한 줄로 변환
        public string ToCsvLine()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss},{FileName},{ProcessType},{Parameter}";
        }
    }
}