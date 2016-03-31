namespace Twice.ViewModels.Columns.Definitions
{
	internal static class ColumnDefinitionFactory
	{
		internal static ColumnDefinition Construct( ColumnType type, ulong[] sourceAccounts, ulong[] targetAccounts )
		{
			return new ColumnDefinition( type )
			{
				SourceAccounts = sourceAccounts,
				TargetAccounts = targetAccounts
			};
		}
	}
}