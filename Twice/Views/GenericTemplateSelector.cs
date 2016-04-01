using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Twice.Views
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[ContentProperty( "Templates" )]
	public class GenericTemplateSelector : DataTemplateSelector
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
			Justification = "Bullshit" )]
		public override DataTemplate SelectTemplate( object item, DependencyObject container )
		{
			if( item == null )
			{
				return null;
			}

			foreach( GenericDataTemplateSelectorItem tpl in Templates )
			{
				if( tpl.TemplatedType != null && IsDerived( item.GetType(), tpl.TemplatedType ) )
				{
					return tpl.Template;
				}
			}

			return null;
		}

		private static bool IsDerived( Type itemType, Type baseType )
		{
			if( itemType == baseType )
			{
				return true;
			}

			return itemType.BaseType != null && IsDerived( itemType.BaseType, baseType );
		}

		public Collection<GenericDataTemplateSelectorItem> Templates { get; set; } = new Collection<GenericDataTemplateSelectorItem>();
	}
}