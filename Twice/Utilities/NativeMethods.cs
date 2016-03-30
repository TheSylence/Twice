using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Twice.Utilities
{
	/// <summary>
	/// Class holding native interop methods.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal static class NativeMethods
	{
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

		/// <summary>
		/// Prompt structure to be used for required parameters.
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
		/// BLOB structure used to pass data to DPAPI functions.
		/// </summary>
		internal struct DATA_BLOB
		{
			public int cbData;
			public IntPtr pbData;
		}
	}
}