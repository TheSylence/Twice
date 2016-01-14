using System.Windows;
using System.Windows.Interactivity;
using Twice.ViewModels.Main;

namespace Twice.Behaviors
{
	internal class CallbackOnLoad : Behavior<Window>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.Loaded += async ( s, e ) =>
			{
				if( Callback != null )
				{
					await Callback.OnLoad();
				}
			};
		}

		public ILoadCallback Callback
		{
			get { return (ILoadCallback)GetValue( CallbackProperty ); }
			set { SetValue( CallbackProperty, value ); }
		}

		public static readonly DependencyProperty CallbackProperty = DependencyProperty.Register( "Callback", typeof( ILoadCallback ), typeof( CallbackOnLoad ),
			new PropertyMetadata( null ) );
	}
}