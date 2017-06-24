using Anotar.NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;

namespace Twice.Utilities.Os
{
	[ExcludeFromCodeCoverage]
	internal static class DisplayHelper
	{
		public static IEnumerable<KeyValuePair<string, string>> GetAvailableDisplays()
		{
			var result = new Dictionary<string, string>();

			try
			{
				var displayModes = GetDisplayModes();

				for( int i = 0; i < displayModes.Length; i++ )
				{
					if( displayModes[i].infoType == NativeMethods.DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET )
					{
						result.Add( displayModes[i].id.ToString(), MonitorFriendlyName( displayModes[i].adapterId, displayModes[i].id ) );
					}
				}
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to read available displays", ex );
			}

			return result;
		}

		public static Rect GetDisplayPosition( string displayName )
		{
			var result = new Rect();

			try
			{
				var displayModes = GetDisplayModes();

				bool next = false;
				for( int i=0; i<displayModes.Length; ++i )
				{
					if( next )
					{
						var mode = displayModes[i].modeInfo.sourceMode;
						return new Rect( mode.position.x, mode.position.y, mode.width, mode.height );
					}

					if( displayModes[i].id.ToString() == displayName )
					{
						next = true;
					}
				}
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to read available displays", ex );
			}

			return result;
		}

		static bool Equals( NativeMethods.LUID a, NativeMethods.LUID b )
		{
			return a.HighPart == b.HighPart && a.LowPart == b.LowPart;
		}

		private static NativeMethods.DISPLAYCONFIG_MODE_INFO[] GetDisplayModes()
		{
			uint pathCount, modeCount;
			int error = NativeMethods.GetDisplayConfigBufferSizes(
				NativeMethods.QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount );

			if( error != NativeMethods.ERROR_SUCCESS )
			{
				throw new Win32Exception( error );
			}

			var displayPaths = new NativeMethods.DISPLAYCONFIG_PATH_INFO[pathCount];
			var displayModes = new NativeMethods.DISPLAYCONFIG_MODE_INFO[modeCount];
			error = NativeMethods.QueryDisplayConfig(
				NativeMethods.QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, ref pathCount, displayPaths,
				ref modeCount, displayModes, IntPtr.Zero );

			if( error != NativeMethods.ERROR_SUCCESS )
			{
				throw new Win32Exception( error );
			}

			return displayModes;
		}

		private static string MonitorFriendlyName( NativeMethods.LUID adapterId, uint targetId )
		{
			var deviceName = new NativeMethods.DISPLAYCONFIG_TARGET_DEVICE_NAME();
			deviceName.header.size = (uint)Marshal.SizeOf( typeof( NativeMethods.DISPLAYCONFIG_TARGET_DEVICE_NAME ) );
			deviceName.header.adapterId = adapterId;
			deviceName.header.id = targetId;
			deviceName.header.type = NativeMethods.DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
			int error = NativeMethods.DisplayConfigGetDeviceInfo( ref deviceName );
			if( error != NativeMethods.ERROR_SUCCESS )
			{
				throw new Win32Exception( error );
			}

			return deviceName.monitorFriendlyDeviceName;
		}
	}
}