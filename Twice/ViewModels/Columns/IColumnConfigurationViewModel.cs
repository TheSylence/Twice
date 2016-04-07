using System;
using System.Windows.Input;
using Twice.Models.Columns;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnConfigurationViewModel
	{
		event EventHandler Saved;
		
		ColumnDefinition Definition { get; }
		bool IsExpanded { get; set; }

		bool PopupEnabled { get; set; }
		ICommand SaveCommand { get; }

		bool SoundEnabled { get; set; }
		bool ToastsEnabled { get; set; }
	}
}