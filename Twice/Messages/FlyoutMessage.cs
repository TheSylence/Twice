using GalaSoft.MvvmLight.Messaging;

namespace Twice.Messages
{
	internal enum FlyoutAction
	{
		Open,
		Close,
		Toggle
	}

	internal class FlyoutMessage : MessageBase
	{
		public FlyoutMessage( string name, FlyoutAction action, object dataContext = null )
		{
			Name = name;
			Action = action;
			DataContext = dataContext;
		}

		public readonly FlyoutAction Action;
		public readonly object DataContext;
		public readonly string Name;
	}
}