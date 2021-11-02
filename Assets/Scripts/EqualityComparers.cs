using System;
using System.Collections.Generic;
using System.Linq;

public sealed class PointEqualityComparer : IEqualityComparer<float[]>
{
	private readonly FloatEqualityComparer _floatEqualityComparer = new FloatEqualityComparer();
	
	public bool Equals(float[] x, float[] y)
	{
		return x != null && y != null && x.SequenceEqual(y, _floatEqualityComparer);
	}

	public int GetHashCode(float[] obj)
	{
		return obj.Aggregate(0, (acc, x) => acc ^ x.GetHashCode()).GetHashCode();
	}
}

public sealed class PointNeighbourhoodComparer : IEqualityComparer<float[]>
{
	private readonly FloatEqualityComparer _floatEqualityComparer = new FloatEqualityComparer(10);
	
	public bool Equals(float[] x, float[] y)
	{
		return x != null && y != null && x.SequenceEqual(y, _floatEqualityComparer);
	}

	public int GetHashCode(float[] obj)
	{
		return obj.Aggregate(0, (acc, x) => acc ^ x.GetHashCode()).GetHashCode();
	}
}
public sealed class FloatEqualityComparer : IEqualityComparer<float>
{
	private readonly float _epsilon;

	public FloatEqualityComparer()
	{
		_epsilon = 1e-4f;
	}
	
	public FloatEqualityComparer(float eps)
	{
		_epsilon = eps;
	}
	
	public bool Equals(float x, float y)
	{
		return Math.Abs(x - y) < _epsilon;
	}

	public int GetHashCode(float obj)
	{
		return obj.GetHashCode();
	}
}