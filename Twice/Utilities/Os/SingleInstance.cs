using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Twice.Utilities.Os
{
	[ExcludeFromCodeCoverage]
	internal static class SingleInstance
	{
		static SingleInstance()
		{
			WM_SHOWFIRSTINSTANCE = NativeMethods.RegisterWindowMessage( $"WM_SHOWFIRSTINSTANCE|{AssemblyGuid}" );
		}

		public static void ShowFirstInstance()
		{
			NativeMethods.PostMessage( (IntPtr)NativeMethods.HWND_BROADCAST, WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero );
		}

		public static bool Start()
		{
			string mutextName = $"Twice.{AssemblyGuid}".Replace( "-", "" );

			bool onlyInstance;
			AppMutex = new Mutex( true, mutextName, out onlyInstance );
			ReleaseMutex = onlyInstance;
			return onlyInstance;
		}

		public static void Stop()
		{
			if( ReleaseMutex )
			{
				AppMutex?.ReleaseMutex();
				AppMutex?.Dispose();
			}
		}

		internal static readonly int WM_SHOWFIRSTINSTANCE;
		private static Mutex AppMutex;
		private static bool ReleaseMutex;

		private static string AssemblyGuid
		{
			get
			{
				IEnumerable<GuidAttribute> attributes =
					Assembly.GetExecutingAssembly().GetCustomAttributes<GuidAttribute>().ToArray();
				if( !attributes.Any() )
				{
					return string.Empty;
				}

				return attributes.First().Value;
			}
		}
	}
}