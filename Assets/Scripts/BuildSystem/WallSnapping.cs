using Environment;
using UnityEngine;

namespace BuildSystem
{
    public class WallSnapping : MonoBehaviour
    {
        public Destroyable destroyable;

        private void Awake()
        {
            destroyable = transform.parent.parent.GetComponent<Destroyable>();
        }

        private void OnTriggerStay(Collider c)
        {
            if (!this.destroyable.isPlaced) return;
            
            Destroyable other = c.GetComponent<Destroyable>();
            
            if (other == null || other.isPlaced || other.isSnapped) return;

            if (c.CompareTag("Wall")) {
                other.isSnapped = true;

                var thisTransform = transform;
                var otherTransform = other.transform;
                otherTransform.position = thisTransform.position;
            }
        }
    }
}