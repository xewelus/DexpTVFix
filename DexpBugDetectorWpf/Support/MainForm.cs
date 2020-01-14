using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Common;
using static System.Drawing.Bitmap;

namespace Support
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		[DllImport("gdi32.dll")]
		static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
			wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
		[DllImport("user32.dll")]
		static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
		[DllImport("gdi32.dll")]
		static extern IntPtr DeleteDC(IntPtr hDc);
		[DllImport("gdi32.dll")]
		static extern IntPtr DeleteObject(IntPtr hDc);
		[DllImport("gdi32.dll")]
		static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
		[DllImport("gdi32.dll")]
		static extern IntPtr CreateCompatibleDC(IntPtr hdc);
		[DllImport("gdi32.dll")]
		static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr ptr);

		private bool isStarted;
		private DateTime? startDate;
		private int captureCount;
		private void btnCapture_Click(object sender, EventArgs e)
		{
			try
			{
				CaptureScreen();
			}
			catch (Exception ex)
			{
				UIHelper.ShowError(ex);
			}
		}

		private void btnStart1_Click(object sender, EventArgs e)
		{
			this.isStarted = !this.isStarted;
			if (this.isStarted)
			{
				this.btnStart1.Text = "Стоп";
				this.startDate = DateTime.Now;
				this.captureCount = 0;
				this.timer.Start();
			}
			else
			{
				this.timer.Stop();
				this.startDate = null;
				this.btnStart1.Text = "Старт";
			}
		}

		private void btnStart2_Click(object sender, EventArgs e)
		{

		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (this.isStarted)
			{
				CaptureScreen();
				this.captureCount++;

				if (this.startDate != null)
				{
					double secs = DateTime.Now.Subtract(this.startDate.Value).TotalSeconds;
					double speed = this.captureCount / secs;
					this.lblStatus.Text = string.Format("Всего: {0}, скорость: {1:0} в секунду.", this.captureCount, speed);
				}
			}
		}

		private static void CaptureScreen()
		{
			CaptureScreen(SaveBitmap);
		}

		private static void SaveBitmap(Bitmap bitmap)
		{
			bitmap.Save(string.Format(@"c:\temp\capture\{0:yyyyMMdd HH mm ss ffffff}.png", DateTime.Now));
		}

		private static void CaptureScreen(Action<Bitmap> action, bool needDispose = true)
		{
			IntPtr hDesk = IntPtr.Zero;
			IntPtr hSrce = IntPtr.Zero;
			IntPtr hDest = IntPtr.Zero;
			IntPtr hBmp = IntPtr.Zero;
			IntPtr hOldBmp = IntPtr.Zero;

			try
			{
				Size sz = Screen.PrimaryScreen.Bounds.Size;
				hDesk = GetDesktopWindow();
				if (hDesk == IntPtr.Zero) throw new Exception("hDesk");

				hSrce = GetWindowDC(hDesk);
				if (hSrce == IntPtr.Zero) throw new Exception("hSrce");

				hDest = CreateCompatibleDC(hSrce);
				if (hDest == IntPtr.Zero) throw new Exception("hDest");

				hBmp = CreateCompatibleBitmap(hSrce, sz.Width, sz.Height);
				if (hBmp == IntPtr.Zero) throw new Exception("hBmp");

				hOldBmp = SelectObject(hDest, hBmp);
				if (hOldBmp == IntPtr.Zero) throw new Exception("hOldBmp");

				bool result = BitBlt(hDest, 0, 0, sz.Width, sz.Height, hSrce, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
				if (!result) throw new Exception("BitBlt");

				Bitmap bmp = null;
				try
				{
					bmp = Image.FromHbitmap(hBmp);
					if (action != null)
					{
						action.Invoke(bmp);
					}
				}
				finally
				{
					if (needDispose && bmp != null)
					{
						bmp.Dispose();
					}
				}
			}
			finally
			{
				if (hOldBmp != IntPtr.Zero)
				{
					SelectObject(hDest, hOldBmp);
				}
				if (hBmp != IntPtr.Zero)
				{
					DeleteObject(hBmp);
				}
				if (hDest != IntPtr.Zero)
				{
					DeleteDC(hDest);
				}
				if (hDesk != IntPtr.Zero)
				{
					ReleaseDC(hDesk, hSrce);
				}
			}
		}
	}
}
