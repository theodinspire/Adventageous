using System.Collections.Generic;

namespace Adventageous.Extentions
{
	public static class NumericCollections
	{
		public static TNumber Total<TNumber>(this ICollection<TNumber> self)
			where TNumber : System.Numerics.IAdditiveIdentity<TNumber, TNumber>, IAdditionOperators<TNumber, TNumber, TNumber>
		{
			
		}
	}
}