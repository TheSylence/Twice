using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Twice.ViewModels.Main;

namespace Twice.Behaviors
{
	/// <summary>
	///     Offers the ability to execute async code when a FrameworkElement has been loaded.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class CallbackOnLoad : BehaviorBase<FrameworkElement>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.Loaded += async ( s, e ) =>
			{
				if( Callback != null )
				{
					await Callback.OnLoad( Data ).ConfigureAwait( false );
				}
			};
		}

		public static readonly DependencyProperty CallbackProperty = DependencyProperty.Register( "Callback",
			typeof( ILoadCallback ), typeof( CallbackOnLoad ),
			new PropertyMetadata( null ) );

		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register( "Data", typeof( object ), typeof( CallbackOnLoad ), new PropertyMetadata( null ) );

		/// <summary>
		///     Handler to call when the element was loaded
		/// </summary>
		public ILoadCallback Callback
		{
			get => (ILoadCallback)GetValue( CallbackProperty );
			set => SetValue( CallbackProperty, value );
		}

		/// <summary>
		///     Data to pass to <see cref="ILoadCallback.OnLoad(object)" />
		/// </summary>
		public object Data
		{
			get => GetValue( DataProperty );
			set => SetValue( DataProperty, value );
		}
	}
}