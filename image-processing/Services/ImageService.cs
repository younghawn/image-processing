using OpenCvSharp;

namespace image_processing.Services
{
    public class ImageService
    {
        // 파일 경로로 이미지 불러와서 Mat 반환
        public Mat LoadImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("파일 경로가 비어있습니다.");

            Mat image = Cv2.ImRead(filePath, ImreadModes.Color);

            if (image.Empty())
                throw new Exception($"이미지를 불러올 수 없습니다: {filePath}");

            return image;
        }

        // 처리된 Mat을 파일로 저장
        public void SaveImage(Mat image, string savePath)
        {
            if (image == null || image.Empty())
                throw new ArgumentException("저장할 이미지가 없습니다.");

            Cv2.ImWrite(savePath, image);
        }
    }
}