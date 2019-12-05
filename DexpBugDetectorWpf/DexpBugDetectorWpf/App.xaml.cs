using System;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace DexpBugDetectorWpf
{
	public partial class App : Application
	{
		private NotifyIcon notifyIcon = new NotifyIcon();

		public App()
		{
			this.notifyIcon.Icon = DexpBugDetectorWpf.Properties.Resources.tv;
			this.notifyIcon.Visible = true;
			this.notifyIcon.Text = "DexpBugDetectorWpf";

			this.notifyIcon.ContextMenuStrip = new ContextMenuStrip();
			ToolStripItem quitItem = this.notifyIcon.ContextMenuStrip.Items.Add("Выйти");
			quitItem.Click += this.quitItem_Click;
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
