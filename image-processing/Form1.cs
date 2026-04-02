using image_processing.Models;
using image_processing.Services;
using OpenCvSharp;
using OpenCvSharp.Extensions;

using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace image_processing
{
    public partial class Form1 : Form
    {
        // Services
        private readonly ImageService _imageService;
        private readonly FilterService _filterService;
        private readonly LogService _logService;

        // 상태 변수
        private Mat? _originalMat;
        private Mat? _currentMat;
        private string _currentFileName = string.Empty;
        private ProcessingOptions _options = ProcessingOptions.Default;

        // UI 컨트롤
        private PictureBox? picOriginal, picProcessed;
        private Button? btnLoad, btnReset, btnSave;
        private Button? btnGrayscale, btnCanny, btnApplyColor;
        private TrackBar? trkBlur;
        private Label? lblOriginal, lblProcessed, lblBlurTitle, lblBlurValue, lblColorTitle;
        private ComboBox? cmbColor;
        private Panel? panelControls;

        public Form1()
        {
            InitializeComponent();

            _imageService = new ImageService();
            _filterService = new FilterService();
            _logService = new LogService(
                Path.Combine(Application.StartupPath, "logs")
            );

            BuildUI();
        }

        
        // UI 직접 구성 (디자이너 없이 코드로)
        
        private void BuildUI()
        {
            this.Text = "Image Processor";
            this.Size = new Size(1200, 750);
            this.MinimumSize = new Size(1000, 650);
            this.BackColor = Color.FromArgb(28, 28, 28);

            // 레이블
            lblOriginal = MakeLabel("원본 이미지", new Point(10, 10));
            lblProcessed = MakeLabel("처리된 이미지", new Point(610, 10));

            // PictureBox
            picOriginal = MakePictureBox(new Point(10, 35));
            picProcessed = MakePictureBox(new Point(610, 35));

            // 컨트롤 패널
            panelControls = new Panel
            {
                Location = new Point(10, 510),
                Size = new Size(1165, 180),
                BackColor = Color.FromArgb(42, 42, 42),
                BorderStyle = BorderStyle.FixedSingle
            };

            // 버튼들
            btnLoad = MakeButton("📂 이미지 불러오기", new Point(10, 10),
                Color.FromArgb(0, 112, 190));
            btnLoad.Click += BtnLoad_Click;

            btnReset = MakeButton("🔄 원본 복원", new Point(160, 10),
                Color.FromArgb(70, 70, 70));
            btnReset.Click += BtnReset_Click;

            btnSave = MakeButton("💾 이미지 저장", new Point(310, 10),
                Color.FromArgb(20, 120, 20));
            btnSave.Click += BtnSave_Click;

            btnGrayscale = MakeButton("⬜ 흑백 처리", new Point(10, 70),
                Color.FromArgb(90, 90, 90));
            btnGrayscale.Click += BtnGrayscale_Click;

            btnCanny = MakeButton("🔍 엣지 검출", new Point(160, 70),
                Color.FromArgb(150, 80, 0));
            btnCanny.Click += BtnCanny_Click;

            // 블러 슬라이더
            lblBlurTitle = MakeLabel("블러 강도", new Point(460, 15));
            lblBlurValue = MakeLabel("0", new Point(700, 15));
            lblBlurValue.ForeColor = Color.LightBlue;

            trkBlur = new TrackBar
            {
                Location = new Point(540, 5),
                Size = new Size(155, 50),
                Minimum = 0,
                Maximum = 20,
                Value = 0,
                TickFrequency = 2,
                BackColor = Color.FromArgb(42, 42, 42)
            };
            trkBlur.ValueChanged += TrkBlur_ValueChanged;

            // 색상 추출
            lblColorTitle = MakeLabel("색상 추출", new Point(460, 80));
            cmbColor = new ComboBox
            {
                Location = new Point(540, 75),
                Size = new Size(90, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(55, 55, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cmbColor.Items.AddRange(new object[] { "Red", "Green", "Blue" });
            cmbColor.SelectedIndex = 0;

            btnApplyColor = MakeButton("🎨 적용", new Point(640, 68),
                Color.FromArgb(100, 0, 140));
            btnApplyColor.Size = new Size(90, 40);
            btnApplyColor.Click += BtnApplyColor_Click;

            // 패널에 컨트롤 추가
            panelControls.Controls.AddRange(new Control[]
            {
                btnLoad, btnReset, btnSave,
                btnGrayscale, btnCanny,
                lblBlurTitle, trkBlur, lblBlurValue,
                lblColorTitle, cmbColor, btnApplyColor
            });

            // 폼에 추가
            this.Controls.AddRange(new Control[]
            {
                lblOriginal, lblProcessed,
                picOriginal, picProcessed,
                panelControls
            });
        }

        
        // 이벤트 핸들러
        
        private void BtnLoad_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "이미지 선택"
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            _originalMat?.Dispose();
            _currentMat?.Dispose();

            _currentFileName = Path.GetFileName(ofd.FileName);
            _originalMat = _imageService.LoadImage(ofd.FileName);
            _currentMat = _originalMat.Clone();

            DisplayImage(picOriginal!, _originalMat);
            DisplayImage(picProcessed!, _currentMat);

            trkBlur!.Value = 0;
            _options = ProcessingOptions.Default;
            _logService.AddLog(new ImageLog(_currentFileName, "load", "-"));
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            if (_originalMat == null) return;

            _currentMat?.Dispose();
            _currentMat = _originalMat.Clone();
            DisplayImage(picProcessed!, _currentMat);
            trkBlur!.Value = 0;
            _options = ProcessingOptions.Default;

            _logService.AddLog(new ImageLog(_currentFileName, "reset", "-"));
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (_currentMat == null) return;

            using SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PNG|*.png|JPEG|*.jpg",
                FileName = "processed_" + _currentFileName
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            _imageService.SaveImage(_currentMat, sfd.FileName);
            _logService.AddLog(new ImageLog(_currentFileName, "save", sfd.FileName));
            MessageBox.Show("저장 완료!", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TrkBlur_ValueChanged(object? sender, EventArgs e)
        {
            if (_originalMat == null) return;

            _options.BlurStrength = trkBlur!.Value;
            lblBlurValue!.Text = _options.BlurStrength.ToString();

            _currentMat?.Dispose();
            _currentMat = _options.BlurStrength == 0
                ? _originalMat.Clone()
                : _filterService.ApplyBlur(_originalMat, _options.BlurStrength);

            DisplayImage(picProcessed!, _currentMat);
            _logService.AddLog(new ImageLog(_currentFileName, "blur", _options.BlurStrength.ToString()));
        }

        private void BtnGrayscale_Click(object? sender, EventArgs e)
        {
            if (_originalMat == null) return;

            _currentMat?.Dispose();
            _currentMat = _filterService.ApplyGrayscale(_originalMat);
            DisplayImage(picProcessed!, _currentMat);

            _logService.AddLog(new ImageLog(_currentFileName, "grayscale", "-"));
        }

        private void BtnCanny_Click(object? sender, EventArgs e)
        {
            if (_originalMat == null) return;

            _currentMat?.Dispose();
            _currentMat = _filterService.ApplyCanny(
                _originalMat,
                _options.CannyThreshold1,
                _options.CannyThreshold2
            );
            DisplayImage(picProcessed!, _currentMat);

            _logService.AddLog(new ImageLog(
                _currentFileName, "canny_edge",
                $"{_options.CannyThreshold1}/{_options.CannyThreshold2}"
            ));
        }

        private void BtnApplyColor_Click(object? sender, EventArgs e)
        {
            if (_originalMat == null) return;

            _options.ColorTarget = cmbColor!.SelectedItem!.ToString()!.ToLower();
            _currentMat?.Dispose();
            _currentMat = _filterService.ApplyColorFilter(_originalMat, _options.ColorTarget);
            DisplayImage(picProcessed!, _currentMat);

            _logService.AddLog(new ImageLog(_currentFileName, "color_filter", _options.ColorTarget));
        }

        
        // 헬퍼 메서드
        
        private void DisplayImage(PictureBox pb, Mat mat)
        {
            pb.Image?.Dispose();
            pb.Image = BitmapConverter.ToBitmap(mat);
        }

        private Label MakeLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Location = location,
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                BackColor = Color.Transparent
            };
        }

        private PictureBox MakePictureBox(Point location)
        {
            return new PictureBox
            {
                Location = location,
                Size = new Size(565, 460),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(45, 45, 45),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Button MakeButton(string text, Point location, Color backColor)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(140, 40),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("맑은 고딕", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        // 폼 닫을 때 Mat 메모리 반드시 해제
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _originalMat?.Dispose();
            _currentMat?.Dispose();
            base.OnFormClosing(e);
        }
    }
}