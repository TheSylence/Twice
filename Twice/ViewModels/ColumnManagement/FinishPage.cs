using System.Diagnostics;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class FinishPage : WizardPageViewModel
	{
		public FinishPage( IWizardViewModel wizard )
			: base( wizard )
		{
			IsLastPage = true;
		}

		public override void OnNavigatedTo( bool forward )
		{
			base.OnNavigatedTo( forward );

			SourceAccount = string.Join( ", ", Wizard.GetProperty<string[]>( AddColumnDialogViewModel.SourceAccountNamesKey ) );
		}

		public string ColumnTypeDescription
		{
			[DebuggerStepThrough] get { return _ColumnTypeDescription; }
			set
			{
				if( _ColumnTypeDescription == value )
				{
					return;
				}

				_ColumnTypeDescription = value;
				RaisePropertyChanged();
			}
		}

		public string SourceAccount
		{
			[DebuggerStepThrough] get { return _SourceAccount; }
			set
			{
				if( _SourceAccount == value )
				{
					return;
				}

				_SourceAccount = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _ColumnTypeDescription;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _SourceAccount;
	}
}