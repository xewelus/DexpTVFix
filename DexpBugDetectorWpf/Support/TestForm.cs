using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common;
using Common.Classes;
using static System.Drawing.Bitmap;

namespace Support
{
	public partial class TestForm : Form
	{
		public TestForm()
		{
			this.InitializeComponent();
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

		private BaseMode mode;
		private bool isStarted;

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

		private void StartStopClick(Type modeType, Button currentButton)
		{
			this.isStarted = !this.isStarted;

			Button[] buttons =
			{
				this.btnStart1,
				this.btnStart2
			};

			if (this.isStarted)
			{
				currentButton.Tag = currentButton.Text;

				this.mode = (BaseMode)Activator.CreateInstance(modeType, this);
				this.mode.Start();
				currentButton.Text = "Стоп";

				foreach (Button button in buttons)
				{
					if (button != currentButton)
					{
						button.Enabled = false;
					}
				}

				this.timer.Start();
			}
			else
			{
				this.timer.Stop();

				if (this.mode != null)
				{
					this.mode.Stop();
					this.mode = null;
				}

				currentButton.Text = (string)currentButton.Tag;

				foreach (Button button in buttons)
				{
					button.Enabled = true;
				}
			}
		}

		private void btnStart1_Click(object sender, EventArgs e)
		{
			this.StartStopClick(typeof(Mode1), (Button)sender);
		}
	
		private void btnStart2_Click(object sender, EventArgs e)
		{
			this.StartStopClick(typeof(Mode2), (Button)sender);
		}

		private void btnStart3_Click(object sender, EventArgs e)
		{
			this.StartStopClick(typeof(Mode3), (Button)sender);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (this.mode != null)
			{
				this.mode.TimerTick();
			}
		}

		private void ShowError(Exception error)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new Action<Exception>(this.ShowError), error);
				return;
			}

			UIHelper.ShowError(error);
		}

		private static void CaptureScreen()
		{
			CaptureScreen(SaveBitmap);
		}

		private static void SaveBitmap(Bitmap bitmap)
		{
			SaveBitmap(bitmap, DateTime.Now);
		}

		private static void SaveBitmap(Bitmap bitmap, DateTime? time)
		{
			if (time == null)
			{
				time = DateTime.Now;
			}
			bitmap.Save(string.Format(@"c:\temp\capture\{0:yyyyMMdd HH mm ss ffffff}.png", time.Value));
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

		private class BaseMode
		{
			protected readonly TestForm form;
			protected BaseMode(TestForm form)
			{
				this.form = form;
			}

			public virtual void TimerTick()
			{
			}

			public virtual void Start()
			{
			}

			public virtual void Stop()
			{
			}
		}

		private class Mode1 : BaseMode
		{
			private readonly DateTime startDate = DateTime.Now;
			private int captureCount;

			public Mode1(TestForm form) : base(form)
			{
			}

			public override void TimerTick()
			{
				CaptureScreen();
				this.captureCount++;

				double secs = DateTime.Now.Subtract(this.startDate).TotalSeconds;
				double speed = this.captureCount / secs;
				this.form.lblStatus.Text = string.Format("Всего: {0}, скорость: {1:0} в секунду.", this.captureCount, speed);
			}
		}

		private class Mode2 : BaseMode
		{
			private readonly DateTime startDate = DateTime.Now;
			private int captureCount;
			private int savedCount;
			private readonly PairsList<Bitmap, DateTime> toSave = new PairsList<Bitmap, DateTime>();
			private bool isComplete;

			public Mode2(TestForm form) : base(form)
			{
				Thread thread = new Thread(this.ThreadMethod);
				thread.Start();
			}

			private void ThreadMethod()
			{
				try
				{
					while (!this.isComplete || this.toSave.Count > 0)
					{
						KeyValuePair<Bitmap, DateTime>? pair = null;
						lock (this.toSave)
						{
							if (this.toSave.Count > 0)
							{
								pair = this.toSave[0];
								this.toSave.RemoveAt(0);
							}
						}

						if (pair == null)
						{
							Thread.Sleep(10);
							continue;
						}

						Bitmap bitmap = pair.Value.Key;
						SaveBitmap(bitmap, pair.Value.Value);
						bitmap.Dispose();
						this.savedCount++;

						this.UpdateStatus();
					}
				}
				catch (Exception ex)
				{
					this.form.ShowError(ex);
				}
			}

			public override void TimerTick()
			{
				CaptureScreen(this.AddToList, false);
				this.captureCount++;

				this.UpdateStatus();
			}

			private void AddToList(Bitmap bitmap)
			{
				lock (this.toSave)
				{
					this.toSave.Add(bitmap, DateTime.Now);
				}
			}

			private void UpdateStatus()
			{
				if (this.form.InvokeRequired)
				{
					this.form.BeginInvoke(new Action(this.UpdateStatus));
					return;
				}

				double secs = DateTime.Now.Subtract(this.startDate).TotalSeconds;
				double speed = this.captureCount / secs;
				this.form.lblStatus.Text = string.Format("Всего: {0}, сохранено: {1}, скорость: {2:0} в секунду.", this.captureCount, this.savedCount, speed);
			}

			public override void Stop()
			{
				base.Stop();
				this.isComplete = true;
			}
		}

		private class Mode3 : BaseMode
		{
			private readonly DateTime startDate = DateTime.Now;
			private int captureCount;
			private int savedCount;
			private readonly PairsList<Bitmap, DateTime> toSave = new PairsList<Bitmap, DateTime>();
			private bool isComplete;

			public Mode3(TestForm form) : base(form)
			{
				Thread saveThread = new Thread(this.SaveThreadMethod);
				saveThread.Start();

				int count = (int)form.nbThreads.Value;
				for (int i = 0; i < count; i++)
				{
					Thread captureThread = new Thread(this.CaptureThreadMethod);
					captureThread.Start();
				}
			}

			private void CaptureThreadMethod()
			{
				try
				{
					while (!this.isComplete)
					{
						CaptureScreen(this.AddToList, false);

						lock (this)
						{
							this.captureCount++;
						}

						this.UpdateStatus();
					}
				}
				catch (Exception ex)
				{
					this.form.ShowError(ex);
				}
			}

			private void SaveThreadMethod()
			{
				try
				{
					while (!this.isComplete || this.toSave.Count > 0)
					{
						KeyValuePair<Bitmap, DateTime>? pair = null;
						lock (this.toSave)
						{
							if (this.toSave.Count > 0)
							{
								pair = this.toSave[0];
								this.toSave.RemoveAt(0);
							}
						}

						if (pair == null)
						{
							Thread.Sleep(10);
							continue;
						}

						Bitmap bitmap = pair.Value.Key;
						SaveBitmap(bitmap, pair.Value.Value);
						bitmap.Dispose();
						this.savedCount++;

						this.UpdateStatus();
					}
				}
				catch (Exception ex)
				{
					this.form.ShowError(ex);
				}
			}

			private void AddToList(Bitmap bitmap)
			{
				lock (this.toSave)
				{
					this.toSave.Add(bitmap, DateTime.Now);
				}
			}

			private void UpdateStatus()
			{
				if (this.form.InvokeRequired)
				{
					this.form.BeginInvoke(new Action(this.UpdateStatus));
					return;
				}

				double secs = DateTime.Now.Subtract(this.startDate).TotalSeconds;
				double speed = this.captureCount / secs;
				this.form.lblStatus.Text = string.Format("Всего: {0}, сохранено: {1}, скорость: {2:0} в секунду.", this.captureCount, this.savedCount, speed);
			}

			public override void Stop()
			{
				base.Stop();
				this.isComplete = true;
			}
		}
	}
}
