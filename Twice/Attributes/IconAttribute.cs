using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Attributes
{
	[ExcludeFromCodeCoverage]
	[AttributeUsage( AttributeTargets.Field )]
	internal class IconAttribute : Attribute
	{
		public IconAttribute( PackIconKind kind )
		{
			Kind = kind;
		}

		public readonly PackIconKind Kind;
	}
}