using System;

public interface ISolver
{
	public void StartSolving(Func<float[], float> targetFunc, int solutionDimention);
	public float[] GetTempResult();
	public void StopSolving();
	
	public bool IsSolving { get; }
}