﻿using System.Runtime.CompilerServices;
using UnityEngine;
using static Pancake.Common.M;

namespace Pancake.Common
{
    public interface IParamCubicSplineSegment1D
    {
        /// <summary>The curve generated by the control points</summary>
        Polynomial Curve { get; }
    }

    public interface IParamCubicSplineSegment2D
    {
        /// <inheritdoc cref="IParamCubicSplineSegment1D.Curve"/>
        Polynomial2D Curve { get; }
    }

    public interface IParamCubicSplineSegment3D
    {
        /// <inheritdoc cref="IParamCubicSplineSegment2D.Curve"/>
        Polynomial3D Curve { get; }
    }

    /// <summary>An interface representing a parametric curve</summary>
    /// <typeparam name="V">The vector type of the curve</typeparam>
    public interface IParamCurve<out V> where V : struct
    {
        /// <summary>Returns the degree of this curve. Quadratic = 2, Cubic = 3, etc</summary>
        int Degree { get; }

        /// <summary>Returns the point at the given t-value on the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        V Eval(float t);
    }

    /// <summary>An interface representing a parametric curve of degree 1 or higher</summary>
    /// <typeparam name="V">The vector type of the curve</typeparam>
    public interface IParamCurve1Diff<V> : IParamCurve<V> where V : struct
    {
        /// <summary>Returns the derivative at the given t-value on the curve. Loosely analogous to "velocity" of the point along the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        V EvalDerivative(float t);
    }

    /// <summary>An interface representing a parametric curve of degree 2 or higher</summary>
    /// <typeparam name="V">The vector type of the curve</typeparam>
    public interface IParamCurve2Diff<V> : IParamCurve1Diff<V> where V : struct
    {
        /// <summary>Returns the second derivative at the given t-value on the curve. Loosely analogous to "acceleration" of the point along the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        V EvalSecondDerivative(float t);
    }

    /// <summary>An interface representing a parametric curve of degree 3 or higher</summary>
    /// <typeparam name="V">The vector type of the curve</typeparam>
    public interface IParamCurve3Diff<V> : IParamCurve2Diff<V> where V : struct
    {
        /// <summary>Returns the third derivative of the curve. Loosely analogous to "jerk/jolt" (rate of change of acceleration) of the point along the curve</summary>
        V EvalThirdDerivative(float t);
    }

    /// <summary>Shared functionality for all 2D parametric curves</summary>
    public static class IParamCurveExt2D
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>Returns the approximate length of the curve in the 0 to 1 interval</summary>
        /// <param name="accuracy">The number of subdivisions to approximate the length with. Higher values are more accurate, but more expensive to calculate</param>
        [MethodImpl(INLINE)]
        public static float GetArcLength<T>(this T curve, int accuracy = 8) where T : IParamCurve<Vector2> => curve.GetArcLength(FloatRange.unit, accuracy);

        /// <summary>Returns the approximate length of the curve in the given interval</summary>
        /// <param name="interval">The parameter interval of the curve to get the length of</param>
        /// <param name="accuracy">The number of subdivisions to approximate the length with. Higher values are more accurate, but more expensive to calculate</param>
        public static float GetArcLength<T>(this T curve, FloatRange interval, int accuracy = 8) where T : IParamCurve<Vector2>
        {
            accuracy = accuracy.AtLeast(2);
            bool unit = interval == FloatRange.unit;
            float totalDist = 0;
            Vector2 prev = curve.Eval(interval.a);
            for (int i = 1; i < accuracy; i++)
            {
                float t = i / (accuracy - 1f);
                Vector2 p = curve.Eval(unit ? t : interval.Lerp(t));
                float dx = p.x - prev.x;
                float dy = p.y - prev.y;
                totalDist += Mathf.Sqrt(dx * dx + dy * dy);
                prev = p;
            }

            return totalDist;
        }
    }

    /// <summary>Shared functionality for all 3D parametric curves</summary>
    public static class IParamCurveExt3D
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <inheritdoc cref="IParamCurveExt2D.GetArcLength{T}"/>
        [MethodImpl(INLINE)]
        public static float GetArcLength<T>(this T curve, int accuracy = 8) where T : IParamCurve<Vector3> => curve.GetArcLength(FloatRange.unit, accuracy);

        /// <inheritdoc cref="IParamCurveExt2D.GetArcLength{T}(T,Freya.FloatRange,int)"/>
        public static float GetArcLength<T>(this T curve, FloatRange interval, int accuracy = 8) where T : IParamCurve<Vector3>
        {
            accuracy = accuracy.AtLeast(2);
            bool unit = interval == FloatRange.unit;
            float totalDist = 0;
            Vector3 prev = curve.Eval(interval.a);
            for (int i = 1; i < accuracy; i++)
            {
                float t = i / (accuracy - 1f);
                Vector3 p = curve.Eval(unit ? t : interval.Lerp(t));
                float dx = p.x - prev.x;
                float dy = p.y - prev.y;
                float dz = p.z - prev.z;
                totalDist += Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
                prev = p;
            }

            return totalDist;
        }
    }

    /// <summary>Shared functionality for 2D parametric curves of degree 1 or higher</summary>
    public static class IParamCurve1DiffExt2D
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>Returns the normalized tangent direction at the given t-value on the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Vector2 EvalTangent<T>(this T curve, float t) where T : IParamCurve1Diff<Vector2> => curve.EvalDerivative(t).normalized;

        /// <summary>Returns the normal direction at the given t-value on the curve.
        /// This normal will point to the inner arc of the current curvature</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Vector2 EvalNormal<T>(this T curve, float t) where T : IParamCurve1Diff<Vector2> => curve.EvalTangent(t).Rotate90CCW();

        /// <summary>Returns the 2D angle of the direction of the curve at the given point, in radians</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static float EvalAngle<T>(this T curve, float t) where T : IParamCurve1Diff<Vector2> => DirToAng(curve.EvalDerivative(t));

        /// <summary>Returns the orientation at the given point t, where the X axis is tangent to the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Quaternion EvalOrientation<T>(this T curve, float t) where T : IParamCurve1Diff<Vector2> => DirToOrientation(curve.EvalDerivative(t));

        /// <summary>Returns the position and orientation at the given t-value on the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Pose EvalPose<T>(this T curve, float t) where T : IParamCurve1Diff<Vector2> => PointDirToPose(curve.Eval(t), curve.EvalTangent(t));

        /// <summary>Returns the position and orientation at the given t-value on the curve, expressed as a matrix</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Matrix4x4 EvalMatrix<T>(this T curve, float t) where T : IParamCurve1Diff<Vector2> => GetMatrixFrom2DPointDir(curve.Eval(t), curve.EvalTangent(t));
    }

    /// <summary>Shared functionality for 3D parametric curves of degree 1 or higher</summary>
    public static class IParamCurve1DiffExt3D
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <inheritdoc cref="IParamCurve1DiffExt2D.EvalTangent{T}(T,float)"/>
        [MethodImpl(INLINE)]
        public static Vector3 EvalTangent<T>(this T curve, float t) where T : IParamCurve1Diff<Vector3> => curve.EvalDerivative(t).normalized;

        /// <summary>Returns a normal of the curve given a reference up vector and t-value on the curve.
        /// The normal will be perpendicular to both the supplied up vector and the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        /// <param name="up">The reference up vector. The normal will be perpendicular to both the supplied up vector and the curve</param>
        [MethodImpl(INLINE)]
        public static Vector3 EvalNormal<T>(this T curve, float t, Vector3 up) where T : IParamCurve1Diff<Vector3> =>
            GetNormalFromLookTangent(curve.EvalDerivative(t), up);

        /// <summary>Returns the binormal of the curve given a reference up vector and t-value on the curve.
        /// The binormal will attempt to be as aligned with the reference vector as possible,
        /// while still being perpendicular to the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        /// <param name="up">The reference up vector. The binormal will attempt to be as aligned with the reference vector as possible, while still being perpendicular to the curve</param>
        [MethodImpl(INLINE)]
        public static Vector3 EvalBinormal<T>(this T curve, float t, Vector3 up) where T : IParamCurve1Diff<Vector3> =>
            GetBinormalFromLookTangent(curve.EvalDerivative(t), up);

        /// <summary>Returns the orientation at the given point t, where the Z direction is tangent to the curve.
        /// The Y axis will attempt to align with the supplied up vector</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        /// <param name="up">The reference up vector. The Y axis will attempt to be as aligned with this vector as much as possible</param>
        [MethodImpl(INLINE)]
        public static Quaternion EvalOrientation<T>(this T curve, float t, Vector3 up) where T : IParamCurve1Diff<Vector3> =>
            Quaternion.LookRotation(curve.EvalDerivative(t), up);

        /// <summary>Returns the position and orientation of curve at the given point t, where the Z direction is tangent to the curve.
        /// The Y axis will attempt to align with the supplied up vector</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        /// <param name="up">The reference up vector. The Y axis will attempt to be as aligned with this vector as much as possible</param>
        [MethodImpl(INLINE)]
        public static Pose EvalPose<T>(this T curve, float t, Vector3 up) where T : IParamCurve1Diff<Vector3> =>
            new Pose(curve.Eval(t), Quaternion.LookRotation(curve.EvalDerivative(t), up));

        /// <summary>Returns the position and orientation of curve at the given point t, expressed as a matrix, where the Z direction is tangent to the curve.
        /// The Y axis will attempt to align with the supplied up vector</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        /// <param name="up">The reference up vector. The Y axis will attempt to be as aligned with this vector as much as possible</param>
        public static Matrix4x4 EvalMatrix<T>(this T curve, float t, Vector3 up) where T : IParamCurve1Diff<Vector3>
        {
            (Vector3 Pt, Vector3 Tn) = (curve.Eval(t), curve.EvalTangent(t));
            Vector3 Nm = Vector3.Cross(up, Tn).normalized; // X axis
            Vector3 Bn = Vector3.Cross(Tn, Nm); // Y axis
            return new Matrix4x4(new Vector4(Nm.x, Nm.y, Nm.z, 0), new Vector4(Bn.x, Bn.y, Bn.z, 0), new Vector4(Tn.x, Tn.y, Tn.z, 0), new Vector4(Pt.x, Pt.y, Pt.z, 1));
        }
    }

    /// <summary>Shared functionality for 2D parametric curves of degree 2 or higher</summary>
    public static class IParamCurve2DiffExt2D
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>Returns the signed curvature at the given t-value on the curve, in radians per distance unit (equivalent to the reciprocal radius of the osculating circle)</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static float EvalCurvature<T>(this T curve, float t) where T : IParamCurve2Diff<Vector2> =>
            M.GetCurvature(curve.EvalDerivative(t), curve.EvalSecondDerivative(t));

        /// <summary>Returns the osculating circle at the given t-value in the curve, if possible. Osculating circles are defined everywhere except on inflection points, where curvature is 0</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Circle2D EvalOsculatingCircle<T>(this T curve, float t) where T : IParamCurve2Diff<Vector2> =>
            Circle2D.GetOsculatingCircle(curve.Eval(t), curve.EvalDerivative(t), curve.EvalSecondDerivative(t));
    }

    /// <summary>Shared functionality for 3D parametric curves of degree 2 or higher</summary>
    public static class IParamCurve2DiffExt3D
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>Returns a pseudovector at the given t-value on the curve, where the magnitude is the curvature in radians per distance unit, and the direction is the axis of curvature</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Vector3 EvalCurvature<T>(this T curve, float t) where T : IParamCurve2Diff<Vector3> =>
            M.GetCurvature(curve.EvalDerivative(t), curve.EvalSecondDerivative(t));

        /// <inheritdoc cref="IParamCurve2DiffExt2D.EvalOsculatingCircle{T}"/>
        [MethodImpl(INLINE)]
        public static Circle3D EvalOsculatingCircle<T>(this T curve, float t) where T : IParamCurve2Diff<Vector3> =>
            Circle3D.GetOsculatingCircle(curve.Eval(t), curve.EvalDerivative(t), curve.EvalSecondDerivative(t));

        /// <summary>Returns the frenet-serret (curvature-based) normal direction at the given t-value on the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Vector3 EvalArcNormal<T>(this T curve, float t) where T : IParamCurve2Diff<Vector3> =>
            M.GetArcNormal(curve.EvalDerivative(t), curve.EvalSecondDerivative(t));

        /// <summary>Returns the frenet-serret (curvature-based) binormal direction at the given t-value on the curve</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Vector3 EvalArcBinormal<T>(this T curve, float t) where T : IParamCurve2Diff<Vector3> =>
            M.GetArcBinormal(curve.EvalDerivative(t), curve.EvalSecondDerivative(t));

        /// <summary>Returns the frenet-serret (curvature-based) orientation of curve at the given point t, where the Z direction is tangent to the curve.
        /// The X axis will point to the inner arc of the current curvature</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Quaternion EvalArcOrientation<T>(this T curve, float t) where T : IParamCurve2Diff<Vector3> =>
            M.GetArcOrientation(curve.EvalDerivative(t), curve.EvalSecondDerivative(t));

        /// <summary>Returns the position and the frenet-serret (curvature-based) orientation of curve at the given point t, where the Z direction is tangent to the curve.
        /// The X axis will point to the inner arc of the current curvature</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static Pose EvalArcPose<T>(this T curve, float t) where T : IParamCurve2Diff<Vector3>
        {
            (Vector3 pt, Vector3 vel, Vector3 acc) = (curve.Eval(t), curve.EvalDerivative(t), curve.EvalSecondDerivative(t));
            Vector3 binormal = Vector3.Cross(vel, acc);
            return new Pose(pt, Quaternion.LookRotation(vel, binormal));
        }

        /// <summary>Returns the position and the frenet-serret (curvature-based) orientation of curve at the given point t, expressed as a matrix, where the Z direction is tangent to the curve.
        /// The X axis will point to the inner arc of the current curvature</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        public static Matrix4x4 EvalArcMatrix<T>(this T curve, float t) where T : IParamCurve2Diff<Vector3>
        {
            Vector3 P = curve.Eval(t);
            Vector3 vel = curve.EvalDerivative(t);
            Vector3 acc = curve.EvalSecondDerivative(t);
            Vector3 Tn = vel.normalized;
            Vector3 B = Vector3.Cross(vel, acc).normalized;
            Vector3 N = Vector3.Cross(B, Tn);
            return new Matrix4x4(new Vector4(N.x, N.y, N.z, 0), new Vector4(B.x, B.y, B.z, 0), new Vector4(Tn.x, Tn.y, Tn.z, 0), new Vector4(P.x, P.y, P.z, 1));
        }
    }

    /// <summary>Shared functionality for 3D parametric curves of degree 3 or higher</summary>
    public static class IParamCurve3DiffExt3D
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>Returns the torsion at the given t-value on the curve, in radians per distance unit</summary>
        /// <param name="t">The t-value along the curve to sample</param>
        [MethodImpl(INLINE)]
        public static float EvalTorsion<T>(this T curve, float t) where T : IParamCurve3Diff<Vector3> =>
            M.GetTorsion(curve.EvalDerivative(t), curve.EvalSecondDerivative(t), curve.EvalThirdDerivative(t));
    }
}