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
}
