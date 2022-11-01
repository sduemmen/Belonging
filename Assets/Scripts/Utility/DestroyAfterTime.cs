using System.Collections;
using UnityEngine;

namespace Utility
{
    public class DestroyAfterTime : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(Something());
        }

        private IEnumerator Something()
        {
            yield return new WaitForSeconds(3.0f);
            Destroy(gameObject);
        }
    }
}