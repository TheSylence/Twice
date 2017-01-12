using GalaSoft.MvvmLight;
using System.Diagnostics;

namespace Twice.ViewModels
{
	internal class ItemSelection<TContent> : ObservableObject
	{
		public ItemSelection( TContent content, bool selected = false )
		{
			Content = content;
			IsSelected = selected;
		}

		public TContent Content { get; }

		public bool IsSelected
		{
			[DebuggerStepThrough]
			get { return _IsSelected; }
			set
			{
				if( _IsSelected == value )
				{
					return;
				}

				_IsSelected = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsSelected;
	}
}