namespace Twice.ViewModels.Columns.Definitions
{
	internal class MessagesColumnDefinition : ColumnDefinition
	{
		public MessagesColumnDefinition( ulong accountId )
			: base( ColumnType.Messages )
		{
			AccountId = accountId;
		}

		public ulong AccountId { get; set; }
	}
}