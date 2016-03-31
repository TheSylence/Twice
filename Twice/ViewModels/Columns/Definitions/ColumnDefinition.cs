namespace Twice.ViewModels.Columns.Definitions
{
	class ColumnDefinition
	{
		public ColumnDefinition( ColumnType type )
		{
			Type = type;
			Width = 300;
		}

		public ColumnType Type { get; set; }
		public int Width { get; set; }
		public ulong[] TargetAccounts { get; set; }
		public ulong[] SourceAccounts { get; set; }
	}
}
