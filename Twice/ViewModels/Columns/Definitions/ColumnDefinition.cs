namespace Twice.ViewModels.Columns.Definitions
{
	abstract class ColumnDefinition
	{
		protected ColumnDefinition( ColumnType type )
		{
			Type = type;
			Width = 300;
		}

		public ColumnType Type { get; set; }
		public int Width { get; set; }
	}
}
