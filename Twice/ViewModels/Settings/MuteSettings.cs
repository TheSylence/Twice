using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using Resourcer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Twice.Messages;
using Twice.Models.Configuration;
using Twice.Resources;
using Twice.Views.Services;

namespace Twice.ViewModels.Settings
{
	internal class MuteSettings : ViewModelBaseEx, IMuteSettings
	{
		public MuteSettings( IConfig config )
		{
			Entries = new ObservableCollection<MuteEntry>( config.Mute.Entries );

			string languageCode = CultureInfo.CreateSpecificCulture( config.General.Language ?? "en-US" ).TwoLetterISOLanguageName;
			HelpDocument = Resource.AsStringUnChecked( $"Twice.Resources.Documentation.Mute_{languageCode}.md" );
		}

		public Task OnLoad( object data )
		{
			return Task.CompletedTask;
		}

		public void SaveTo( IConfig config )
		{
			config.Mute.Entries.Clear();
			config.Mute.Entries.AddRange( Entries );

			if( Changed )
			{
				var msg = new FilterUpdateMessage();
				MessengerInstance.Send( msg );

				if( msg.RemoveCount > 0 )
				{
					var str = string.Format( Strings.StatusesMuted, msg.RemoveCount );
					Notifier.DisplayMessage( str, NotificationType.Information );
				}
			}
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
			Changed = true;
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
			Changed = true;
		}

		public ICommand AddCommand => _AddCommand ?? ( _AddCommand = new RelayCommand( ExecuteAddCommand ) );

		public ICommand EditCommand
			=> _EditCommand ?? ( _EditCommand = new RelayCommand( ExecuteEditCommand, CanExecuteEditCommand ) );

		public IMuteEditViewModel EditData { get; set; }

		public ICollection<MuteEntry> Entries { get; }
		public string HelpDocument { get; }

		[Inject]
		public INotifier Notifier { get; set; }

		public ICommand RemoveCommand
			=> _RemoveCommand ?? ( _RemoveCommand = new RelayCommand( ExecuteRemoveCommand, CanExecuteRemoveCommand ) );

		public MuteEntry SelectedEntry { get; set; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _AddCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _EditCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _RemoveCommand;

		private bool Changed;
	}
}