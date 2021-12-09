namespace Snorlax.Common
{
    using UnityEngine;

    public static partial class Util
    {
        /// <summary> Copies the RectTransform settings </summary>
        /// <param name="target"> Target RectTransform </param>
        /// <param name="from"> Source RectTransform </param>
        public static void Copy(this RectTransform target, RectTransform from)
        {
            target.localScale = from.localScale;
            target.anchorMin = from.anchorMin;
            target.anchorMax = from.anchorMax;
            target.pivot = from.pivot;
            target.sizeDelta = from.sizeDelta;
            target.anchoredPosition3D = from.anchoredPosition3D;
        }

        /// <summary> Makes the RectTransform match its parent size </summary>
        /// <param name="target"> Target RectTransform </param>
        /// <param name="resetScaleToOne"> Reset LocalScale to Vector3.one </param>
        public static void FullScreen(this RectTransform target, bool resetScaleToOne = true)
        {
            if (resetScaleToOne) target.LocalScaleToOne();
            target.AnchorMinToZero();
            target.AnchorMaxToOne();
            target.CenterPivot();
            target.SizeDeltaToZero();
            target.AnchoredPosition3DToZero();
            target.LocalPositionToZero();
        }

        /// <summary> Moves the RectTransform pivot settings to its center </summary>
        /// <param name="target"> Target RectTransform </param>
        /// <param name="resetScaleToOne"> Reset LocalScale to Vector3.one </param>
        public static void Center(this RectTransform target, bool resetScaleToOne)
        {
            if (resetScaleToOne) target.LocalScaleToOne();
            target.AnchorMinToCenter();
            target.AnchorMaxToCenter();
            target.CenterPivot();
            target.SizeDeltaToZero();
        }

        /// <summary> Resets the target's anchoredPosition3D to Vector3.zero </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void AnchoredPosition3DToZero(this RectTransform target) { target.anchoredPosition3D = Vector3.zero; }

        /// <summary> Resets the target's localPosition to Vector3.zero </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void LocalPositionToZero(this RectTransform target) { target.localPosition = Vector3.zero; }

        /// <summary> Resets the target's localScale to Vector3.one </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void LocalScaleToOne(this RectTransform target) { target.localScale = Vector3.one; }

        /// <summary> Resets the target's anchorMin to Vector2.zero </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void AnchorMinToZero(this RectTransform target) { target.anchorMin = Vector2.zero; }

        /// <summary> Sets the target's anchorMin to Vector2(0.5f, 0.5f) </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void AnchorMinToCenter(this RectTransform target) { target.anchorMin = new Vector2(0.5f, 0.5f); }

        /// <summary> Resets the target's anchorMax to Vector2.one </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void AnchorMaxToOne(this RectTransform target) { target.anchorMax = Vector2.one; }

        /// <summary> Sets the target's anchorMax to Vector2(0.5f, 0.5f) </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void AnchorMaxToCenter(this RectTransform target) { target.anchorMax = new Vector2(0.5f, 0.5f); }

        /// <summary> Sets the target's pivot to Vector2(0.5f, 0.5f) </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void CenterPivot(this RectTransform target) { target.pivot = new Vector2(0.5f, 0.5f); }

        /// <summary> Sets the target's pivot to Vector2(0f, 0.5f) </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void LeftPivot(this RectTransform target) { target.pivot = new Vector2(0f, 0.5f); }

        /// <summary> Sets the target's pivot to Vector2(1f, 0.5f) </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void RightPivot(this RectTransform target) { target.pivot = new Vector2(1f, 0.5f); }

        /// <summary> Sets the target's pivot to Vector2(0.5f, 0f) </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void TopPivot(this RectTransform target) { target.pivot = new Vector2(0.5f, 0f); }

        /// <summary> Sets the target's pivot to Vector2(0.5f, 1f) </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void BottomPivot(this RectTransform target) { target.pivot = new Vector2(0.5f, 1f); }

        /// <summary> Resets the target's sizeDelta to Vector2.zero </summary>
        /// <param name="target"> Target RectTransform </param>
        public static void SizeDeltaToZero(this RectTransform target) { target.sizeDelta = Vector2.zero; }

        /// <summary>
        /// Sets the left offset of a rect transform to the specified value
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="left"></param>
        public static void SetLeft(this RectTransform rectTransform, float left) { rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y); }

        /// <summary>
        /// Sets the right offset of a rect transform to the specified value
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="right"></param>
        public static void SetRight(this RectTransform rectTransform, float right) { rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y); }

        /// <summary>
        /// Sets the top offset of a rect transform to the specified value
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="top"></param>
        public static void SetTop(this RectTransform rectTransform, float top) { rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top); }

        /// <summary>
        /// Sets the bottom offset of a rect transform to the specified value
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="bottom"></param>
        public static void SetBottom(this RectTransform rectTransform, float bottom) { rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom); }

        /// <summary>
        /// get Corner from rectTransform
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static Vector3[] GetCorners(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return corners;
        }

        /// <summary>
        /// get Corner MaxY from rectTransform
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static float MaxCornerY(this RectTransform rectTransform) { return rectTransform.GetCorners()[1].y; }

        /// <summary>
        /// get Corner MinY from rectTransform
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static float MinCornerY(this RectTransform rectTransform) { return rectTransform.GetCorners()[0].y; }

        /// <summary>
        /// get Corner MaxX from rectTransform
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static float MaxCornerX(this RectTransform rectTransform) { return rectTransform.GetCorners()[2].x; }

        /// <summary>
        /// get Corner MinX from rectTransform
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static float MinCornerX(this RectTransform rectTransform) { return rectTransform.GetCorners()[0].x; }

        /// <summary>
        /// Returns true if this rectangle intersects the other specified rectangle
        /// </summary>
        /// <param name="thisRectangle"></param>
        /// <param name="otherRectangle"></param>
        /// <returns></returns>
        public static bool Intersects(this Rect thisRectangle, Rect otherRectangle)
        {
            return !(thisRectangle.x > otherRectangle.xMax || thisRectangle.xMax < otherRectangle.x || thisRectangle.y > otherRectangle.yMax ||
                     thisRectangle.yMax < otherRectangle.y);
        }

        /// <summary>
        /// Returns true if the rectangle of RectTransform a overlaps rectangle of RectTransform b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Overlaps(this RectTransform a, RectTransform b) { return a.WorldRect().Overlaps(b.WorldRect()); }

        /// <summary>
        /// Returns true if the rectangle of RectTransform a overlaps rectangle of RectTransform b
        /// If allowInverse is present and true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="allowInverse"></param>
        /// <returns></returns>
        public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse) { return a.WorldRect().Overlaps(b.WorldRect(), allowInverse); }

        /// <summary>
        /// Return world rect of RectTransform
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static Rect WorldRect(this RectTransform rectTransform)
        {
            var sizeDelta = rectTransform.sizeDelta;
            var rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
            // ReSharper disable once Unity.InefficientPropertyAccess
            var rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

            var position = rectTransform.position;
            return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
        }
    }
}