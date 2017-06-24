using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Global ReSharper disable InconsistentNaming

namespace Twice.Utilities.Os
{
	/// <summary>
	///     Class holding native interop methods.
	/// </summary>
	[ExcludeFromCodeCoverage]
	[SuppressMessage( "ReSharper", "InconsistentNaming" )]
	internal static class NativeMethods
	{
		public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
		{
			DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
			DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
			DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
			DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
			DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
			DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
			DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = 0xFFFFFFFF
		}

		public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
		{
			DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
			DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
			DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = 0xFFFFFFFF
		}

		public enum DISPLAYCONFIG_PIXELFORMAT : uint
		{
			DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
			DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
			DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
			DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
			DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
			DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = 0xffffffff
		}

		public enum DISPLAYCONFIG_ROTATION : uint
		{
			DISPLAYCONFIG_ROTATION_IDENTITY = 1,
			DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
			DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
			DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
			DISPLAYCONFIG_ROTATION_FORCE_UINT32 = 0xFFFFFFFF
		}

		public enum DISPLAYCONFIG_SCALING : uint
		{
			DISPLAYCONFIG_SCALING_IDENTITY = 1,
			DISPLAYCONFIG_SCALING_CENTERED = 2,
			DISPLAYCONFIG_SCALING_STRETCHED = 3,
			DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
			DISPLAYCONFIG_SCALING_CUSTOM = 5,
			DISPLAYCONFIG_SCALING_PREFERRED = 128,
			DISPLAYCONFIG_SCALING_FORCE_UINT32 = 0xFFFFFFFF
		}

		public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
		{
			DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
			DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
			DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
			DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
			DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
			DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = 0xFFFFFFFF
		}

		public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
		{
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = 0xFFFFFFFF,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = 0x80000000,
			DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = 0xFFFFFFFF
		}

		public enum QUERY_DEVICE_CONFIG_FLAGS : uint
		{
			QDC_ALL_PATHS = 0x00000001,
			QDC_ONLY_ACTIVE_PATHS = 0x00000002,
			QDC_DATABASE_CURRENT = 0x00000004
		}

		[DllImport( "user32.dll" )]
		public static extern int DisplayConfigGetDeviceInfo(
			ref DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName
		);

		[DllImport( "user32.dll" )]
		public static extern int GetDisplayConfigBufferSizes(
			QUERY_DEVICE_CONFIG_FLAGS Flags,
			out uint NumPathArrayElements,
			out uint NumModeInfoArrayElements
		);

		[DllImport( "user32.dll" )]
		public static extern int QueryDisplayConfig(
			QUERY_DEVICE_CONFIG_FLAGS Flags,
			ref uint NumPathArrayElements,
			[Out] DISPLAYCONFIG_PATH_INFO[] PathInfoArray,
			ref uint NumModeInfoArrayElements,
			[Out] DISPLAYCONFIG_MODE_INFO[] ModeInfoArray,
			IntPtr CurrentTopologyId
		);

		[return: MarshalAs( UnmanagedType.Bool )]
		[DllImport( "user32.dll", SetLastError = true )]
		internal static extern bool PostMessage( IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam );

		[DllImport( "user32" )]
		internal static extern int RegisterWindowMessage( [MarshalAs( UnmanagedType.LPWStr )] string message );

		public const int ERROR_SUCCESS = 0;

		public const int HWND_BROADCAST = 0xffff;

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_2DREGION
		{
			public uint cx;
			public uint cy;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
		{
			public LUID adapterId;
			public uint id;
			public uint size;
			public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_MODE_INFO
		{
			public LUID adapterId;
			public uint id;
			public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
			public DISPLAYCONFIG_MODE_INFO_UNION modeInfo;
		}

		[StructLayout( LayoutKind.Explicit )]
		public struct DISPLAYCONFIG_MODE_INFO_UNION
		{
			[FieldOffset( 0 )] public DISPLAYCONFIG_SOURCE_MODE sourceMode;

			[FieldOffset( 0 )] public DISPLAYCONFIG_TARGET_MODE targetMode;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_PATH_INFO
		{
			public uint flags;
			public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
			public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_PATH_SOURCE_INFO
		{
			public LUID adapterId;
			public uint id;
			public uint modeInfoIdx;
			public uint statusFlags;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_PATH_TARGET_INFO
		{
			public LUID adapterId;
			public uint id;
			public uint modeInfoIdx;
			private readonly DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
			private readonly DISPLAYCONFIG_RATIONAL refreshRate;
			private readonly DISPLAYCONFIG_ROTATION rotation;
			private readonly DISPLAYCONFIG_SCALING scaling;
			private readonly DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
			public uint statusFlags;
			public bool targetAvailable;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_RATIONAL
		{
			public uint Denominator;
			public uint Numerator;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_SOURCE_MODE
		{
			public uint height;
			public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
			public POINTL position;
			public uint width;
		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
		{
			public uint connectorInstance;
			public ushort edidManufactureId;
			public ushort edidProductCodeId;
			public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
			public DISPLAYCONFIG_DEVICE_INFO_HEADER header;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )] public string monitorDevicePath;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 64 )] public string monitorFriendlyDeviceName;

			public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
		{
			public uint value;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_TARGET_MODE
		{
			public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
		{
			public DISPLAYCONFIG_2DREGION activeSize;
			public DISPLAYCONFIG_RATIONAL hSyncFreq;
			public ulong pixelRate;
			public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
			public DISPLAYCONFIG_2DREGION totalSize;
			public uint videoStandard;
			public DISPLAYCONFIG_RATIONAL vSyncFreq;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct LUID
		{
			public int HighPart;
			public uint LowPart;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct POINTL
		{
			public int x;
			public int y;
		}
	}
}