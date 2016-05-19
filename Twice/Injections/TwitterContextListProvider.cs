using Ninject;
using Ninject.Activation;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class TwitterContextListProvider : Provider<ITwitterContextList>
	{
		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		protected override ITwitterContextList CreateInstance( IContext context )
		{
			var notifier = context.Kernel.Get<INotifier>();
			var serializer = context.Kernel.Get<ISerializer>();
			return new TwitterContextList( notifier, Constants.IO.AccountsFileName, serializer );
		}
	}
}