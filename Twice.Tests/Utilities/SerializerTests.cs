using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Utilities;

namespace Twice.Tests.Utilities
{
	[TestClass, ExcludeFromCodeCoverage]
	public class SerializerTests
	{
		[TestMethod, TestCategory( "Utilities" )]
		public void SerializedDataCanBeDeserialized()
		{
			// Arrange
			var obj = DateTime.Now;
			var serializer = new Serializer();

			// Act
			var serialized = serializer.Serialize( obj );
			var deserialized = serializer.Deserialize<DateTime>( serialized );

			// Assert
			Assert.AreEqual( obj, deserialized );
		}
	}
}