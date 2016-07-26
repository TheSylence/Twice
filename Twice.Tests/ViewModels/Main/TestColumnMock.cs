using LinqToTwitter;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Main
{
	[ExcludeFromCodeCoverage]
	internal class TestColumnMock : ColumnViewModelBase
	{
		public TestColumnMock( IContextEntry context, ColumnDefinition definition, IConfig config = null,
			IStreamParser parser = null )
			: base( context, definition, config ?? DefaultConfig(), parser ?? DefaultParser() )
		{
			StatusFilterExpression = s => true;
			Icon = Icon.User;
		}

		public TestColumnMock()
			: base( DefaultContext(), new ColumnDefinition( ColumnType.User ), DefaultConfig(), DefaultParser() )
		{
		}

		public void RaiseStatusWrapper( ColumnItem iteme )
		{
			RaiseNewItem( iteme );
		}

		public void SetLoading( bool isLoading )
		{
			IsLoading = isLoading;
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			return SuitableCheck( status );
		}

		protected override bool IsSuitableForColumn( DirectMessage message )
		{
			return false;
		}

		private static IConfig DefaultConfig()
		{
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.General ).Returns( new GeneralConfig() );

			return cfg.Object;
		}

		private static IContextEntry DefaultContext()
		{
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.AccountName ).Returns( string.Empty );

			return context.Object;
		}

		private static IStreamParser DefaultParser()
		{
			var parser = new Mock<IStreamParser>();

			return parser.Object;
		}

		public override Icon Icon { get; }
		protected override Expression<Func<Status, bool>> StatusFilterExpression { get; }
		public readonly Func<Status, bool> SuitableCheck = s => true;
	}
}