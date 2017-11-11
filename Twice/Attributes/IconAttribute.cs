using System;
using System.Diagnostics.CodeAnalysis;
using MaterialDesignThemes.Wpf;

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