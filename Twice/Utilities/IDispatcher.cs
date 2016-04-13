using System;
using System.Threading.Tasks;

namespace Twice.Utilities
{
	internal interface IDispatcher
	{
		void CheckBeginInvokeOnUI( Action action );

		Task RunAsync( Action action );
	}
}