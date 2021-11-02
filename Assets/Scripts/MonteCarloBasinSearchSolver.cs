// using System;
// using System.Collections.Generic;
// using System.Threading;
//
// public class MonteCarloBasinSearchSolver : ISolver
// {
// 	private readonly int trialPointsNumber = 1000;
// 	private readonly float derivativeFiniteDelta = 0.01f;
// 	private readonly float descentRate = 10f;
// 	
// 	private readonly object bestLock = new object();
// 	private float[] BestSolution{get; set;}
// 	private float BestEvaluation{get; set;}
// 	
// 	private readonly Thread thread;
// 	private Func<float[], float> targetFunc;
// 	
// 	private int dimension;
// 	private float[] direction;
//
// 	private List<int[]> iterativeDimensionMasks;
//
// 	private readonly Random random;
//
// 	public MonteCarloBasinSearchSolver()
// 	{
// 		thread = new Thread(Solve);
// 		random = new Random();
// 		iterativeDimensionMasks = new List<int[]>();
// 	}
// 	
// 	public void StartSolving(Func<float[], float> target, int solutionDimension)
// 	{
// 		BestSolution = new float[solutionDimension];
// 		BestEvaluation = float.PositiveInfinity;
// 		targetFunc = target;
// 		dimension = solutionDimension;
// 		direction = new float[dimension];
// 		for (var i = 0; i < dimension; ++i)
// 		{
// 			var mask = new[] {i};
// 			iterativeDimensionMasks.Add(mask);
// 		}
// 		thread.Start();
// 	}
//
// 	public float[] GetTempResult()
// 	{
// 		float[] toReturn;
// 		lock(bestLock)
// 		{
// 			// Debug.Log($"{Best.Item2}: {Best.Item1[0]} {Best.Item1[1]} {Best.Item1[2]} {Best.Item1[3]}");
// 			toReturn = (float[]) BestSolution.Clone();
// 		}
// 		return toReturn;
// 	}
//
// 	public void StopSolving()
// 	{
// 		thread.Abort();
// 	}
//
// 	public bool IsSolving => thread.IsAlive;
//
// 	private void Solve()
// 	{
// 		// init
// 		var pts = new List<float[]>(trialPointsNumber);
// 		pts.Add(new float[dimension]);
// 		for(var i = 1; i < trialPointsNumber; ++i)
// 		{
// 			pts.Add(GenerateRandomTrialPoint());
// 			TryUpdateBest(pts, i);
// 		}
// 		
// 		// local update
// 		// for(var tries = 0; tries < 10000; ++tries)
// 		while(true)
// 		{
// 			for(var i = 0; i < trialPointsNumber; ++i)
// 			{
// 				foreach (var mask in iterativeDimensionMasks)
// 				{
// 					var current = targetFunc(pts[i]); // should be cached
// 					var sumToNormalize = 0f;
// 					// for(var j = 0; j < dimension; ++j)
// 					foreach(var j in mask)
// 					{
// 						var pt = (float[])pts[i].Clone();
// 						pt[j] += derivativeFiniteDelta;
// 						var attempt = targetFunc(pt);
// 						direction[j] = current - attempt;
// 						if(Math.Abs(direction[j]) < 1e-16)
// 							direction[j] = 0;
// 						sumToNormalize += direction[j];
// 					}
//
// 					if(sumToNormalize < float.Epsilon)
// 					{
// 						sumToNormalize = 1f;
// 					}
//
// 					// for(var j = 0; j < dimension; ++j)
// 					foreach(var j in mask)
// 					{
// 						pts[i][j] += direction[j] / sumToNormalize * dimension * descentRate;
// 						pts[i][j] %= 360;
// 					}
// 					TryUpdateBest(pts, i);
// 				}
// 			}
// 		}
// 	}
//
// 	private void TryUpdateBest(List<float[]> pts, int i)
// 	{
// 		var attempt = targetFunc(pts[i]);
// 		if(attempt < BestEvaluation)
// 		{
// 			BestEvaluation = attempt;
// 			lock(bestLock)
// 			{
// 				BestSolution = (float[])pts[i].Clone();
// 			}
// 		}
// 	}
//
// 	private float[] GenerateRandomTrialPoint()
// 	{
// 		var pt = new float[dimension];
// 		for(var i = 0; i < dimension; ++i)
// 		{
// 			// if(i < 2)
// 				pt[i] = random.Next() % 360;
// 			// else
// 			// 	pt[i] = 0;
// 		}
//
// 		return pt;
// 	}
// }