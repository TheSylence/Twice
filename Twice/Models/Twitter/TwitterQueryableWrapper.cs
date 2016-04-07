using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;

namespace Twice.Models.Twitter
{
	internal class TwitterQueryableWrapper<T> : ITwitterQueryable<T>
	{
		public TwitterQueryableWrapper( TwitterQueryable<T> obj )
		{
			Wrapped = obj;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Wrapped.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Type ElementType => Wrapped.ElementType;
		public Expression Expression => Wrapped.Expression;
		public IQueryProvider Provider => Wrapped.Provider;
		private readonly TwitterQueryable<T> Wrapped;
	}
}