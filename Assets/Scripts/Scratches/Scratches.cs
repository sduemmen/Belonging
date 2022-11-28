using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Scratches
{
    public class Scratches : MonoBehaviour
    {
        public List<Vector3> vectors;

        private void Awake()
        {
            List<Vector3> v = Utils.Math.GramSchmidtOrthogonalization(vectors, true);

            foreach (Vector3 vector3 in v)
            {
                Debug.Log(vector3);
            }
        }
    }
}