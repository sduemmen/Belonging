using System;
using UnityEngine;

public class Debugging : MonoBehaviour
{
    public enum ColliderType
    {
        BoxCollider,
        SphereCollider,
        CapsuleCollider,
        MeshCollider,
    }
    public bool drawLocalDirections;
    public bool drawVelocity;
    public bool drawCollider;
    public ColliderType[] colliderTypes;

    private void OnDrawGizmos()
    {
        if (drawLocalDirections) {
            Transform t = transform;
            if (t == null) {
                Debug.LogError($"Couldn't find Transform component on GameObject {this.gameObject.name}");
            } else {
                Vector3 position = t.position;
                DrawArrow(position, t.right, Color.red);
                DrawArrow(position, t.forward, Color.blue);
                DrawArrow(position, t.up, Color.green);
            }
        }

        if (drawVelocity) {
            Rigidbody t = GetComponent<Rigidbody>();
            if (t == null) {
                Debug.LogError($"Couldn't find Rigidbody component on GameObject {this.gameObject.name}");
            } else {
                Vector3 velocity = t.velocity;
                if (velocity != Vector3.zero) {
                    DrawArrow(transform.position, velocity, Color.magenta);
                }
            }
        }

        if (drawCollider) {
            Gizmos.color = Color.green;
            foreach (ColliderType colliderType in colliderTypes) {
                Collider c = (Collider) GetComponent(GetColliderType(colliderType));
                if (c == null) {
                    Debug.LogError($"Couldn't find {colliderType.ToString()} component on GameObject {this.gameObject.name}");
                } else {
                    var bounds = c.bounds;
                    if (colliderType == ColliderType.BoxCollider) 
                        Gizmos.DrawWireCube(bounds.center, bounds.size);
                    
                    if (colliderType == ColliderType.SphereCollider) 
                        Gizmos.DrawWireSphere(bounds.center, bounds.extents.x);
                    
                    if (colliderType == ColliderType.CapsuleCollider) {
                        CapsuleCollider capsuleCollider = (CapsuleCollider)c;
                        float diameter = capsuleCollider.radius * 2;
                        Gizmos.DrawWireCube(capsuleCollider.center, new Vector3(diameter, capsuleCollider.height, diameter));
                    }
                    
                    if (colliderType == ColliderType.MeshCollider) {
                        MeshCollider meshCollider = (MeshCollider)c;
                        Transform t = transform;
                        Gizmos.DrawWireMesh(meshCollider.sharedMesh, t.position, t.rotation, t.localScale);
                    }
                }
            }
        }
    }

    public Type GetColliderType(ColliderType c)
    {
        switch (c) {
            case ColliderType.BoxCollider:
                return typeof(BoxCollider);
            case ColliderType.SphereCollider:
                return typeof(SphereCollider);
            case ColliderType.CapsuleCollider:
                return typeof(CapsuleCollider);
            case ColliderType.MeshCollider:
                return typeof(MeshCollider);
            default:
                return null;
        }
    }
    
    public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        DrawLine(pos, direction, 4);
       
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        DrawLine(pos + direction, right * arrowHeadLength, 4);
        DrawLine(pos + direction, left * arrowHeadLength, 4);
    }
 
    public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        DrawLine(pos, direction, 4);
       
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        DrawLine(pos + direction, right * arrowHeadLength, 4);
        DrawLine(pos + direction, left * arrowHeadLength, 4);
    }
    
    public static void DrawLine(Vector3 p1, Vector3 p2, float width)
    {
        int count = 1 + Mathf.CeilToInt(width); // how many lines are needed.
        if (count == 1)
        {
            Gizmos.DrawRay(p1, p2);
        }
        else
        {
            Camera c = Camera.current;
            if (c == null)
            {
                Debug.LogError("Camera.current is null");
                return;
            }
            var scp1 = c.WorldToScreenPoint(p1);
            var scp2 = c.WorldToScreenPoint(p2);
 
            Vector3 v1 = (scp2 - scp1).normalized; // line direction
            Vector3 n = Vector3.Cross(v1, Vector3.forward); // normal vector
 
            for (int i = 0; i < count; i++)
            {
                Vector3 o = 0.5f * n * width * ((float)i / (count - 1) - 0.5f);
                Vector3 origin = c.ScreenToWorldPoint(scp1 + o);
                Gizmos.DrawRay(origin, p2);
            }
        }
    }
}
