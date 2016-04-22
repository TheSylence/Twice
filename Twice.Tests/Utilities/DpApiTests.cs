using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Utilities;
using Twice.Utilities.Os;

namespace Twice.Tests.Utilities
{
	[TestClass]
	public class DpApiTests
	{
		[TestMethod, TestCategory( "Utilities" )]
		public void MachineAndUserKeysAreDifferent()
		{
			// Arrange
			string plain = "Hello World this is a test with some text";

			// Act
			string machine = DpApi.Encrypt( DpApi.KeyType.MachineKey, plain );
			string user = DpApi.Encrypt( DpApi.KeyType.UserKey, plain );

			// Assert
			Assert.AreNotEqual( machine, user );
		}

		[TestMethod, TestCategory( "Utilities" )]
		public void MachineLevelEncryptionCanBeDecrypted()
		{
			// Arrange
			string plain = "Hello World this is a test with some text";

			// Act
			string encrypted = DpApi.Encrypt( plain );
			string decrypted = DpApi.Decrypt( encrypted );

			// Assert
			Assert.AreNotEqual( plain, encrypted );
			Assert.AreEqual( plain, decrypted );
		}

		[TestMethod, TestCategory( "Utilities" )]
		public void UserLevelEncryptionCanBeDecrypted()
		{
			// Arrange
			string plain = "Hello World this is a test with some text";

			// Act
			string encrypted = DpApi.Encrypt( DpApi.KeyType.UserKey, plain );
			string decrypted = DpApi.Decrypt( encrypted );

			// Assert
			Assert.AreNotEqual( plain, encrypted );
			Assert.AreEqual( plain, decrypted );
		}
	}
}