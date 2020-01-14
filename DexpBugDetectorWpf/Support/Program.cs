using System;
using System.Threading;
using System.Windows.Forms;
using Common;

namespace Support
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.ThreadException += ApplicationOnThreadException;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		private static void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs e)
		{
			if (e.Exception == null)
			{
				UIHelper.ShowError("Unknown error.");
			}
			else
			{
				UIHelper.ShowError(e.Exception);
			}
		}
	}
}
