using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Twice.Utilities
{
	/// <summary>
	///     Class holding native interop methods.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal static class NativeMethods
	{
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

		// Wrapper for DPAPI CryptProtectData function.
		[DllImport( "crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
		[return: MarshalAs( UnmanagedType.Bool )]
		internal static extern bool CryptProtectData( ref DATA_BLOB pPlainText, string szDescription, ref DATA_BLOB pEntropy, IntPtr pReserved,
			ref CRYPTPROTECT_PROMPTSTRUCT pPrompt, int dwFlags, ref DATA_BLOB pCipherText );

		// Wrapper for DPAPI CryptUnprotectData function.
		[DllImport( "crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
		[return: MarshalAs( UnmanagedType.Bool )]
		internal static extern bool CryptUnprotectData( ref DATA_BLOB pCipherText, ref string pszDescription, ref DATA_BLOB pEntropy, IntPtr pReserved,
			ref CRYPTPROTECT_PROMPTSTRUCT pPrompt, int dwFlags, ref DATA_BLOB pPlainText );

		[DllImport( "user32.dll" )]
		internal static extern bool EnumDisplayDevices( string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags );

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
		public struct DISPLAY_DEVICE
		{
			[MarshalAs( UnmanagedType.U4 )] public int cb;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 32 )] public string DeviceName;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )] public string DeviceString;

			[MarshalAs( UnmanagedType.U4 )] public DisplayDeviceStateFlags StateFlags;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )] public string DeviceID;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )] public string DeviceKey;
		}

		/// <summary>
		///     Prompt structure to be used for required parameters.
		/// </summary>
		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		internal struct CRYPTPROTECT_PROMPTSTRUCT
		{
			public int cbSize;
			public int dwPromptFlags;
			public IntPtr hwndApp;
			public string szPrompt;
		}

		/// <summary>
		///     BLOB structure used to pass data to DPAPI functions.
		/// </summary>
		internal struct DATA_BLOB
		{
			public int cbData;
			public IntPtr pbData;
		}
	}
}