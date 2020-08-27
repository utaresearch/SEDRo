using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Vector3 GetGlobalToLocalScaleFactor(Transform t)
    {
        Vector3 factor = Vector3.one;

        while (true)
        {
            Transform tParent = t.parent;

            if (tParent != null)
            {
                factor.x *= tParent.localScale.x;
                factor.y *= tParent.localScale.y;
                factor.z *= tParent.localScale.z;

                t = tParent;
            }
            else
            {
                return factor;
            }
        }
    }

    public static float Vector2DDistance(Vector3 v1, Vector3 v2)
    {
        float xDiff = v1.x - v2.x;
        float zDiff = v1.z - v2.z;
        return Mathf.Sqrt((xDiff * xDiff) + (zDiff * zDiff));
    }
}
