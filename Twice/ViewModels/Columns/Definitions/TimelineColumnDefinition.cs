namespace Twice.ViewModels.Columns.Definitions
{
	class TimelineColumnDefinition : ColumnDefinition
	{
		public TimelineColumnDefinition( ulong[] accountIds )
			: base( ColumnType.Timeline )
		{
			AccountIds = accountIds;
		}

		public ulong[] AccountIds { get; set; }
	}
}