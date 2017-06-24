using System;
using System.Security.Cryptography;
using System.Text;

namespace Twice.Utilities.Os
{
	/// <summary>
	///     Encrypts and decrypts data using DPAPI functions.
	/// </summary>
	public static class DpApi
	{
		/// <summary>
		///     Flag indicating the type of key. DPAPI terminology refers to key types as user store or
		///     machine store.
		/// </summary>
		public enum KeyType
		{
			/// <summary>
			///     Invalid. Do not use.
			/// </summary>
			None = 0,

			/// <summary>
			///     User store
			/// </summary>
			UserKey = 1,

			/// <summary>
			///     Machine store
			/// </summary>
			MachineKey
		}

		/// <summary>
		///     Calls DPAPI CryptUnprotectData to decrypt ciphertext bytes. This function does not use
		///     additional entropy and does not return data description.
		/// </summary>
		/// <param name="keyType">
		///     Defines type of encryption key to use. When user key is specified, any application
		///     running under the same user account as the one making this call, will be able to decrypt
		///     data. Machine key will allow any application running on the same computer where data
		///     were encrypted to perform decryption.
		///     Note: If optional entropy is specifed, it will be required for decryption.
		/// </param>
		/// <param name="cipherText"> Encrypted data formatted as a base64-encoded string. </param>
		/// <returns> Decrypted data returned as a UTF-8 string. </returns>
		/// <remarks>
		///     When decrypting data, it is not necessary to specify which type of encryption key to
		///     use: user-specific or machine-specific; DPAPI will figure it out by looking at the
		///     signature of encrypted data.
		/// </remarks>
		public static string Decrypt( KeyType keyType, string cipherText )
		{
			var userData = Convert.FromBase64String( cipherText );
			var data = ProtectedData.Unprotect( userData, null, keyType == KeyType.MachineKey ? DataProtectionScope.LocalMachine : DataProtectionScope.CurrentUser );

			return Encoding.UTF8.GetString( data );
		}

		public static string Decrypt( string cipherText )
		{
			return Decrypt( DefaultKeyType, cipherText );
		}

		/// <summary>
		///     Calls DPAPI CryptProtectData function to encrypt a plaintext string value with a
		///     user-specific key. This function does not specify data description and additional entropy.
		/// </summary>
		/// <param name="plainText"> Plaintext data to be encrypted. </param>
		/// <returns> Encrypted value in a base64-encoded format. </returns>
		public static string Encrypt( string plainText )
		{
			return Encrypt( DefaultKeyType, plainText );
		}

		/// <summary>
		///     Calls DPAPI CryptProtectData function to encrypt a plaintext string value. This function
		///     does not specify data description and additional entropy.
		/// </summary>
		/// <param name="keyType">
		///     Defines type of encryption key to use. When user key is specified, any application
		///     running under the same user account as the one making this call, will be able to decrypt
		///     data. Machine key will allow any application running on the same computer where data
		///     were encrypted to perform decryption.
		///     Note: If optional entropy is specifed, it will be required for decryption.
		/// </param>
		/// <param name="plainText"> Plaintext data to be encrypted. </param>
		/// <returns> Encrypted value in a base64-encoded format. </returns>
		public static string Encrypt( KeyType keyType, string plainText )
		{
			var userData = Encoding.UTF8.GetBytes( plainText );
			var data = ProtectedData.Protect( userData, null, keyType == KeyType.MachineKey ? DataProtectionScope.LocalMachine : DataProtectionScope.CurrentUser );

			return Convert.ToBase64String( data );
		}

		// It is reasonable to set default key type to user key.
		private const KeyType DefaultKeyType = KeyType.UserKey;
	}
}