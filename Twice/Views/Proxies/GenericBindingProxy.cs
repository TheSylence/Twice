using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Seal.Fody;

namespace Twice.Views.Proxies
{
	[ExcludeFromCodeCoverage]
	[LeaveUnsealed]
	internal class GenericBindingProxy<TData> : Freezable
		where TData : class
	{
		protected override Freezable CreateInstanceCore()
		{
			return new GenericBindingProxy<TData>();
		}

		public static readonly DependencyProperty DataProperty = DependencyProperty.Register( "Data", typeof(TData),
			typeof(GenericBindingProxy<TData>),
			new UIPropertyMetadata( null ) );

		public TData Data
		{
			get { return (TData)GetValue( DataProperty ); }
			set { SetValue( DataProperty, value ); }
		}
	}
}