using System;
using System.Windows;
using Twice.Behaviors;

namespace Twice.Views.Dialogs
{
	/// <summary>
	///  Interaction logic for DialogWindowHost.xaml 
	/// </summary>
	public partial class DialogWindowHost
	{
		public DialogWindowHost()
		{
			InitializeComponent();
		}

		public void Center()
		{
			WindowHelper.Center( this );
		}

		protected override void OnInitialized( EventArgs e )
		{
			base.OnInitialized( e );

			Center();
		}

		private void CenterButton_Click( object sender, RoutedEventArgs e )
		{
			WindowHelper.Center( this );
		}
	}
}