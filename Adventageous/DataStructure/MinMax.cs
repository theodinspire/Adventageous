using System.Numerics;

namespace Adventageous.DataStructure;

public class MinMax<TNumber>
	where TNumber: INumber<TNumber>, IEqualityOperators<TNumber, TNumber, bool>
{
	public MinMax(TNumber value)
	{
		this.Min = value;
		this.Max = value;
	}

	public TNumber Min { get; private set; }

	public TNumber Max { get; private set; }

	public void Consume(TNumber value)
	{
		this.Min = TNumber.Min(this.Min, value);
		this.Max = TNumber.Max(this.Max, value);
	}
}