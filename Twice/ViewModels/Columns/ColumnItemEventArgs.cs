using System;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class ColumnItemEventArgs : EventArgs
	{
		public ColumnItemEventArgs( ColumnItem item )
		{
			Item = item;
		}

		public readonly ColumnItem Item;
	}
}