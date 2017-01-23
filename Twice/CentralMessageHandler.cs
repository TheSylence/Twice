using GalaSoft.MvvmLight.Messaging;
using Ninject;
using System;
using Twice.Messages;
using Twice.Models.Configuration;

namespace Twice
{
	/// <summary>
	///  Central message handler to keep stuff out of view models 
	/// </summary>
	internal class CentralMessageHandler : IDisposable
	{
		public CentralMessageHandler( IKernel kernel )
		{
			Kernel = kernel;
			Messenger = kernel.Get<IMessenger>();

			Messenger.Register<ColumnLockMessage>( this, OnColumnLock );
		}

		/// <summary>
		///  Performs application-defined tasks associated with freeing, releasing, or resetting
		///  unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Messenger.Unregister( this );
		}

		private void OnColumnLock( ColumnLockMessage msg )
		{
			var config = Kernel.Get<IConfig>();
			config.General.ColumnsLocked = msg.Locked;
			config.Save();
		}

		private readonly IKernel Kernel;
		private readonly IMessenger Messenger;
	}
}