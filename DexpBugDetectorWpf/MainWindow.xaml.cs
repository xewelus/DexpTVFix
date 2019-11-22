using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DexpBugDetectorWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			this.Setup();
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

			Setup();
			Thread.Sleep(2000);
			Setup();
		}

		private void Setup()
		{
			this.Topmost = true;
			this.Left = 0;
			this.Top = 0;
			this.Width = 1920;
			this.Height = 1;

			//this.Background = new SolidColorBrush(Color.FromArgb(128, 255, 121, 44));
			//this.Opacity = 0.5;
		}
	}
}
