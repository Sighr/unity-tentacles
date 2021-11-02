using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class IterativeLinearMinimizerSolver : ISolver
{
	private readonly int trialPointsNumber = 100;
	private readonly float derivativeFiniteDelta = 0.01f;
	private readonly float baseDescentRate = 90f;

	private readonly object bestLock = new object();
	private float[] BestSolution{get; set;}
	private float BestEvaluation{get; set;}
	
	private Thread thread;
	private Func<float[], float> targetFunc;
	
	private int dimension;


	private readonly Random random;

	public IterativeLinearMinimizerSolver()
	{
		random = new Random();
	}

	private void Solve()
	{
		// init
		var pts = new List<float[]>(trialPointsNumber);
		pts.Add(new float[dimension]);
		for(var i = 1; i < trialPointsNumber; ++i)
		{
			pts.Add(GenerateRandomTrialPoint());
			TryUpdateBest(pts[i]);
		}
		
		var pointEqualityComparer = new PointNeighbourhoodComparer();
		var mask = Enumerable.Range(0, dimension).ToList();
		
		for(var cycles = 0; true; ++cycles)
		{
			for (var i = 0; i < trialPointsNumber; ++i)
			{
				// for(var j = 0; j < dimension; ++j)
				foreach(var j in mask)
				{
					var descentRate = baseDescentRate;
					var previousDirection = 0;
					int direction = 1;
					do
					{
						var current = targetFunc(pts[i]);
						var modifiedPoint = (float[])pts[i].Clone();
						modifiedPoint[j] += derivativeFiniteDelta;
						var attempt = targetFunc(modifiedPoint);
						direction = Math.Sign(current - attempt); // positive if attempt better
						pts[i][j] += descentRate * direction;
						if(previousDirection != direction)
							descentRate /= 2;
						previousDirection = direction;
					} while(descentRate > 1f && direction != 0);

					pts = pts.Select(pt => pt.Select(DegreeRanger).ToArray()).ToList();
					TryUpdateBest(pts[i]);
				}
			}

			pts = pts.Skip(5).Distinct(pointEqualityComparer).ToList();
			// if (pts.Count < trialPointsNumber)
			// 	pts.Add(new float[dimension]);
			while (pts.Count < trialPointsNumber)
				pts.Add(GenerateRandomTrialPoint());
			// mask = mask.OrderBy(val => random.Next()).ToList();
		}
	}

	private float DegreeRanger(float angle)
	{
		while(angle > 360)
			angle -= 360;
		while(angle < 0)
			angle += 360;
		return angle;
	}

	public void StartSolving(Func<float[], float> target, int solutionDimension)
	{
		thread = new Thread(Solve);
		BestSolution = new float[solutionDimension];
		BestEvaluation = float.PositiveInfinity;
		targetFunc = target;
		dimension = solutionDimension;
		thread.Start();
	}

	public float[] GetTempResult()
	{
		float[] toReturn;
		lock(bestLock)
		{
			toReturn = (float[]) BestSolution.Clone();
		}
		return toReturn;
	}

	public void StopSolving()
	{
		thread.Abort();
	}

	public bool IsSolving => thread is { IsAlive: true };

	
	private void TryUpdateBest(float[] pt)
	{
		var attempt = targetFunc(pt);
		if(attempt < BestEvaluation)
		{
			BestEvaluation = attempt;
			lock(bestLock)
			{
				BestSolution = (float[])pt.Clone();
			}
		}
	}
	
	private float[] GenerateRandomTrialPoint()
	{
		var pt = new float[dimension];
		for(var i = 0; i < dimension; ++i)
		{
			// if(i < 2)
			pt[i] = random.Next() % 360;
			// else
			// 	pt[i] = 0;
		}

		return pt;
	}
}