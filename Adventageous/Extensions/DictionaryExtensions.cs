using System.Numerics;
using Adventageous.DataStructure;

namespace Adventageous.Extensions;

public static class DictionaryExtensions
{
	/// <summary>
	/// Consumes a value for min-max for a key
	/// </summary>
	/// <param name="self">The dictionary of MinMax objects</param>
	/// <param name="key">The key of the min max object</param>
	/// <param name="value">The value to be consumed.</param>
	/// <typeparam name="TKey">The key type</typeparam>
	/// <typeparam name="TNumber">The min-max type</typeparam>
	public static void Consume<TKey, TNumber>(this IDictionary<TKey, MinMax<TNumber>> self, TKey key, TNumber value)
		where TNumber : INumber<TNumber>, IEqualityOperators<TNumber, TNumber, bool>
	{
		if (self.TryGetValue(key, out var minMax))
			minMax.Consume(value);
		else
			self.Add(key, new MinMax<TNumber>(value));
	}

	public static TNumber GetMin<TKey, TNumber>(this IReadOnlyDictionary<TKey, MinMax<TNumber>> self, TKey key)
		where TNumber : INumber<TNumber>, IEqualityOperators<TNumber, TNumber, bool> =>
		self[key].Min;

	public static TNumber GetMax<TKey, TNumber>(this IReadOnlyDictionary<TKey, MinMax<TNumber>> self, TKey key)
		where TNumber : INumber<TNumber>, IEqualityOperators<TNumber, TNumber, bool> =>
		self[key].Max;
}