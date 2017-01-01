using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Configuration;
using Twice.Models.Media;
using Twice.Models.Twitter.Entities;
using Twice.Views.Services;

namespace Twice.ViewModels.Twitter
{
	internal abstract class ColumnItem : ObservableObject, IHighlightable
	{
		/// <summary>
		/// For testing purposes only!
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		protected ColumnItem()
			: this( null, null )
		{
		}

		protected ColumnItem( IConfig config, IViewServiceRepository viewServiceRepo )
		{
			Config = config;
			ViewServiceRepository = viewServiceRepo;
		}

		public virtual Task LoadDataAsync()
		{
			return Task.CompletedTask;
		}

		public virtual void UpdateRelativeTime()
		{
			RaisePropertyChanged( nameof( CreatedAt ) );
		}

		protected abstract Task LoadInlineMedias();

		private void ExecuteDismissSensibleWarningCommand()
		{
			HasSensibleContent = false;
		}

		protected async void Image_OpenRequested( object sender, EventArgs args )
		{
			var selected = sender as StatusMediaViewModel;
			Debug.Assert( selected != null );

			var allUris = InlineMedias.ToList();
			var selectedUri = selected;

			await ViewServiceRepository.ViewImage( allUris, selectedUri );
		}

		public abstract DateTime CreatedAt { get; }

		public ICommand DismissSensibleWarningCommand => _DismissSensibleWarningCommand ?? ( _DismissSensibleWarningCommand = new RelayCommand(
			                                                 ExecuteDismissSensibleWarningCommand ) );

		public bool DisplayMedia => InlineMedias.Any();
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
		public IEnumerable<StatusMediaViewModel> InlineMedias => _InlineMedias;

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

		public IMediaExtractorRepository MediaExtractor
		{
			protected get { return _MediaExtractor ?? DefaultMediaExtractor; }
			set { _MediaExtractor = value; }
		}

		public abstract ulong OrderId { get; }
		public abstract string Text { get; }
		public UserViewModel User { get; protected set; }
		protected readonly IConfig Config;
		protected readonly IViewServiceRepository ViewServiceRepository;
		private static readonly IMediaExtractorRepository DefaultMediaExtractor = MediaExtractorRepository.Default;
		protected readonly List<StatusMediaViewModel> _InlineMedias = new List<StatusMediaViewModel>();
		private RelayCommand _DismissSensibleWarningCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _HasSensibleContent;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsLoading;

		private IMediaExtractorRepository _MediaExtractor;
		public abstract ICommand BlockUserCommand { get; }
		public abstract ICommand ReportSpamCommand { get; }
	}
}