using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Anotar.NLog;

namespace Twice.Utilities
{
	internal class DisplayHelper
	{
		public static IEnumerable<KeyValuePair<string,string>> GetAvailableDisplays()
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			var device = new NativeMethods.DISPLAY_DEVICE();
			device.cb = Marshal.SizeOf( device );
			try
			{
				for( uint id = 0; NativeMethods.EnumDisplayDevices( null, id, ref device, 0 ); id++ )
				{
					device.cb = Marshal.SizeOf( device );
					NativeMethods.EnumDisplayDevices( device.DeviceName, 0, ref device, 0 );

					result.Add( device.DeviceID, device.DeviceString );
					device.cb = Marshal.SizeOf( device );
				}
			}

			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to read available displays", ex );
			}

			return result;
		}
	}
}