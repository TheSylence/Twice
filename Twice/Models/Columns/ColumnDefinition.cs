namespace Twice.Models.Columns
{
	internal class ColumnDefinition
	{
		public ColumnDefinition( ColumnType type )
		{
			Type = type;
			Width = 300;
			Notifications = new ColumnNotifications();
		}

		public ColumnNotifications Notifications { get; set; }
		public ulong[] SourceAccounts { get; set; }
		public ulong[] TargetAccounts { get; set; }
		public ColumnType Type { get; set; }
		public int Width { get; set; }
	}
}