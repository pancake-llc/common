using System.Collections.Generic;

namespace Gamee.Common
{
    using UnityEngine;

    public static partial class Util
    {
        /// <summary>
        /// Makes a copy of the Vector2 with changed x/y values, keeping all undefined values as they were before. Can be
        /// called with named parameters like vector.Change2(y: 5), for example, only changing the y component.
        /// </summary>
        /// <param name="vector">The Vector2 to be copied with changed values.</param>
        /// <param name="x">If this is not null, the x component is set to this value.</param>
        /// <param name="y">If this is not null, the y component is set to this value.</param>
        /// <returns>A copy of the Vector2 with changed values.</returns>
        public static Vector2 Change(this Vector2 vector, float? x = null, float? y = null)
        {
            if (x.HasValue) vector.x = x.Value;
            if (y.HasValue) vector.y = y.Value;
            return vector;
        }

        /// <summary>
        /// Makes a copy of the Vector3 with changed x/y/z values, keeping all undefined values as they were before. Can be
        /// called with named parameters like vector.Change3(x: 5, z: 10), for example, only changing the x and z components.
        /// </summary>
        /// <param name="vector">The Vector3 to be copied with changed values.</param>
        /// <param name="x">If this is not null, the x component is set to this value.</param>
        /// <param name="y">If this is not null, the y component is set to this value.</param>
        /// <param name="z">If this is not null, the z component is set to this value.</param>
        /// <returns>A copy of the Vector3 with changed values.</returns>
        public static Vector3 Change(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            if (x.HasValue) vector.x = x.Value;
            if (y.HasValue) vector.y = y.Value;
            if (z.HasValue) vector.z = z.Value;
            return vector;
        }

        /// <summary>
        /// Makes a copy of the Vector4 with changed x/y/z/w values, keeping all undefined values as they were before. Can be
        /// called with named parameters like vector.Change4(x: 5, z: 10), for example, only changing the x and z components.
        /// </summary>
        /// <param name="vector">The Vector4 to be copied with changed values.</param>
        /// <param name="x">If this is not null, the x component is set to this value.</param>
        /// <param name="y">If this is not null, the y component is set to this value.</param>
        /// <param name="z">If this is not null, the z component is set to this value.</param>
        /// <param name="w">If this is not null, the w component is set to this value.</param>
        /// <returns>A copy of the Vector4 with changed values.</returns>
        public static Vector4 Change(this Vector4 vector, float? x = null, float? y = null, float? z = null, float? w = null)
        {
            if (x.HasValue) vector.x = x.Value;
            if (y.HasValue) vector.y = y.Value;
            if (z.HasValue) vector.z = z.Value;
            if (w.HasValue) vector.w = w.Value;
            return vector;
        }

        /// <summary>
        /// Rotates a Vector2.
        /// </summary>
        /// <param name="v">The Vector2 to rotate.</param>
        /// <param name="angleRad">How far to rotate the Vector2 in radians.</param>
        /// <returns>The rotated Vector2.</returns>
        public static Vector2 RotateRad(this Vector2 v, float angleRad)
        {
            // http://answers.unity3d.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html
            var sin = Mathf.Sin(angleRad);
            var cos = Mathf.Cos(angleRad);

            var tx = v.x;
            var ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);

            return v;
        }

        /// <summary>
        /// Rotates a Vector2.
        /// </summary>
        /// <param name="v">The Vector2 to rotate.</param>
        /// <param name="angleDeg">How far to rotate the Vector2 in degrees.</param>
        /// <returns>The rotated Vector2.</returns>
        public static Vector2 RotateDeg(this Vector2 v, float angleDeg) { return v.RotateRad(angleDeg * Mathf.Deg2Rad); }

        /// <summary>
        /// Creates a Vector2 with a length of 1 pointing towards a certain angle.
        /// </summary>
        /// <param name="angleRad">The angle in radians.</param>
        /// <returns>The Vector2 pointing towards the angle.</returns>
        public static Vector2 CreateVector2AngleRad(this float angleRad) { return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)); }

        /// <summary>
        /// Creates a Vector2 with a length of 1 pointing towards a certain angle.
        /// </summary>
        /// <param name="angleDeg">The angle in degrees.</param>
        /// <returns>The Vector2 pointing towards the angle.</returns>
        public static Vector2 CreateVector2AngleDeg(this float angleDeg) { return CreateVector2AngleRad(angleDeg * Mathf.Deg2Rad); }

        /// <summary>
        /// Gets the rotation of a Vector2.
        /// </summary>
        /// <param name="vector">The Vector2.</param>
        /// <returns>The rotation of the Vector2 in radians.</returns>
        public static float GetAngleRad(this Vector2 vector) { return Mathf.Atan2(vector.y, vector.x); }

        /// <summary>
        /// Gets the rotation of a Vector2.
        /// </summary>
        /// <param name="vector">The Vector2.</param>
        /// <returns>The rotation of the Vector2 in degrees.</returns>
        public static float GetAngleDeg(this Vector2 vector) { return vector.GetAngleRad() * Mathf.Rad2Deg; }

        /// <summary>
        /// set new value of <paramref name="pivot"/> for <paramref name="rectTransform"/>
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="pivot"></param>
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            if (rectTransform == null) return;

            var size = rectTransform.rect.size;
            var deltaPivot = rectTransform.pivot - pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        /// <summary>
        /// Sets the x/y/z transform.position using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetPosition(x: 5, z: 10), for example, only changing transform.position.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.position at.</param>
        /// <param name="x">If this is not null, transform.position.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.position.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.position.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetPosition(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            transform.position = transform.position.Change(x, y, z);
            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.localPosition using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetLocalPosition(x: 5, z: 10), for example, only changing transform.localPosition.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.localPosition at.</param>
        /// <param name="x">If this is not null, transform.localPosition.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.localPosition.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.localPosition.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLocalPosition(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            transform.localPosition = transform.localPosition.Change(x, y, z);
            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.localScale using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetLocalScale(x: 5, z: 10), for example, only changing transform.localScale.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.localScale at.</param>
        /// <param name="x">If this is not null, transform.localScale.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.localScale.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.localScale.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLocalScale(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            transform.localScale = transform.localScale.Change(x, y, z);
            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.lossyScale using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetLossyScale(x: 5, z: 10), for example, only changing transform.lossyScale.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.lossyScale at.</param>
        /// <param name="x">If this is not null, transform.lossyScale.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.lossyScale.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.lossyScale.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLossyScale(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            var lossyScale = transform.lossyScale.Change(x, y, z);

            transform.localScale = Vector3.one;
            // ReSharper disable once Unity.InefficientPropertyAccess
            transform.localScale = new Vector3(lossyScale.x / transform.lossyScale.x,
                // ReSharper disable once Unity.InefficientPropertyAccess
                lossyScale.y / transform.lossyScale.y,
                // ReSharper disable once Unity.InefficientPropertyAccess
                lossyScale.z / transform.lossyScale.z);

            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.eulerAngles using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetEulerAngles(x: 5, z: 10), for example, only changing transform.eulerAngles.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.eulerAngles at.</param>
        /// <param name="x">If this is not null, transform.eulerAngles.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.eulerAngles.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.eulerAngles.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetEulerAngles(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            transform.eulerAngles = transform.eulerAngles.Change(x, y, z);
            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.localEulerAngles using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetLocalEulerAngles(x: 5, z: 10), for example, only changing transform.localEulerAngles.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.localEulerAngles at.</param>
        /// <param name="x">If this is not null, transform.localEulerAngles.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.localEulerAngles.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.localEulerAngles.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLocalEulerAngles(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            transform.localEulerAngles = transform.localEulerAngles.Change(x, y, z);
            return transform;
        }

        /// <summary>
        /// Reset transform setting
        /// Scale to one
        /// Position to zero
        /// Roration to zero
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Transform Reset(this Transform transform)
        {
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            return transform;
        }

        /// <summary>
        /// Destroys a transform's children
        /// </summary>
        /// <param name="transform"></param>
        public static void RemoveAllChildren(this Transform transform)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(transform.GetChild(i).gameObject);
                }
                else
                {
                    Object.DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }

        /// <summary>
        /// Finds children by name, breadth first
        /// </summary>
        /// <param name="root"></param>
        /// <param name="transformName"></param>
        /// <returns></returns>
        public static Transform FindDeepChildBreadthFirst(this Transform root, string transformName)
        {
            var queue = new Queue<Transform>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var child = queue.Dequeue();
                if (child.name == transformName)
                {
                    return child;
                }

                foreach (Transform t in child)
                {
                    queue.Enqueue(t);
                }
            }

            return null;
        }

        /// <summary>
        /// Finds children by name, depth first
        /// </summary>
        /// <param name="root"></param>
        /// <param name="transformName"></param>
        /// <returns></returns>
        public static Transform FindDeepChildDepthFirst(this Transform root, string transformName)
        {
            foreach (Transform child in root)
            {
                if (child.name == transformName)
                {
                    return child;
                }

                var result = child.FindDeepChildDepthFirst(transformName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Changes the layer of a transform and all its children to the new one
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="layerName"></param>
        public static void ChangeLayersRecursively(this Transform transform, string layerName)
        {
            transform.gameObject.layer = LayerMask.NameToLayer(layerName);
            foreach (Transform child in transform)
            {
                child.ChangeLayersRecursively(layerName);
            }
        }

        /// <summary>
        /// Changes the layer of a transform and all its children to the new one
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="layerIndex"></param>
        public static void ChangeLayersRecursively(this Transform transform, int layerIndex)
        {
            transform.gameObject.layer = layerIndex;
            foreach (Transform child in transform)
            {
                child.ChangeLayersRecursively(layerIndex);
            }
        }
    }
}