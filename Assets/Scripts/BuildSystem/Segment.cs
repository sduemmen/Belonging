using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildSystem
{
    [Serializable]
    public class Segment : MonoBehaviour
    {
        public string m_name;

        public Sprite m_icon;

        public string m_description;
        
        public List<ItemStack> m_requirements = new List<ItemStack>();

        public FXList m_placementFX;

        public bool m_enableSnapping = true;

        public bool m_needsGroundContact;

        public bool m_needsCeilingContact;

        public bool m_needsWallContact;

        public bool m_isLightSource;

        private static Collider[] segmentColliders = new Collider[500];
        

        public static void GetSnapPointsInRadius(Vector3 center, float radius, List<Transform> snapPointsOut, List<Segment> segmentsOut)
        {
            int colliderCount = Physics.OverlapSphereNonAlloc(center, radius, segmentColliders, LayerMask.GetMask("Segment"));
            for (int i = 0; i < colliderCount; i++)
            {
                Segment segment = segmentColliders[i].GetComponentInParent<Segment>();
                if (segment != null)
                {
                    segment.GetOwnSnapPoints(snapPointsOut);
                    segmentsOut.Add(segment);
                }
            }
        }
        
        public void GetOwnSnapPoints(List<Transform> snapPointsOut)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.CompareTag("Snappoint"))
                {
                    snapPointsOut.Add(child);
                }
            }
        }
    }
}