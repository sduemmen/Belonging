using UnityEngine;

namespace Utility
{
    public class ColliderDebug : MonoBehaviour
    {
        public ColliderDebugInformation colliderInformation;
        private Collider c;

        private void Awake()
        {
            if (colliderInformation == null) Debug.LogError($"Collider Debug Information value is uninitialized on GameObject {gameObject.name}");
        }

        private void OnDrawGizmos()
        {
            if (DebugInformation.Instance == null || colliderInformation == null) return;
            if (!DebugInformation.Instance.drawSnapPoints && colliderInformation.usage == ColliderUsage.SnappingPoint) return;
            if (!DebugInformation.Instance.drawCollisionColliders && colliderInformation.usage == ColliderUsage.Collision) return;
            if (!DebugInformation.Instance.drawItemPickupCollider && colliderInformation.usage == ColliderUsage.ItemPickup) return;

            if (c == null)
            {
                c = (Collider)GetComponent(DebugInformation.GetColliderType(colliderInformation.type));
                if (c == null)
                {
                    Debug.LogError($"Couldn't find {colliderInformation.type.ToString()} component on GameObject {gameObject.name}");
                }
            }

            Gizmos.color = colliderInformation.color;

            if (c != null && colliderInformation != null)
            {
                Bounds bounds = c.bounds;
                if (colliderInformation.type == ColliderType.BoxCollider) DebugExtension.DebugBounds(bounds, Gizmos.color, depthTest: false);

                if (colliderInformation.type == ColliderType.SphereCollider) DebugExtension.DebugWireSphere(bounds.center, Gizmos.color, bounds.extents.x, depthTest: false);

                if (colliderInformation.type == ColliderType.CapsuleCollider)
                {
                    CapsuleCollider capsuleCollider = (CapsuleCollider)c;
                    Vector3 heightOffset = new Vector3(0, capsuleCollider.height / 2, 0);
                    Vector3 center = capsuleCollider.center;
                    DebugExtension.DebugCapsule(center + heightOffset, center - heightOffset, Gizmos.color, capsuleCollider.radius, depthTest: false);
                }

                if (colliderInformation.type == ColliderType.MeshCollider)
                {
                    MeshCollider meshCollider = (MeshCollider)c;
                    Transform t = transform;

                    Gizmos.DrawWireMesh(meshCollider.sharedMesh, t.position, t.rotation, t.localScale);
                }
            }
        }
    }
}