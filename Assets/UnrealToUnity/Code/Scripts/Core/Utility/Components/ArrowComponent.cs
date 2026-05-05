using System;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility.Components
{
    public class ArrowComponent : MonoBehaviour
    {
        [SerializeField] private Color color = Color.red;
        [SerializeField] private float scale = 1f;
        [SerializeField] private float arrowHeadAngle = 70f;

        private void OnDrawGizmos()
        {
            // Draw an arrow

            var center = transform.position;
            var lineStart = center - (transform.forward * scale * 0.5f);
            var lineEnd = center + (transform.forward * scale * 0.5f);

            var leftArrowHeadEnd =
                lineEnd - (Quaternion.Euler(0, arrowHeadAngle / 2, 0) * transform.forward * scale * 0.25f);
            var rightArrowHeadEnd =
                lineEnd - (Quaternion.Euler(0, -arrowHeadAngle / 2, 0) * transform.forward * scale * 0.25f);

            Gizmos.color = color;
            Gizmos.DrawLine(lineStart, lineEnd);
            Gizmos.DrawLine(lineEnd, leftArrowHeadEnd);
            Gizmos.DrawLine(lineEnd, rightArrowHeadEnd);
        }
    }
}