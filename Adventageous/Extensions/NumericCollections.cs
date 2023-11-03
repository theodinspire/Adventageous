using System.Numerics;

namespace Adventageous.Extensions
{
	public static class NumericCollections
	{
		public static TNumber Total<TNumber>(this IEnumerable<TNumber> self)
			where TNumber : IAdditiveIdentity<TNumber, TNumber>, IAdditionOperators<TNumber, TNumber, TNumber>
		{
			return self.Aggregate(TNumber.AdditiveIdentity, (left, right) => left + right);
		}
	}
}