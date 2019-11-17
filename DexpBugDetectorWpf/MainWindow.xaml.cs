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

			this.Deactivated += this.MainWindow_Deactivated;
		}

		private void MainWindow_Deactivated(object sender, EventArgs e)
		{
			Setup();
			Thread.Sleep(2000);
			Setup();
		}

		private void Setup()
		{
			this.Topmost = true;
			this.Left = 0;
			this.Top = 0;
			this.Width = 0.1;
			this.Height = 0.1;

			//this.AllowsTransparency = true;
			//this.Opacity = 0.5;
		}
	}
}
