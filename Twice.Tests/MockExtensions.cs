using Moq;
using System.Linq;

namespace Twice.Tests
{
	public static class MockExtensions
	{
		public static void SetupIQueryable<T>( this Mock<T> mock, IQueryable queryable, IQueryProvider provider = null )
			where T : class, IQueryable
		{
			mock.Setup( r => r.GetEnumerator() ).Returns( queryable.GetEnumerator() );
			mock.SetupGet( r => r.Provider ).Returns( provider ?? queryable.Provider );
			mock.SetupGet( r => r.ElementType ).Returns( queryable.ElementType );
			mock.SetupGet( r => r.Expression ).Returns( queryable.Expression );
		}
	}
}