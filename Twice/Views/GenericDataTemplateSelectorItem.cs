using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;

namespace Twice.Views
{
	[ExcludeFromCodeCoverage]
	[ContentProperty( "Template" )]
	public class GenericDataTemplateSelectorItem
	{
		public DataTemplate Template { get; set; }

		public Type TemplatedType { get; set; }
	}
}