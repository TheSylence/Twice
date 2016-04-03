using System.Diagnostics.CodeAnalysis;
using Ninject;
using Ninject.Activation;
using Twice.Models.Twitter;
using Twice.ViewModels;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	class TwitterContextListProvider : Provider<ITwitterContextList>
	{
		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// The created instance.
		/// </returns>
		protected override ITwitterContextList CreateInstance( IContext context )
		{
			var notifier = context.Kernel.Get<INotifier>();
			return new TwitterContextList( notifier, Constants.IO.AccountsFileName );
		}
	}
}