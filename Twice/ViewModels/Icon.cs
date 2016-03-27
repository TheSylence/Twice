using MaterialDesignThemes.Wpf;
using Twice.Attributes;

namespace Twice.ViewModels
{
	internal enum Icon
	{
		[Icon( PackIconKind.Account )]
		User,

		[Icon( PackIconKind.Home )]
		Home,

		[Icon( PackIconKind.At )]
		Mentions
	}
}