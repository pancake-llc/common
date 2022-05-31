﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Pancake.Common
{
    /// <summary>An optimized 2D uniform B-spline segment</summary>
    [Serializable]
    public struct UBSCubic2D : IParamCubicSplineSegment2D
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>Creates a uniform cubic B-spline segment, given 4 control points</summary>
        /// <param name="p0">The first point of the B-spline hull</param>
        /// <param name="p1">The second point of the B-spline hull</param>
        /// <param name="p2">The third point of the B-spline hull</param>
        /// <param name="p3">The fourth point of the B-spline hull</param>
        public UBSCubic2D(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            (this.p0, this.p1, this.p2, this.p3) = (p0, p1, p2, p3);
            validCoefficients = false;
            curve = default;
        }

        Polynomial2D curve;

        public Polynomial2D Curve
        {
            get
            {
                ReadyCoefficients();
                return curve;
            }
        }

        #region Control Points

        [SerializeField] Vector2 p0, p1, p2, p3; // the points of the B-spline hull

        /// <summary>The first point of the B-spline hull</summary>
        public Vector2 P0
        {
            [MethodImpl(INLINE)] get => p0;
            [MethodImpl(INLINE)] set => _ = (p0 = value, validCoefficients = false);
        }

        /// <summary>The second point of the B-spline hull</summary>
        public Vector2 P1
        {
            [MethodImpl(INLINE)] get => p1;
            [MethodImpl(INLINE)] set => _ = (p1 = value, validCoefficients = false);
        }

        /// <summary>The third point of the B-spline hull</summary>
        public Vector2 P2
        {
            [MethodImpl(INLINE)] get => p2;
            [MethodImpl(INLINE)] set => _ = (p2 = value, validCoefficients = false);
        }

        /// <summary>The fourth point of the B-spline hull</summary>
        public Vector2 P3
        {
            [MethodImpl(INLINE)] get => p3;
            [MethodImpl(INLINE)] set => _ = (p3 = value, validCoefficients = false);
        }

        /// <summary>Get or set a control point position by index. Valid indices: 0, 1, 2 or 3</summary>
        public Vector2 this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return P0;
                    case 1: return P1;
                    case 2: return P2;
                    case 3: return P3;
                    default: throw new ArgumentOutOfRangeException(nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know");
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        P0 = value;
                        break;
                    case 1:
                        P1 = value;
                        break;
                    case 2:
                        P2 = value;
                        break;
                    case 3:
                        P3 = value;
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know");
                }
            }
        }

        #endregion

        #region Coefficients

        [NonSerialized] bool validCoefficients; // inverted isDirty flag (can't default to true in structs)

        // Coefficient Calculation
        [MethodImpl(INLINE)]
        void ReadyCoefficients()
        {
            if (validCoefficients)
                return; // no need to update
            validCoefficients = true;
            curve = CharMatrix.cubicUniformBspline.GetCurve(p0, p1, p2, p3);
        }

        #endregion

        /// <summary>Returns the exact cubic bézier representation of this segment</summary>
        public BezierCubic2D ToBezier()
        {
            const float _13 = 1f / 3f;
            const float _23 = 2f / 3f;
            float ax = p0.x + _23 * (p1.x - p0.x);
            float bx = p1.x + _13 * (p2.x - p1.x);
            float cx = p1.x + _23 * (p2.x - p1.x);
            float dx = p2.x + _13 * (p3.x - p2.x);
            float ay = p0.y + _23 * (p1.y - p0.y);
            float by = p1.y + _13 * (p2.y - p1.y);
            float cy = p1.y + _23 * (p2.y - p1.y);
            float dy = p2.y + _13 * (p3.y - p2.y);
            return new BezierCubic2D(new Vector2(0.5f * (ax + bx), 0.5f * (ay + by)),
                new Vector2(bx, by),
                new Vector2(cx, cy),
                new Vector2(0.5f * (cx + dx), 0.5f * (cy + dy)));
        }
    }
}