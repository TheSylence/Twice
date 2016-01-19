using System.Windows;
using System.Windows.Interactivity;
using Twice.ViewModels.Main;

namespace Twice.Behaviors
{
	internal class CallbackOnLoad : Behavior<FrameworkElement>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.Loaded += async ( s, e ) =>
			{
				if( Callback != null )
				{
					await Callback.OnLoad( Data );
				}
			};
		}

		public ILoadCallback Callback
		{
			get { return (ILoadCallback)GetValue( CallbackProperty ); }
			set { SetValue( CallbackProperty, value ); }
		}

		public object Data
		{
			get { return (object)GetValue( DataProperty ); }
			set { SetValue( DataProperty, value ); }
		}

		public static readonly DependencyProperty CallbackProperty = DependencyProperty.Register( "Callback", typeof( ILoadCallback ), typeof( CallbackOnLoad ),
			new PropertyMetadata( null ) );

		public static readonly DependencyProperty DataProperty =
					DependencyProperty.Register( "Data", typeof( object ), typeof( CallbackOnLoad ), new PropertyMetadata( null ) );
	}
}