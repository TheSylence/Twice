namespace Twice.ViewModels.Columns.Definitions
{
	class UserColumnDefintion : ColumnDefinition
	{
		public UserColumnDefintion( ulong userId )
			: base(ColumnType.User)
		{
			UserId = userId;
		}

		public ulong UserId { get; set; }
	}
}