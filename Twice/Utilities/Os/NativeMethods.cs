using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Twice.Utilities.Os
{
	/// <summary>
	///     Class holding native interop methods.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal static class NativeMethods
	{
		[DllImport( "user32.dll" )]
		internal static extern bool EnumDisplayDevices( string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice,
			uint dwFlags );

		[return: MarshalAs( UnmanagedType.Bool )]
		[DllImport( "user32.dll", SetLastError = true )]
		internal static extern bool PostMessage( IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam );

		[DllImport( "user32" )]
		internal static extern int RegisterWindowMessage( [MarshalAs( UnmanagedType.LPWStr )] string message );

		[Flags]
		public enum DisplayDeviceStateFlags
		{
			/// <summary>
			///     The device is part of the desktop.
			/// </summary>
			AttachedToDesktop = 0x1,

			MultiDriver = 0x2,

			/// <summary>
			///     The device is part of the desktop.
			/// </summary>
			PrimaryDevice = 0x4,

			/// <summary>
			///     Represents a pseudo device used to mirror application drawing for remoting or other purposes.
			/// </summary>
			MirroringDriver = 0x8,

			/// <summary>
			///     The device is VGA compatible.
			/// </summary>
			VGACompatible = 0x10,

			/// <summary>
			///     The device is removable; it cannot be the primary display.
			/// </summary>
			Removable = 0x20,

			/// <summary>
			///     The device has more display modes than its output devices support.
			/// </summary>
			ModesPruned = 0x8000000,

			Remote = 0x4000000,
			Disconnect = 0x2000000
		}

		public const int HWND_BROADCAST = 0xffff;

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
		public struct DISPLAY_DEVICE
		{
			[MarshalAs( UnmanagedType.U4 )]
			public int cb;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 32 )]
			public string DeviceName;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )]
			public string DeviceString;

			[MarshalAs( UnmanagedType.U4 )]
			public DisplayDeviceStateFlags StateFlags;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )]
			public string DeviceID;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )]
			public string DeviceKey;
		}
	}
}