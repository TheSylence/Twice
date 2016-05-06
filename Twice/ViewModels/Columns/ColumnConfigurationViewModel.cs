using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.Models.Columns;

namespace Twice.ViewModels.Columns
{
	internal class ColumnConfigurationViewModel : ObservableObject, IColumnConfigurationViewModel
	{
		public ColumnConfigurationViewModel( ColumnDefinition definition )
		{
			Definition = definition;
		}

		private bool CanExecuteSaveCommand()
		{
			return Changed;
		}

		private void ExecuteSaveCommand()
		{
			Definition.Notifications.Toast = ToastsEnabled;
			Definition.Notifications.Sound = SoundEnabled;
			Definition.Notifications.Popup = PopupEnabled;

			Saved?.Invoke( this, EventArgs.Empty );
			Changed = false;
			IsExpanded = false;
		}

		public event EventHandler Saved;

		public ColumnDefinition Definition { get; }

		public bool IsExpanded
		{
			[DebuggerStepThrough] get { return _IsExpanded; }
			set
			{
				if( _IsExpanded == value )
				{
					return;
				}

				_IsExpanded = value;
				RaisePropertyChanged();

				if( _IsExpanded )
				{
					ToastsEnabled = Definition.Notifications.Toast;
					SoundEnabled = Definition.Notifications.Sound;
					PopupEnabled = Definition.Notifications.Popup;
					Changed = false;
				}
			}
		}

		public bool PopupEnabled
		{
			[DebuggerStepThrough] get { return _PopupEnabled; }
			set
			{
				if( _PopupEnabled == value )
				{
					return;
				}

				Changed = true;
				_PopupEnabled = value;
				RaisePropertyChanged();
			}
		}

		public ICommand SaveCommand
			=> _SaveCommand ?? ( _SaveCommand = new RelayCommand( ExecuteSaveCommand, CanExecuteSaveCommand ) );

		public bool SoundEnabled
		{
			[DebuggerStepThrough] get { return _SoundEnabled; }
			set
			{
				if( _SoundEnabled == value )
				{
					return;
				}

				Changed = true;
				_SoundEnabled = value;
				RaisePropertyChanged();
			}
		}

		public bool ToastsEnabled
		{
			[DebuggerStepThrough] get { return _ToastsEnabled; }
			set
			{
				if( _ToastsEnabled == value )
				{
					return;
				}

				Changed = true;
				_ToastsEnabled = value;
				RaisePropertyChanged();
			}
		}

		public bool Changed
		{
			[DebuggerStepThrough] get { return _Changed; }
			set
			{
				if( _Changed == value )
				{
					return;
				}

				_Changed = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _Changed;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsExpanded;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _PopupEnabled;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _SaveCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _SoundEnabled;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _ToastsEnabled;
	}
}