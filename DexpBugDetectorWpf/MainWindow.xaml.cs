using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DexpBugDetectorWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			DrawingVisual drawingVisual = CreateDrawingVisualRectangle(out this.maxHeight);
			DrawingBrush drawingBrush = new DrawingBrush(drawingVisual.Drawing);
			drawingBrush.Stretch = Stretch.None;
			drawingBrush.AlignmentX = AlignmentX.Left;
			drawingBrush.AlignmentY = AlignmentY.Top;
			this.Background = drawingBrush;

			this.Setup();
		}

		private readonly int maxHeight;

		private static DrawingVisual CreateDrawingVisualRectangle(out int heigth)
		{
			heigth = 0;

			DrawingVisual drawingVisual = new DrawingVisual();

			using (DrawingContext dc = drawingVisual.RenderOpen())
			{
				foreach (Configuration.RectInfo rectInfo in Configuration.Rects)
				{
					Rect rect = new Rect(rectInfo.X, rectInfo.Y, rectInfo.Width, rectInfo.Height);
					dc.DrawRectangle(new SolidColorBrush(rectInfo.Color), null, rect);

					heigth = Math.Max(heigth, rectInfo.Height);
				}
			}

			return drawingVisual;
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
			WindowsServices.SetWindowExTransparent(windowInteropHelper.Handle);
		}

		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);
			this.Setup();
		}

		private void Setup()
		{
			this.Topmost = true;
			this.Left = 0;
			this.Top = 0;
			this.Width = 1920;
			this.Height = this.maxHeight;
		}
	}
}
