using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Twice.ViewModels.Twitter;

namespace Twice.Views
{
	[ExcludeFromCodeCoverage]
	internal class CardTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate( object item, DependencyObject container )
		{
			var card = item as CardViewModel;
			if( card != null )
			{
				switch( card.Card.Type )
				{
				case Models.Media.CardType.Summary:
					return SummaryTemplate;

				case Models.Media.CardType.SummaryLargeImage:
					return SummaryImageTemplate;
				}
			}

			return base.SelectTemplate( item, container );
		}

		public DataTemplate SummaryImageTemplate { get; set; }
		public DataTemplate SummaryTemplate { get; set; }
	}
}