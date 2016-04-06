using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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

	internal class ColumnConfigurationViewModel : ObservableObject, IColumnConfigurationViewModel
	{
		public ColumnConfigurationViewModel( ColumnDefinition definition )
		{
			Definition = definition;
		}

		public event EventHandler Saved;

		private void ExecuteSaveCommand()
		{
			Definition.Notifications.Toast = ToastsEnabled;
			Definition.Notifications.Sound = SoundEnabled;
			Definition.Notifications.Popup = PopupEnabled;

			Saved?.Invoke( this, EventArgs.Empty );
		}

		public ColumnDefinition Definition { get; }

		public bool IsExpanded
		{
			[DebuggerStepThrough]
			get
			{
				return _IsExpanded;
			}
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
				}
			}
		}

		public bool PopupEnabled
		{
			[DebuggerStepThrough]
			get
			{
				return _PopupEnabled;
			}
			set
			{
				if( _PopupEnabled == value )
				{
					return;
				}

				_PopupEnabled = value;
				RaisePropertyChanged();
			}
		}

		public ICommand SaveCommand => _SaveCommand ?? ( _SaveCommand = new RelayCommand( ExecuteSaveCommand ) );

		public bool SoundEnabled
		{
			[DebuggerStepThrough]
			get
			{
				return _SoundEnabled;
			}
			set
			{
				if( _SoundEnabled == value )
				{
					return;
				}

				_SoundEnabled = value;
				RaisePropertyChanged();
			}
		}

		public bool ToastsEnabled
		{
			[DebuggerStepThrough]
			get
			{
				return _ToastsEnabled;
			}
			set
			{
				if( _ToastsEnabled == value )
				{
					return;
				}

				_ToastsEnabled = value;
				RaisePropertyChanged();
			}
		}

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