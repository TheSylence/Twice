using System;
using System.Windows;
using Twice.Behaviors;
using Twice.ViewModels;

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

		protected override void OnContentChanged( object oldContent, object newContent )
		{
			if( ViewController != null )
			{
				ViewController.CenterRequested -= ViewController_CenterRequested;
			}

			base.OnContentChanged( oldContent, newContent );

			var dataContext = ( newContent as FrameworkElement )?.DataContext;
			ViewController = dataContext as IViewController;
			if( ViewController != null )
			{
				ViewController.CenterRequested += ViewController_CenterRequested;
			}
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

		private void ViewController_CenterRequested( object sender, EventArgs e )
		{
			Center();
		}

		private IViewController ViewController;
	}
}