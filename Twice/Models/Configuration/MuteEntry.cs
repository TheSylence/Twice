using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;

namespace Twice.Models.Configuration
{
	internal class MuteEntry : ObservableObject
	{
		public DateTime? EndDate
		{
			[DebuggerStepThrough] get { return _EndDate; }
			set
			{
				if( _EndDate == value )
				{
					return;
				}

				_EndDate = value;
				RaisePropertyChanged();
				RaisePropertyChanged( nameof( HasEndDate ) );
			}
		}

		public string Filter
		{
			[DebuggerStepThrough] get { return _Filter; }
			set
			{
				if( _Filter == value )
				{
					return;
				}

				_Filter = value;
				RaisePropertyChanged();
			}
		}

		public bool HasEndDate
		{
			[DebuggerStepThrough] get { return _EndDate.HasValue; }
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private DateTime? _EndDate;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Filter;
	}
}