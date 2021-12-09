#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Snorlax.Common
{
    public static partial class Common
    {
        public static bool GetMousePosition(out Vector3 mousePosition, SceneView sceneView)
        {
            Event e = Event.current;
            var position = sceneView.GetInnerGuiPosition();

            mousePosition = Vector3.zero;
            if (position.Contains(e.mousePosition))
            {
                var filter = ProbeFilter.@default;
                filter.proBuilder = false; // Too slow and useless here anyway
                var hit = Probe.Pick(filter, sceneView, e.mousePosition, out var point);

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    mousePosition = point;
                }

                return true;
            }

            return false;
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
        public static void FakeRenderSprite(
            GameObject obs,
            Vector3 position,
            Vector3 scale,
            Quaternion rotation)
        {
            var rends = obs.GetComponentsInChildren<SpriteRenderer>();
            foreach (var rend in rends)
            {
                var bounds = rend.bounds;
                var pos = rend.transform.position - obs.transform.position + position;
                DrawSprite(rend.sprite, pos, bounds.size.Multiply(scale));
            }
        }
        
        public static System.Type GetInspectorWindowType()
        {
            var editorAsm = typeof(UnityEditor.Editor).Assembly;
            var inspWndType = editorAsm.GetType("UnityEditor.InspectorWindow");
            return inspWndType;
        }
    }
}
#endif