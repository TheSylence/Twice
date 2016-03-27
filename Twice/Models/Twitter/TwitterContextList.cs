using System;
using System.Collections.Generic;
using Twice.ViewModels;

namespace Twice.Models.Twitter
{
	internal class TwitterContextList : ITwitterContextList
	{
		public TwitterContextList( INotifier notifier )
		{
			Contexts = new List<IContextEntry>();

			Contexts.Add( new ContextEntry(notifier) );
		}
		
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		private void Dispose( bool disposing )
		{
			if( disposing )
			{
				foreach( var context in Contexts )
				{
					context.Dispose();
				}
			}
		}

		public ICollection<IContextEntry> Contexts { get; }
	}
}