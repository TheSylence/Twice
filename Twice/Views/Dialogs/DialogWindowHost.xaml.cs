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
				ViewController.CloseRequested -= ViewController_CloseRequested;
			}

			base.OnContentChanged( oldContent, newContent );

			var frameworkContent = newContent as FrameworkElement;

			var dataContext = frameworkContent?.DataContext;
			ViewController = dataContext as IViewController;
			if( ViewController != null )
			{
				ViewController.CenterRequested += ViewController_CenterRequested;
				ViewController.CloseRequested += ViewController_CloseRequested;
			}
		}

		protected override void OnInitialized( EventArgs e )
		{
			base.OnInitialized( e );

			Center();
		}

		private void CenterButton_Click( object sender, RoutedEventArgs e )
		{
			Center();
		}

		private void ViewController_CenterRequested( object sender, EventArgs e )
		{
			Center();
		}

		private void ViewController_CloseRequested( object sender, CloseEventArgs e )
		{
			Close();
		}

		private IViewController ViewController;
	}
}