using System;

namespace Twice.Models.Columns
{
	internal static class ColumnDefinitionFactory
	{
		internal static ColumnDefinition Construct( ColumnType type, ulong[] sourceAccounts, ulong[] targetAccounts )
		{
			return new ColumnDefinition( type )
			{
				Id = Guid.NewGuid(),
				SourceAccounts = sourceAccounts,
				TargetAccounts = targetAccounts
			};
		}
	}
}