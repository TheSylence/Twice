using System;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Attributes
{
	[ExcludeFromCodeCoverage]
	internal class LocalizeKeyAttribute : Attribute
	{
		public LocalizeKeyAttribute( string key = null )
		{
			Key = key;
		}

		public string Key { get; }
	}
}