namespace Twice.Models.Twitter.Entities
{
	internal interface IHighlightable
	{
		LinqToTwitter.Entities Entities { get; }
		string Text { get; }
	}
}