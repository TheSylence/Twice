using System;
using MaterialDesignThemes.Wpf;

namespace Twice.Attributes
{
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