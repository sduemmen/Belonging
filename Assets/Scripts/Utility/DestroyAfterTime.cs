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
 
        IEnumerator Something()
        {
            yield return new WaitForSeconds(3.0f);
            Destroy(this.gameObject);
        }
    }
}
