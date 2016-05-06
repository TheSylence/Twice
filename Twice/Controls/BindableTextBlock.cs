using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Twice.Controls
{
	/// <summary>
	///     A TextBlock that exposes it's Inlines property as a dependency property called Elements.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class BindableTextBlock : TextBlock
	{
		/// <summary>
		///     Called when the Elements property changed its value.
		/// </summary>
		/// <param name="d">The object on which the property was chagned.</param>
		/// <param name="e">
		///     The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.
		/// </param>
		private static void OnElementsChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			BindableTextBlock txt = d as BindableTextBlock;
			if( txt != null )
			{
				IEnumerable<Inline> oldInlines = e.OldValue as IEnumerable<Inline>;
				IEnumerable<Inline> newInlines = e.NewValue as IEnumerable<Inline>;

				if( oldInlines != null )
				{
					foreach( Inline inline in oldInlines )
					{
						txt.Inlines.Remove( inline );
					}
				}

				if( newInlines != null )
				{
					foreach( Inline inline in newInlines )
					{
						txt.Inlines.Add( inline );
					}
				}
			}
		}

		public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register( "Elements",
			typeof(IEnumerable<Inline>), typeof(BindableTextBlock), new PropertyMetadata( null, OnElementsChanged ) );

		/// <summary>
		///     Collection of Inlines this TextBox uses..
		/// </summary>
		public IEnumerable<Inline> Elements
		{
			get { return (IEnumerable<Inline>)GetValue( ElementsProperty ); }
			set { SetValue( ElementsProperty, value ); }
		}
	}
}