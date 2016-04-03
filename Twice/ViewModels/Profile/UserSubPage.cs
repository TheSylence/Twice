using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Twice.ViewModels.Profile
{
	class UserSubPage : ObservableObject
	{
		public UserSubPage( string title, Func<Task<IEnumerable<object>>> loadAction, int count )
		{
			Title = title;
			Count = count;
			LoadAction = loadAction;
		}

		public int Count { get; }

		public bool IsLoading
		{
			[DebuggerStepThrough]
			get
			{
				return _IsLoading;
			}

			set
			{
				if( _IsLoading == value )
				{
					return;
				}

				_IsLoading = value;
				RaisePropertyChanged( nameof( IsLoading ) );
			}
		}

		public List<object> Items
		{
			get
			{
				if( _Items == null )
				{
					IsLoading = true;
					Task.Run( async () =>
					{
						_Items = new List<object>( await LoadAction() );
						RaisePropertyChanged( nameof( Items ) );
					} ).ContinueWith( t =>
					{
						IsLoading = false;
					} );
				}

				return _Items;
			}
		}

		public string Title { get; private set; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsLoading;

		private List<object> _Items;

		private readonly Func<Task<IEnumerable<object>>> LoadAction;
	}
}