namespace Twice.ViewModels.Columns.Definitions
{
	internal class ActivityColumnDefintion : ColumnDefinition
	{
		public ActivityColumnDefintion( ulong[] accountIds )
			: base( ColumnType.Activity )
		{
			AccountIds = accountIds;
		}

		public ulong[] AccountIds { get; set; }
	}
}