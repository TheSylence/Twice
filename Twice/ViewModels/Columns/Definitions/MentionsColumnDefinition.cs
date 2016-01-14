namespace Twice.ViewModels.Columns.Definitions
{
	internal class MentionsColumnDefinition : ColumnDefinition
	{
		public MentionsColumnDefinition( ulong[] accountIds )
			: base( ColumnType.Mentions )
		{
			AccountIds = accountIds;
		}

		public ulong[] AccountIds { get; set; }
	}
}