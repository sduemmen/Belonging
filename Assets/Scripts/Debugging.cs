using UnityEngine;

public class Debugging : MonoBehaviour
{
    public bool drawLocalDirections;
    public bool drawVelocity;

    private void OnDrawGizmos()
    {
        if (drawLocalDirections) {
            var t = transform;
            var position = t.position;
            DrawArrow(position, t.right, Color.red);
            DrawArrow(position, t.up, Color.green);
            DrawArrow(position, t.forward, Color.blue);
        }

        if (drawVelocity) {
            Rigidbody t = GetComponent<Rigidbody>();
            if (t == null) {
                Debug.LogWarning("Couldn't find Rigidbody component on GameObject " + transform.name);
            } else {
                Vector3 velocity = t.velocity;
                if (velocity != Vector3.zero) {
                    DrawArrow(transform.position, velocity, Color.red);
                }
            }
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
