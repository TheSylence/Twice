using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Twitter.Entities;

namespace Twice.ViewModels.Twitter
{
	internal abstract class ColumnItem : ObservableObject, IHighlightable
	{
		public virtual void UpdateRelativeTime()
		{
			RaisePropertyChanged( nameof( CreatedAt ) );
		}

		private void ExecuteDismissSensibleWarningCommand()
		{
			HasSensibleContent = false;
		}

		public abstract DateTime CreatedAt { get; }

		public ICommand DismissSensibleWarningCommand => _DismissSensibleWarningCommand ?? ( _DismissSensibleWarningCommand = new RelayCommand(
			                                                 ExecuteDismissSensibleWarningCommand ) );

		public abstract Entities Entities { get; }

		public bool HasSensibleContent
		{
			[DebuggerStepThrough] get { return _HasSensibleContent; }
			protected set
			{
				if( _HasSensibleContent == value )
				{
					return;
				}

				_HasSensibleContent = value;
				RaisePropertyChanged();
			}
		}

		public abstract ulong Id { get; }

		public bool IsLoading
		{
			[DebuggerStepThrough] get { return _IsLoading; }
			set
			{
				if( _IsLoading == value )
				{
					return;
				}

				_IsLoading = value;
				RaisePropertyChanged();
			}
		}

		public abstract ulong OrderId { get; }

		public abstract string Text { get; }

		public UserViewModel User { get; protected set; }

		private RelayCommand _DismissSensibleWarningCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _HasSensibleContent;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsLoading;
	}
}