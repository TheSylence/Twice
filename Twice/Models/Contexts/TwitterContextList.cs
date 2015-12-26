using System;
using System.Collections.Generic;

namespace Twice.Models.Contexts
{
	internal class TwitterContextList : ITwitterContextList
	{
		public TwitterContextList()
		{
			Contexts = new List<IContextEntry>();

			Contexts.Add( new ContextEntry() );
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