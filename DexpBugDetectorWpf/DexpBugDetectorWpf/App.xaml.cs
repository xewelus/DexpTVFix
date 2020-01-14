using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace DexpBugDetectorWpf
{
	public partial class App : Application
	{
		private NotifyIcon notifyIcon = new NotifyIcon();
		private readonly ToolStripItem startRecordItem;
		private readonly ToolStripItem stopRecordItem;

		public App()
		{
			this.notifyIcon.Icon = DexpBugDetectorWpf.Properties.Resources.tv;
			this.notifyIcon.Visible = true;
			this.notifyIcon.Text = "DexpBugDetectorWpf";

			this.notifyIcon.ContextMenuStrip = new ContextMenuStrip();

			this.startRecordItem = this.notifyIcon.ContextMenuStrip.Items.Add("Начать запись");
			this.startRecordItem.Click += this.startRecordItem_Click;

			this.stopRecordItem = this.notifyIcon.ContextMenuStrip.Items.Add("Завершить запись");
			this.stopRecordItem.Click += this.stopRecordItem_Click;
			this.stopRecordItem.Enabled = false;

			this.notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

			ToolStripItem operFolderItem = this.notifyIcon.ContextMenuStrip.Items.Add("Открыть папку сохранения");
			operFolderItem.Click += this.operFolderItem_Click;

			this.notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

			ToolStripItem quitItem = this.notifyIcon.ContextMenuStrip.Items.Add("Выйти");
			quitItem.Click += this.quitItem_Click;
		}

		private void startRecordItem_Click(object sender, EventArgs eventArgs)
		{
			this.startRecordItem.Enabled = false;
			this.stopRecordItem.Enabled = true;
			Capturer.Start(this.OnCaptureComplete, this.OnCaptureError);
		}

		private void OnCaptureComplete(string folder)
		{
			if (this.Dispatcher != null && !this.Dispatcher.CheckAccess())
			{
				this.Dispatcher.Invoke(new Action<string>(this.OnCaptureComplete), folder);
				return;
			}

			this.startRecordItem.Enabled = true;
			this.stopRecordItem.Enabled = false;
			MessageBoxResult result = MessageBox.Show("Захват завершен. Открыть папку сохранения?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				Process.Start("explorer", folder);
			}
		}

		private void OnCaptureError(Exception exception)
		{
			if (this.Dispatcher != null && !this.Dispatcher.CheckAccess())
			{
				this.Dispatcher.Invoke(new Action<Exception>(this.OnCaptureError), exception);
				return;
			}
			MessageBox.Show(exception.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void stopRecordItem_Click(object sender, EventArgs eventArgs)
		{
			Capturer.Stop();
		}

		private void operFolderItem_Click(object sender, EventArgs eventArgs)
		{
			Process.Start("explorer", Capturer.GetCaptureFolder());
		}

		private void quitItem_Click(object sender, EventArgs eventArgs)
		{
			this.Shutdown();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			if (this.notifyIcon != null)
			{
				this.notifyIcon.Dispose();
				this.notifyIcon = null;
			}

			base.OnExit(e);
		}
	}
}
