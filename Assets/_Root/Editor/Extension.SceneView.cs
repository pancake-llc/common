using Pancake.Common;
using UnityEditor;
using UnityEngine;

namespace Pancake.Editor
{
    public static partial class UtilEditor
    {
        public static ProbeHit? GetMousePosition(out Vector3 mousePosition, SceneView sceneView)
        {
            var e = Event.current;
            var position = sceneView.GetInnerGuiPosition();

            mousePosition = Vector3.zero;
            if (position.Contains(e.mousePosition))
            {
                var filter = ProbeFilter.Default;
                filter.ProBuilder = false; // Too slow and useless here anyway
                var hit = Probe.Pick(filter, sceneView, e.mousePosition, out var point);
                if (e.type == EventType.MouseDown && e.button == 0) mousePosition = point;

                return hit;
            }

            return null;
        }

        public static bool Get2DMouseScenePosition(out Vector2 result)
        {
            result = Vector2.zero;

            var cam = Camera.current;
            var mp = Event.current.mousePosition;
            mp.y = cam.pixelHeight - mp.y;
            var ray = cam.ScreenPointToRay(mp);
            if (ray.direction != Vector3.forward) return false;

            result = ray.origin;
            return true;
        }

        public static Rect GetInnerGuiPosition(this SceneView sceneView)
        {
            var position = sceneView.position;
            position.x = position.y = 0;
            position.height -= EditorStyles.toolbar.fixedHeight;
            return position;
        }

        /// <summary>
        /// Render an object on sceneView using sprite renderers
        /// </summary>
        public static void FakeRenderSprite(GameObject obs, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            var rends = obs.GetComponentsInChildren<SpriteRenderer>();
            foreach (var rend in rends)
            {
                var bounds = rend.bounds;
                var pos = rend.transform.position - obs.transform.position + position;
                DrawSprite(rend.sprite, pos, bounds.size.Multiply(scale));
            }
        }

        /// <summary>
        /// get inspector type to display window
        /// </summary>
        public static System.Type InspectorWindow => typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
    }
}