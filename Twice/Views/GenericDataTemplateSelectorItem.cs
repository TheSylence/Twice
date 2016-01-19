using System;
using System.Windows;
using System.Windows.Markup;

namespace Twice.Views
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[ContentProperty( "Template" )]
	public class GenericDataTemplateSelectorItem
	{
		public DataTemplate Template { get; set; }

		public Type TemplatedType { get; set; }
	}
}