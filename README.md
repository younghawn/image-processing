# 🖼️ 이미지 처리 프로그램

## 기술 스택
- C#
- WinForms (.NET 10)
- OpenCvSharp4

---

## 시연 영상
---![이미지 프로세싱1](https://github.com/user-attachments/assets/145fb4bc-0a32-4e0e-bfb5-f0b8753c849c)
![이미지 프로세싱2](https://github.com/user-attachments/assets/25ef4eea-921d-4a46-891a-8d6f8f29837d)


## 프로젝트 설명

C#, WinForms, OpenCV를 이용하여 이미지 처리 프로그램을 개발했습니다.

이미지를 불러온 후 다양한 필터를 실시간으로 적용할 수 있으며,
모든 처리 내역은 CSV 파일로 자동 저장됩니다.

---

## 주요 기능

| 기능 | 설명 |
|------|------|
| 이미지 로드 | 파일 탐색기를 통해 jpg, png, bmp 형식의 이미지 불러오기 |
| 블러 처리 | 슬라이더로 강도 조절 가능한 가우시안 블러 |
| 흑백 처리 | BGR → Grayscale 변환 |
| 엣지 검출 | Canny 알고리즘을 이용한 경계선 추출 |
| 색상 추출 | HSV 색공간 기반 Red / Green / Blue 색상 영역 추출 |
| 원본 복원 | 처리 전 원본 이미지로 즉시 복원 |
| 이미지 저장 | 처리된 이미지를 png / jpg로 저장 |
| CSV 로그 | 모든 처리 동작을 CSV 파일로 자동 기록 |

---

## 프로젝트 구조
```
ImageProcessor/
├── Models/
│   ├── ImageLog.cs          # 로그 데이터 구조체
│   └── ProcessingOptions.cs # 처리 옵션 구조체
├── Services/
│   ├── ImageService.cs      # 이미지 로드/저장
│   ├── FilterService.cs     # OpenCV 필터 처리
│   └── LogService.cs        # CSV 로그 기록
└── Form1.cs                 # UI 및 이벤트 처리
```

---

## CSV 로그 예시
```csv
Timestamp,FileName,ProcessType,Parameter
2026-04-02 12:30:01,a.jpg,load,-
2026-04-02 12:30:05,a.jpg,blur,5
2026-04-02 12:30:10,a.jpg,grayscale,-
2026-04-02 12:30:15,a.jpg,color_filter,red
```
