using UnityEngine;
using System;

[Serializable]
public class Trajectory 
{
    //#region Line renderer veriables
    //[Header("Line renderer veriables")]
    //public LineRenderer line;
    //[Range(2, 30)]
    //public int resolution;
    //#endregion

    [Header("Formula variables")]
    public Vector3 velocity;
    public float yLimit;
    private float g;

    [Header("Linecast variables")]
    [Range(2, 30)]
    public int linecastResolution;
    public LayerMask canHit;

    Transform trajectoryBeginTransform;

    //private Vector3[] trajectoryPositions;

    public void initialize(Transform trajectoryBeginTr)
    {
        g = Mathf.Abs(Physics.gravity.y);
        trajectoryBeginTransform = trajectoryBeginTr;

        //trajectoryPositions = new Vector3[linecastResolution];
    }


    public void updateTrajectory(Vector3 velocity)
    {
        this.velocity = velocity;
        //RenderArc();
    }
    //private void Update()
    //{
    //    RenderArc();
    //}

    //private void RenderArc()
    //{
    //    line.positionCount = resolution + 1;
    //    line.SetPositions(CalculateLineArray());
    //}

    //private Vector3[] CalculateLineArray()
    //{
    //    Vector3[] lineArray = new Vector3[resolution + 1];

    //    var lowestTimeValueX = MaxTimeX() / resolution;
    //    var lowestTimeValueZ = MaxTimeZ() / resolution;
    //    var lowestTimeValue = lowestTimeValueX > lowestTimeValueZ ? lowestTimeValueZ : lowestTimeValueX;

    //    for (int i = 0; i < lineArray.Length; i++)
    //    {
    //        var t = lowestTimeValue * i;
    //        lineArray[i] = CalculateLinePoint(t);
    //    }

    //    return lineArray;
    //}

    public Vector3 getHitPos(Vector3 velocity)
    {
        this.velocity = velocity;
        return HitPosition();
    }

    private Vector3 HitPosition()
    {
        var lowestTimeValue = MaxTimeY() / linecastResolution;

        for (int i = 0; i < linecastResolution + 1; i++)
        {
            RaycastHit rayHit;

            var t = lowestTimeValue * i;
            var tt = lowestTimeValue * (i + 1);

            if (Physics.Linecast(CalculateLinePoint(t), CalculateLinePoint(tt), out rayHit, canHit))
            {
                return rayHit.point;
            }

        }

        return CalculateLinePoint(MaxTimeY());
    }

    private Vector3 CalculateLinePoint(float t)
    {
        float x = velocity.x * t;
        float z = velocity.z * t;
        float y = (velocity.y * t) - (g * Mathf.Pow(t, 2) / 2);
        return new Vector3(x + trajectoryBeginTransform.position.x, y + trajectoryBeginTransform.position.y, z + trajectoryBeginTransform.position.z);
    }

    private float MaxTimeY()
    {
        var v = velocity.y;
        var vv = v * v;
        if (trajectoryBeginTransform == null) Debug.Log("trajevtrory is null");
        var t = (v + Mathf.Sqrt(vv + 2 * g * (trajectoryBeginTransform.position.y - yLimit))) / g;
        return t;
    }

    private float MaxTimeX()
    {
        if (IsValueAlmostZero(velocity.x))
            SetValueToAlmostZero(ref velocity.x);

        var x = velocity.x;

        var t = (HitPosition().x - trajectoryBeginTransform.position.x) / x;
        return t;
    }

    private float MaxTimeZ()
    {
        if (IsValueAlmostZero(velocity.z))
            SetValueToAlmostZero(ref velocity.z);

        var z = velocity.z;

        var t = (HitPosition().z - trajectoryBeginTransform.position.z) / z;
        return t;
    }

    private bool IsValueAlmostZero(float value)
    {
        return value < 0.0001f && value > -0.0001f;
    }

    private void SetValueToAlmostZero(ref float value)
    {
        value = 0.0001f;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }
}
