using System.Diagnostics;

namespace Twice.ViewModels.Twitter
{
	internal interface IRetweetDialogViewModel : IDialogViewModel
	{
		bool IsQuote { get; set; }
		string QuoteText { get; set; }
		StatusViewModel Status { get; }
	}

	internal class RetweetDialogViewModel : DialogViewModel, IRetweetDialogViewModel
	{
		public bool IsQuote
		{
			[DebuggerStepThrough] get { return _IsQuote; }
			set
			{
				if( _IsQuote == value )
				{
					return;
				}

				_IsQuote = value;
				RaisePropertyChanged();
			}
		}

		public string QuoteText
		{
			[DebuggerStepThrough] get { return _QuoteText; }
			set
			{
				if( _QuoteText == value )
				{
					return;
				}

				_QuoteText = value;
				RaisePropertyChanged();
			}
		}

		public StatusViewModel Status
		{
			[DebuggerStepThrough] get { return _Status; }
			set
			{
				if( _Status == value )
				{
					return;
				}

				_Status = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsQuote;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _QuoteText;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private StatusViewModel _Status;
	}
}