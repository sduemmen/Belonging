using System;
using Flags;
using TMPro;
using UnityEngine;

namespace Utility
{
    public enum ColliderUsage
    {
        SnappingPoint,
        ItemPickup,
        Collision
    }

    public enum ColliderType
    {
        BoxCollider,
        SphereCollider,
        CapsuleCollider,
        MeshCollider
    }

    public class DebugInformation : MonoBehaviour
    {
        private static DebugInformation _instance;
        public bool drawSnapPoints;
        public bool drawCurrentSnapPoint;
        public bool drawCollisionColliders;
        public bool drawItemPickupCollider;
        public bool drawTransformPositions;

        public bool showFps;
        public bool showGameFlags;

        public TextMeshProUGUI fpsLabel;
        public TextMeshProUGUI gameFlagsLabel;
        private float deltaTime;

        public static DebugInformation Instance {
            get {
                if (_instance == null)
                    _instance = (DebugInformation)FindObjectOfType(typeof(DebugInformation));
                return _instance;
            }
        }

        private void Update()
        {
            if (showFps)
            {
                deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
                float fps = 1.0f / deltaTime;
                fpsLabel.text = Mathf.Ceil(fps).ToString();
            }
            else
            {
                fpsLabel.text = "";
            }


            if (showGameFlags)
            {
                string t = "";
                t += nameof(GameFlags.GAME_PAUSED) + "=" + GetColorString(GameFlags.GAME_PAUSED) + "\n";
                t += nameof(GameFlags.MAIN_MENU_ACTIVE) + "=" + GetColorString(GameFlags.MAIN_MENU_ACTIVE) + "\n";

                t += nameof(GameFlags.INVENTORY_OPEN) + "=" + GetColorString(GameFlags.INVENTORY_OPEN) + "\n";
                t += nameof(GameFlags.INVENTORY_CLOSED) + "=" + GetColorString(GameFlags.INVENTORY_CLOSED) + "\n";
                t += nameof(GameFlags.BUILD_MENU_OPEN) + "=" + GetColorString(GameFlags.BUILD_MENU_OPEN) + "\n";
                t += nameof(GameFlags.BUILD_MENU_CLOSED) + "=" + GetColorString(GameFlags.BUILD_MENU_CLOSED) + "\n";
                t += nameof(GameFlags.QUEST_DISPLAY_OPEN) + "=" + GetColorString(GameFlags.QUEST_DISPLAY_OPEN) + "\n";
                t += nameof(GameFlags.QUEST_DISPLAY_CLOSED) + "=" + GetColorString(GameFlags.QUEST_DISPLAY_CLOSED) + "\n";
                t += nameof(GameFlags.UI_ELEMENT_OPEN) + "=" + GetColorString(GameFlags.UI_ELEMENT_OPEN) + "\n";

                t += nameof(GameFlags.AXE_EQUIPPED) + "=" + GetColorString(GameFlags.AXE_EQUIPPED) + "\n";
                t += nameof(GameFlags.PICKAXE_EQUIPPED) + "=" + GetColorString(GameFlags.PICKAXE_EQUIPPED) + "\n";
                t += nameof(GameFlags.HAMMER_EQUIPPED) + "=" + GetColorString(GameFlags.HAMMER_EQUIPPED) + "\n";
                t += nameof(GameFlags.SLOT_EQUIPPED) + "=" + GetColorString(GameFlags.SLOT_EQUIPPED) + "\n";
                gameFlagsLabel.text = t;
            }
            else
            {
                gameFlagsLabel.text = "";
            }
        }

        private static string GetColorString(bool b)
        {
            return b ? "<color=#00ff00>True<color=#4fc0f1>" : "<color=#ff0000>False<color=#4fc0f1>";
        }

        public static Type GetColliderType(ColliderType c)
        {
            switch (c)
            {
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

        public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            DrawLine(pos, direction, 4);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
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

                Vector3 scp1 = c.WorldToScreenPoint(p1);
                Vector3 scp2 = c.WorldToScreenPoint(p2);

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
}