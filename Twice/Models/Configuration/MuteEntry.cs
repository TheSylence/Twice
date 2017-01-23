using GalaSoft.MvvmLight;
using System;
using System.Diagnostics;

namespace Twice.Models.Configuration
{
	internal class MuteEntry : ObservableObject
	{
		public bool CaseSensitive
		{
			[DebuggerStepThrough]
			get { return _CaseSensitive; }
			set
			{
				if( _CaseSensitive == value )
				{
					return;
				}

				_CaseSensitive = value;
				RaisePropertyChanged();
			}
		}

		public DateTime? EndDate
		{
			[DebuggerStepThrough]
			get { return _EndDate; }
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
			[DebuggerStepThrough]
			get { return _Filter; }
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
			[DebuggerStepThrough]
			get { return _EndDate.HasValue; }
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _CaseSensitive;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private DateTime? _EndDate;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Filter;
	}
}