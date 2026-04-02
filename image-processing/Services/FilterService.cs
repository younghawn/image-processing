using OpenCvSharp;

// FilterService는 OpenCvSharp.Size만 사용하므로 명시
using Size = OpenCvSharp.Size;

namespace image_processing.Services
{
    public class FilterService
    {
        // 가우시안 블러 - strength가 클수록 더 흐릿해짐
        // 커널 사이즈는 반드시 홀수여야 해서 *2+1 공식 사용
        public Mat ApplyBlur(Mat source, int strength)
        {
            Mat result = new Mat();
            int kernelSize = strength * 2 + 1;
            Cv2.GaussianBlur(source, result, new Size(kernelSize, kernelSize), 0);
            return result;
        }

        // 흑백 처리 - BGR→GRAY 변환 후 다시 BGR로 변환 (PictureBox 호환)
        public Mat ApplyGrayscale(Mat source)
        {
            Mat gray = new Mat();
            Mat result = new Mat();
            Cv2.CvtColor(source, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.CvtColor(gray, result, ColorConversionCodes.GRAY2BGR);
            gray.Dispose();
            return result;
        }

        // 색상 추출 - HSV 공간에서 특정 색상 범위만 마스킹해서 추출
        public Mat ApplyColorFilter(Mat source, string color)
        {
            Mat hsv = new Mat();
            Mat mask = new Mat();
            Mat result = new Mat();

            Cv2.CvtColor(source, hsv, ColorConversionCodes.BGR2HSV);

            // HSV 기준 색상 범위 지정
            Scalar lower, upper;
            switch (color.ToLower())
            {
                case "red":
                    // 빨강은 HSV에서 0~10 / 160~180 두 구간에 걸쳐있음
                    Mat mask1 = new Mat(), mask2 = new Mat();
                    Cv2.InRange(hsv, new Scalar(0, 100, 100), new Scalar(10, 255, 255), mask1);
                    Cv2.InRange(hsv, new Scalar(160, 100, 100), new Scalar(180, 255, 255), mask2);
                    Cv2.Add(mask1, mask2, mask);
                    mask1.Dispose(); mask2.Dispose();
                    break;
                case "green":
                    lower = new Scalar(40, 60, 60);
                    upper = new Scalar(80, 255, 255);
                    Cv2.InRange(hsv, lower, upper, mask);
                    break;
                case "blue":
                    lower = new Scalar(100, 60, 60);
                    upper = new Scalar(140, 255, 255);
                    Cv2.InRange(hsv, lower, upper, mask);
                    break;
                default:
                    hsv.Dispose(); mask.Dispose();
                    return source.Clone();
            }

            Cv2.BitwiseAnd(source, source, result, mask);
            hsv.Dispose(); mask.Dispose();
            return result;
        }

        // 캐니 엣지 검출 - 경계선만 추출
        public Mat ApplyCanny(Mat source, int threshold1 = 100, int threshold2 = 200)
        {
            Mat gray = new Mat();
            Mat edges = new Mat();
            Mat result = new Mat();

            Cv2.CvtColor(source, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Canny(gray, edges, threshold1, threshold2);
            Cv2.CvtColor(edges, result, ColorConversionCodes.GRAY2BGR);

            gray.Dispose(); edges.Dispose();
            return result;
        }
    }
}