using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Resourcer;
using Twice.Models.Configuration;
using Twice.Resources;
using Twice.Services.Views;

namespace Twice.ViewModels.Settings
{
	internal class MuteSettings : ViewModelBaseEx, IMuteSettings
	{
		public MuteSettings( IConfig config )
		{
			Entries = new ObservableCollection<MuteEntry>( config.Mute.Entries );

			// TODO: Load for correct language
			HelpDocument = Resource.AsString( "Twice.Resources.Documentation.Mute.md" );
		}

		private bool CanExecuteEditCommand()
		{
			return SelectedEntry != null;
		}

		private bool CanExecuteRemoveCommand()
		{
			return SelectedEntry != null;
		}

		private void EditData_Cancelled( object sender, EventArgs e )
		{
			EditData.Cancelled -= EditData_Cancelled;
			EditData.Saved -= EditData_Saved;
			EditData = null;
		}

		private void EditData_Saved( object sender, MuteEditArgs e )
		{
			Entries.Remove( SelectedEntry );

			var entry = new MuteEntry
			{
				Filter = EditData.Filter,
				EndDate = null,
				CaseSensitive = EditData.CaseSensitive
			};

			if( EditData.HasEndDate )
			{
				entry.EndDate = EditData.EndDate;
			}

			Entries.Add( entry );

			EditData.Cancelled -= EditData_Cancelled;
			EditData.Saved -= EditData_Saved;
			EditData = null;
		}

		private void ExecuteAddCommand()
		{
			EditData = new MuteEditViewModel( MuteEditAction.Add );
			EditData.Cancelled += EditData_Cancelled;
			EditData.Saved += EditData_Saved;
		}

		private void ExecuteEditCommand()
		{
			EditData = new MuteEditViewModel( MuteEditAction.Edit )
			{
				Filter = SelectedEntry.Filter,
				HasEndDate = SelectedEntry.EndDate.HasValue,
				 CaseSensitive = SelectedEntry.CaseSensitive
			};
			if( EditData.HasEndDate && SelectedEntry.EndDate.HasValue )
			{
				EditData.EndDate = SelectedEntry.EndDate.Value;
			}

			EditData.Cancelled += EditData_Cancelled;
			EditData.Saved += EditData_Saved;
		}

		private async void ExecuteRemoveCommand()
		{
			var csa = new ConfirmServiceArgs( Strings.ConfirmDeleteFilter );
			if( !await ViewServiceRepository.Confirm( csa ) )
			{
				return;
			}

			Entries.Remove( SelectedEntry );
			SelectedEntry = null;
		}

		public void SaveTo( IConfig config )
		{
			config.Mute.Entries.Clear();
			config.Mute.Entries.AddRange( Entries );
		}

		public ICommand AddCommand => _AddCommand ?? ( _AddCommand = new RelayCommand( ExecuteAddCommand ) );

		public ICommand EditCommand
			=> _EditCommand ?? ( _EditCommand = new RelayCommand( ExecuteEditCommand, CanExecuteEditCommand ) );

		public IMuteEditViewModel EditData
		{
			[DebuggerStepThrough] get { return _EditData; }
			set
			{
				if( _EditData == value )
				{
					return;
				}

				_EditData = value;
				RaisePropertyChanged();
			}
		}

		public ICollection<MuteEntry> Entries { get; }

		public string HelpDocument { get; }

		public ICommand RemoveCommand
			=> _RemoveCommand ?? ( _RemoveCommand = new RelayCommand( ExecuteRemoveCommand, CanExecuteRemoveCommand ) );

		public MuteEntry SelectedEntry
		{
			[DebuggerStepThrough] get { return _SelectedEntry; }
			set
			{
				if( _SelectedEntry == value )
				{
					return;
				}

				_SelectedEntry = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _AddCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _EditCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private IMuteEditViewModel _EditData;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _RemoveCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private MuteEntry _SelectedEntry;
	}
}