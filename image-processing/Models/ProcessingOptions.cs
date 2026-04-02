namespace image_processing.Models
{
    // 각 필터에 필요한 옵션값들을 하나로 묶음
    public struct ProcessingOptions
    {
        public int BlurStrength { get; set; }      // 블러 강도 (1~20)
        public string ColorTarget { get; set; }    // 추출할 색상 (red/green/blue)
        public int CannyThreshold1 { get; set; }   // 엣지 검출 하한 임계값
        public int CannyThreshold2 { get; set; }   // 엣지 검출 상한 임계값

        // 기본값 세팅
        public static ProcessingOptions Default => new ProcessingOptions
        {
            BlurStrength = 5,
            ColorTarget = "red",
            CannyThreshold1 = 100,
            CannyThreshold2 = 200
        };
    }
}