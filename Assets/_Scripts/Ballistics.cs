//using UnityEngine;
//using System;

//public static class Ballistics
//{



//    // Note, doesn't take drag into account.

//    /// <summary>
//    /// Calculate the lanch angle.
//    /// </summary>
//    /// <returns>Angle to be fired on.</returns>
//    /// <param name="start">The muzzle.</param>
//    /// <param name="end">Wanted hit point.</param>
//    /// <param name="muzzleVelocity">Muzzle velocity.</param>
//    public static bool CalculateTrajectory(Vector3 start, Vector3 end, float muzzleVelocity, out float angle)
//    {//, out float highAngle){

//        Vector3 dir = end - start;
//        float vSqr = muzzleVelocity * muzzleVelocity;
//        float y = dir.y;
//        dir.y = 0.0f;
//        float x = dir.sqrMagnitude;
//        float g = -Physics.gravity.y;

//        float uRoot = vSqr * vSqr - g * (g * (x) + (2.0f * y * vSqr));


//        if (uRoot < 0.0f)
//        {

//            //target out of range.
//            angle = -45.0f;
//            //highAngle = -45.0f;
//            return false;
//        }

//        //        float r = Mathf.Sqrt (uRoot);
//        //        float bottom = g * Mathf.Sqrt (x);

//        angle = -Mathf.Atan2(g * Mathf.Sqrt(x), vSqr + Mathf.Sqrt(uRoot)) * Mathf.Rad2Deg;
//        //highAngle = -Mathf.Atan2 (bottom, vSqr - r) * Mathf.Rad2Deg;
//        return true;

//    }

//    /// <summary>
//    /// Gets the ballistic path.
//    /// </summary>
//    /// <returns>The ballistic path.</returns>
//    /// <param name="startPos">Start position.</param>
//    /// <param name="forward">Forward direction.</param>
//    /// <param name="velocity">Velocity.</param>
//    /// <param name="timeResolution">Time from frame to frame.</param>
//    /// <param name="maxTime">Max time to simulate, will be clamped to reach height 0 (aprox.).</param>

//    public static Vector3[] GetBallisticPath(Vector3 startPos, Vector3 forward, float velocity, float timeResolution, float maxTime = Mathf.Infinity)
//    {

//        maxTime = Mathf.Min(maxTime, Ballistics.GetTimeOfFlight(velocity, Vector3.Angle(forward, Vector3.up) * Mathf.Deg2Rad, startPos.y));
//        Vector3[] positions = new Vector3[Mathf.CeilToInt(maxTime / timeResolution)];
//        Vector3 velVector = forward * velocity;
//        int index = 0;
//        Vector3 curPosition = startPos;
//        for (float t = 0.0f; t < maxTime; t += timeResolution)
//        {

//            if (index >= positions.Length)
//                break;//rounding error using certain values for maxTime and timeResolution

//            positions[index] = curPosition;
//            curPosition += velVector * timeResolution;
//            velVector += Physics.gravity * timeResolution;
//            index++;
//        }
//        return positions;
//    }

//    /// <summary>
//    /// Checks the ballistic path for collisions.
//    /// </summary>
//    /// <returns><c>false</c>, if ballistic path was blocked by an object on the Layermask, <c>true</c> otherwise.</returns>
//    /// <param name="arc">Arc.</param>
//    /// <param name="lm">Anything in this layer will block the path.</param>
//    public static bool CheckBallisticPath(Vector3[] arc, LayerMask lm)
//    {

//        RaycastHit hit;
//        for (int i = 1; i < arc.Length; i++)
//        {

//            if (Physics.Raycast(arc[i - 1], arc[i] - arc[i - 1], out hit, (arc[i] - arc[i - 1]).magnitude) && GameMaster.IsInLayerMask(hit.transform.gameObject.layer, lm))
//                return false;

//            //            if (Physics.Raycast (arc [i - 1], arc [i] - arc [i - 1], out hit, (arc [i] - arc [i - 1]).magnitude) && GameMaster.IsInLayerMask(hit.transform.gameObject.layer, lm)) {
//            //                Debug.DrawRay (arc [i - 1], arc [i] - arc [i - 1], Color.red, 10f);
//            //                return false;
//            //            } else {
//            //                Debug.DrawRay (arc [i - 1], arc [i] - arc [i - 1], Color.green, 10f);
//            //            }
//        }
//        return true;
//    }

//    public static Vector3 GetHitPosition(Vector3 startPos, Vector3 forward, float velocity)
//    {

//        Vector3[] path = GetBallisticPath(startPos, forward, velocity, .35f);
//        RaycastHit hit;
//        for (int i = 1; i < path.Length; i++)
//        {

//            //Debug.DrawRay (path [i - 1], path [i] - path [i - 1], Color.red, 10f);
//            if (Physics.Raycast(path[i - 1], path[i] - path[i - 1], out hit, (path[i] - path[i - 1]).magnitude))
//            {
//                return hit.point;
//            }
//        }

//        return Vector3.zero;
//    }


//    public static float CalculateMaxRange(float muzzleVelocity)
//    {
//        return (muzzleVelocity * muzzleVelocity) / -Physics.gravity.y;
//    }

//    public static float GetTimeOfFlight(float vel, float angle, float height)
//    {

//        return (2.0f * vel * Mathf.Sin(angle)) / -Physics.gravity.y;
//    }

//}