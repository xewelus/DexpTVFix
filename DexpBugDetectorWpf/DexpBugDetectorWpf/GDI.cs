using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DexpBugDetectorWpf
{
	class GDI
	{
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
		static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		static extern IntPtr GetWindowDC(IntPtr ptr);

		public static void CaptureScreen(Action<Bitmap> action, bool needDispose = true)
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
