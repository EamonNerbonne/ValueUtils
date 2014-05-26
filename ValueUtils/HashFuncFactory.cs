using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueUtils {
	static class HashFuncFactory<T> {
		public static readonly Func<T, int> Instance = Create();

		static Func<T, int> Create() {
			var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var paramExpr = Expression.Parameter(typeof(T), "valueToHash");

			throw new NotImplementedException();
		}

	}
}
