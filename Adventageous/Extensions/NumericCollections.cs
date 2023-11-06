using System.Numerics;

namespace Adventageous.Extensions
{
	public static class NumericCollections
	{
		public static TNumber Product<TNumber>(this IEnumerable<TNumber> self)
			where TNumber : IMultiplicativeIdentity<TNumber, TNumber>, IMultiplyOperators<TNumber, TNumber, TNumber> =>
			self.Aggregate(TNumber.MultiplicativeIdentity, (left, right) => left * right);
	}
}