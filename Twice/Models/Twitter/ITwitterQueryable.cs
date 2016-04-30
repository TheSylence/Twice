using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Twice.Models.Twitter
{
	internal interface ITwitterQueryable<T> : IOrderedQueryable<T>, IOrderedQueryable, IQueryable, IEnumerable, IQueryable<T>, IEnumerable<T>
	{
	}
}