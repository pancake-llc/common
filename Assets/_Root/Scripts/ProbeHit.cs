namespace Gamee.Common
{
    #if UNITY_EDITOR
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Profiling;
    using UnityEngine.Rendering;

    public delegate void ProbeHitSelectHandler(bool add);

    public struct ProbeHit
    {
        public GameObject gameObject;

        public Vector3? point;

        public float? distance;

        public string label;

        public ProbeHitSelectHandler selectHandler;

        public Action focusHandler;

        public Action lostFocusHandler;

        public Transform transform { get => gameObject.transform; set => gameObject = value.gameObject; }

        public RectTransform rectTransform { get => gameObject.GetComponent<RectTransform>(); set => gameObject = value.gameObject; }

        public GameObject groupGameObject;

        public double groupOrder;

        public ProbeHit(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.groupGameObject = gameObject;
            groupOrder = 0;
            point = default;
            distance = default;
            label = default;
            selectHandler = default;
            focusHandler = default;
            lostFocusHandler = default;
        }

        public void Select(bool add)
        {
            if (selectHandler != null)
            {
                selectHandler(add);
            }
            else if (gameObject != null)
            {
                if (add)
                {
                    Selection.objects = Selection.objects.Append(gameObject).ToArray();
                }
                else
                {
                    Selection.activeGameObject = gameObject;
                }
            }
        }

        public void OnFocusEnter()
        {
            if (focusHandler != null)
            {
                focusHandler.Invoke();
            }
            else
            {
                //Highlight
            }
        }

        public void OnFocusLeave()
        {
            if (lostFocusHandler != null)
            {
                lostFocusHandler.Invoke();
            }
            else
            {
                //ClearHighlight
            }
        }
    }

    public static class Probe
    {
        #region Object Picking

        public const int DefaultLimit = 100;

        public static bool canPickHandles =>
            e != null && (e.type == EventType.MouseMove || e.type == EventType.MouseDown || e.type == EventType.MouseUp || e.type == EventType.MouseDrag ||
                          e.type == EventType.MouseEnterWindow || e.type == EventType.MouseLeaveWindow);

        public static ProbeHit? Pick(ProbeFilter filter, SceneView sceneView, Vector2 guiPosition, out Vector3 point)
        {
            var results = ListPool<ProbeHit>.New();

            try
            {
                PickAllNonAlloc(results, filter, sceneView, guiPosition);

                foreach (var result in results)
                {
                    if (result.point.HasValue)
                    {
                        point = result.point.Value;
                        return result;
                    }
                }

                point = DefaultPoint(sceneView, guiPosition);
                return null;
            }
            finally
            {
                results.Free();
            }
        }

        public static void PickAllNonAlloc(List<ProbeHit> hits, ProbeFilter filter, SceneView sceneView, Vector2 guiPosition, int limit = DefaultLimit)
        {
            var screenPosition = HandleUtility.GUIPointToScreenPixelCoordinate(guiPosition);
            var ray3D = HandleUtility.GUIPointToWorldRay(guiPosition);
            var worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);
            var layerMask = Physics.DefaultRaycastLayers;

            var raycastHits = ArrayPool<RaycastHit>.New(limit);
            var overlapHits = ArrayPool<Collider2D>.New(limit);
            var handleHits = HashSetPool<GameObject>.New();
            var ancestorHits = HashSetPool<ProbeHit>.New();
#if PROBUILDER_4_OR_NEWER
			var proBuilderHits = ListPool<ProbeHit>.New();
#endif

            var gameObjectHits = DictionaryPool<GameObject, ProbeHit>.New();

            try
            {
                // Raycast (3D)
                if (filter.raycast)
                {
                    var raycastHitCount = Physics.RaycastNonAlloc(ray3D, raycastHits, Mathf.Infinity, layerMask);

                    for (var i = 0; i < raycastHitCount; i++)
                    {
                        var raycastHit = raycastHits[i];

#if UNITY_2019_2_OR_NEWER
                        if (SceneVisibilityManager.instance.IsHidden(raycastHit.transform.gameObject))
                        {
                            continue;
                        }
#endif

                        var gameObject = raycastHit.transform.gameObject;

                        if (!gameObjectHits.TryGetValue(gameObject, out var hit))
                        {
                            hit = new ProbeHit(gameObject);
                        }

                        hit.point = raycastHit.point;
                        hit.distance = raycastHit.distance;

                        gameObjectHits[gameObject] = hit;
                    }
                }

                // Overlap (2D)
                if (filter.overlap)
                {
                    var overlapHitCount = Physics2D.OverlapPointNonAlloc(worldPosition, overlapHits, layerMask);

                    for (var i = 0; i < overlapHitCount; i++)
                    {
                        var overlapHit = overlapHits[i];

#if UNITY_2019_2_OR_NEWER
                        if (SceneVisibilityManager.instance.IsHidden(overlapHit.transform.gameObject))
                        {
                            continue;
                        }
#endif

                        var gameObject = overlapHit.transform.gameObject;

                        if (!gameObjectHits.TryGetValue(gameObject, out var hit))
                        {
                            hit = new ProbeHit(gameObject);
                        }

                        hit.distance = hit.distance ?? Vector3.Distance(overlapHit.transform.position, worldPosition);

                        gameObjectHits[gameObject] = hit;
                    }
                }

                // Handles (Editor Default)
                if (filter.handles && canPickHandles)
                {
                    PickAllHandlesNonAlloc(handleHits, guiPosition, limit);

                    foreach (var handleHit in handleHits)
                    {
                        var gameObject = handleHit;

                        if (!gameObjectHits.TryGetValue(gameObject, out var hit))
                        {
                            hit = new ProbeHit(gameObject);
                        }

                        hit.distance = hit.distance ?? Vector3.Distance(handleHit.transform.position, worldPosition);

                        gameObjectHits[gameObject] = hit;
                    }
                }

                // Ancestors
                foreach (var gameObjectHit in gameObjectHits)
                {
                    var gameObject = gameObjectHit.Key;
                    var hit = gameObjectHit.Value;

                    var parent = gameObject.transform.parent;

                    int depth = 0;

                    while (parent != null)
                    {
                        var parentGameObject = parent.gameObject;

                        var parentHit = new ProbeHit(parentGameObject);
                        parentHit.groupGameObject = gameObject;
                        parentHit.distance = hit.distance ?? Vector3.Distance(parentHit.transform.position, worldPosition);
                        parentHit.groupOrder = 1000 + depth;

                        ancestorHits.Add(parentHit);

                        parent = parent.parent;
                        depth++;
                    }
                }

                // Prepare final hits
                hits.Clear();

                // Add hits
                foreach (var gameObjectHit in gameObjectHits.Values)
                {
                    hits.Add(gameObjectHit);
                }

                foreach (var ancestorHit in ancestorHits)
                {
                    hits.Add(ancestorHit);
                }

                // Sort by distance
                hits.Sort(compareHits);
            }
            finally
            {
                raycastHits.Free();
                overlapHits.Free();
                handleHits.Free();
                ancestorHits.Free();

                gameObjectHits.Free();
            }
        }

        private static void PickAllHandlesNonAlloc(HashSet<GameObject> results, Vector2 position, int limit = DefaultLimit)
        {
            if (!canPickHandles)
            {
                // HandleUtility.PickGameObject is not supported in those contexts
                Debug.LogWarning($"Cannot pick game objects in the current event: {e?.ToString() ?? "null"}");
                return;
            }

            GameObject result = null;

            var count = 0;

            do
            {
                var ignored = results.ToArray();

                result = HandleUtility.PickGameObject(position, false, ignored);

                // Ignored doesn't seem very reliable. Sometimes, an item included
                // in ignored will still be returned. That's a sign we should stop.
                if (results.Contains(result))
                {
                    result = null;
                }

                if (result != null)
                {
                    results.Add(result);
                }
            } while (result != null && count++ < limit);
        }

        private static readonly Comparison<ProbeHit> compareHits = CompareHits;

        private static int CompareHits(ProbeHit a, ProbeHit b)
        {
            var distanceA = a.distance ?? Mathf.Infinity;
            var distanceB = b.distance ?? Mathf.Infinity;
            return distanceA.CompareTo(distanceB);
        }

        private static Vector3 DefaultPoint(SceneView sceneView, Vector2 guiPosition)
        {
            var screenPosition = (Vector3)HandleUtility.GUIPointToScreenPixelCoordinate(guiPosition);
            screenPosition.z = sceneView.cameraDistance;
            return sceneView.camera.ScreenToWorldPoint(screenPosition);
        }

        #endregion


        #region Scene View Integration

        private static Event e => Event.current;

        private static Vector2? pressPosition;

        #endregion
    }

    public struct ProbeFilter
    {
        public bool raycast { get; set; }
        public bool overlap { get; set; }
        public bool handles { get; set; }
        public bool proBuilder { get; set; }

        public static ProbeFilter @default { get; } = new ProbeFilter
        {
            raycast = true, overlap = true, handles = true, proBuilder = true,
        };
    }

#endif
}