using Moq;
using System.Linq;

namespace Twice.Tests
{
	public static class MockExtensions
	{
		public static void SetupIQueryable<T>( this Mock<T> mock, IQueryable queryable )
			where T : class, IQueryable
		{
			mock.Setup( r => r.GetEnumerator() ).Returns( queryable.GetEnumerator() );
			mock.Setup( r => r.Provider ).Returns( queryable.Provider );
			mock.Setup( r => r.ElementType ).Returns( queryable.ElementType );
			mock.Setup( r => r.Expression ).Returns( queryable.Expression );
		}
	}
}