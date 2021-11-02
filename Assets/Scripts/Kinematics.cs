using System;
using System.Linq;
using UnityEngine;


public class Kinematics : MonoBehaviour
{

    [SerializeField]
    private Transform target;
    private Vector3 targetPos;
    private SingleDegreeJoint[] joints;
    public const float ActorHeight = 1;
    private ISolver solver;
    private CoordinateSystem baseCoordinateSystem;

    private float[] previousAngles;
    private float[] desiredAngles;
    public float speed = 2;
    private FloatEqualityComparer feo;
    
    // Start is called before the first frame update
    void Start()
    {
        joints = GetComponentsInChildren<SingleDegreeJoint>();
        solver = new IterativeLinearMinimizerSolver();
        targetPos = target.position;
        baseCoordinateSystem = new CoordinateSystem(transform, new Vector3(0, -1, 0));
        previousAngles = new float[joints.Length];
        desiredAngles = new float[joints.Length];
        feo = new FloatEqualityComparer(1f);
    }

    // Update is called once per frame
    void Update()
    {
        // if (targetPos != target.position)
        // {
        //     solver.StopSolving();
        //     solver.StartSolving(CalculatePenalty, joints.Length);
        // }
        targetPos = target.position;
        for (var i = 0; i < joints.Length; i++)
        {
            previousAngles[i] = joints[i].GetValue();
        }
        var solution = Solve();
        if(!desiredAngles.SequenceEqual(solution, feo))
        {
            desiredAngles = solution;
            // Debug.Log(CalculatePenalty(solution));
        }
        for (var i = 0; i < desiredAngles.Length; i++)
        {
            // var diff = InRange(desiredAngles[i]) - InRange(previousAngles[i]);
            // if(Math.Abs(diff) < 1)
            // {
            //     joints[i].SetValue(desiredAngles[i]);
            //     continue;
            // }
            // var direction = diff > 0 && diff < 180 ? 1 : -1;
            // joints[i].SetValue(previousAngles[i] + direction * speed * Time.deltaTime);
            joints[i].SetValue(desiredAngles[i]);
        }
        // Debug.Log(joints[3].GetValue());
    }

    void OnDestroy()
    {
        solver.StopSolving();
    }

    private float[] Solve()
    {
        if(!solver.IsSolving)
            solver.StartSolving(CalculatePenalty, joints.Length);
        return solver.GetTempResult();
    }

    private float CalculatePenalty(float[] solution)
    {
        return Vector3.Distance(targetPos, EstimateActorPosition(solution));
    }

    private Vector3 EstimateActorPosition(float[] solution)
    {
        var d = 2.0f;

        var system = new CoordinateSystem(baseCoordinateSystem);
        for(var i = 0; i < solution.Length; ++i)
        {
            var degree = joints[i].degreeOfFreedom;
            system.TranslateUp(d);
            system.Rotate(degree, solution[i]);
        }
        system.TranslateUp(d + ActorHeight);
        return system.Pos;
    }

    private float InRange(float value)
    {
        while (value < 0)
        {
            value += 360;
        }

        while (value > 360)
        {
            value -= 360;
        }

        return value;
    }
}
