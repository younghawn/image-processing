using image_processing.Models;
using System.IO;

namespace image_processing.Services
{
    public class LogService
    {
        private readonly string _logDirectory;
        private List<ImageLog> _logList; // 요구사항: List 활용

        public LogService(string logDirectory)
        {
            _logDirectory = logDirectory;
            _logList = new List<ImageLog>();

            // 로그 폴더 없으면 자동 생성
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }

        public void AddLog(ImageLog log)
        {
            _logList.Add(log);   // 메모리 리스트에 추가
            WriteToFile(log);    // 파일에 즉시 기록
        }

        private void WriteToFile(ImageLog log)
        {
            // 이미지 파일명 기준으로 로그 파일 분리 (a.jpg → a_log.csv)
            string baseName = Path.GetFileNameWithoutExtension(log.FileName);
            string csvPath = Path.Combine(_logDirectory, $"{baseName}_log.csv");

            bool isNewFile = !File.Exists(csvPath);

            using (StreamWriter writer = new StreamWriter(csvPath, append: true))
            {
                // 새 파일이면 헤더 먼저 작성
                if (isNewFile)
                    writer.WriteLine("Timestamp,FileName,ProcessType,Parameter");

                writer.WriteLine(log.ToCsvLine());
            }
        }

        // 전체 로그 리스트 조회
        public List<ImageLog> GetAllLogs() => new List<ImageLog>(_logList);

        // 특정 파일 로그만 필터링
        public List<ImageLog> GetLogsByFile(string fileName)
        {
            return _logList.Where(l => l.FileName == fileName).ToList();
        }
    }
}