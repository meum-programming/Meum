using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gallery.Builder
{
    public class WallInfo : MonoBehaviour
    {
        public float placeHeight;
        public float placeDistance;

        public Vector3 GetNormalDir(Vector3 camPos)
        {
            var selfTransform = transform;
            Vector3 dir = camPos - selfTransform.position;

            var forward = selfTransform.forward;
            var right = selfTransform.right;
            var up = selfTransform.up;

            var dotForward = Vector3.Dot(forward, dir);
            var dotRight = Vector3.Dot(right, dir);
            var dotUp = Vector3.Dot(up, dir);

            if (Mathf.Abs(dotForward) > Mathf.Abs(dotRight) && Mathf.Abs(dotForward) > Mathf.Abs(dotUp))
                return dotForward > 0 ? forward : -forward;
            if (Mathf.Abs(dotRight) > Mathf.Abs(dotUp))
                return dotRight > 0 ? right : -right;
            return dotUp > 0 ? up : -up;
        }
    }
}
