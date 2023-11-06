using System.Numerics;

namespace Adventageous.Utilities;

public static class SomeMath
{
	public static TNumber UnsignedModulo<TNumber>(TNumber left, TNumber right)
		where TNumber : IModulusOperators<TNumber, TNumber, TNumber>, INumber<TNumber>
	{
		if (right <= TNumber.AdditiveIdentity)
			throw new InvalidOperationException("Modulo of numbers less than 0 not supported");

		var remainder = left % right;
		if (TNumber.Sign(remainder) < 0)
			return remainder + right;
		return remainder;
	}
}