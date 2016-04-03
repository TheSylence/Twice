using MaterialDesignThemes.Wpf;
using Twice.Models.Columns;

namespace Twice.ViewModels.ColumnManagement
{
	internal class ColumnTypeItem
	{
		public ColumnTypeItem( ColumnType type, string name, string description, PackIconKind icon )
		{
			Type = type;
			Icon = icon;
			DisplayName = name;
			Description = description;
		}

		public string Description { get; }
		public string DisplayName { get; }
		public PackIconKind Icon { get; }
		public ColumnType Type { get; }
	}
}