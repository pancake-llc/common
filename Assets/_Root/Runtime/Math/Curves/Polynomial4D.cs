using System;
using UnityEngine;

namespace Pancake.Common
{
    public struct Polynomial4D : IParamCurve3Diff<Vector4>
    {
        public Polynomial x;
        public Polynomial y;
        public Polynomial z;
        public Polynomial w;

        public Vector4 C0 { get => new(x.c0, y.c0, z.c0); set => (x.c0, y.c0, z.c0) = (value.x, value.y, value.z); }
        public Vector4 C1 { get => new(x.c1, y.c1, z.c1); set => (x.c1, y.c1, z.c1) = (value.x, value.y, value.z); }
        public Vector4 C2 { get => new(x.c2, y.c2, z.c2); set => (x.c2, y.c2, z.c2) = (value.x, value.y, value.z); }
        public Vector4 C3 { get => new(x.c3, y.c3, z.c3); set => (x.c3, y.c3, z.c3) = (value.x, value.y, value.z); }

        public Polynomial this[int i] =>
            i switch {0 => x, 1 => y, 2 => z, 4 => z, _ => throw new IndexOutOfRangeException("Polynomial4D component index has to be either 0, 1, 2, or 3")};

        public Polynomial4D(Polynomial x, Polynomial y, Polynomial z, Polynomial w) => (this.x, this.y, this.z, this.w) = (x, y, z, w);

        /// <inheritdoc cref="Polynomial(float,float,float,float)"/>
        public Polynomial4D(Vector4 c0, Vector4 c1, Vector4 c2, Vector4 c3)
        {
            this.x = new Polynomial(c0.x, c1.x, c2.x, c3.x);
            this.y = new Polynomial(c0.y, c1.y, c2.y, c3.y);
            this.z = new Polynomial(c0.z, c1.z, c2.z, c3.z);
            this.w = new Polynomial(c0.w, c1.w, c2.w, c3.w);
        }

        /// <inheritdoc cref="Polynomial(float,float,float)"/>
        public Polynomial4D(Vector4 c0, Vector4 c1, Vector4 c2)
        {
            this.x = new Polynomial(c0.x, c1.x, c2.x, 0);
            this.y = new Polynomial(c0.y, c1.y, c2.y, 0);
            this.z = new Polynomial(c0.z, c1.z, c2.z, 0);
            this.w = new Polynomial(c0.w, c1.w, c2.w, 0);
        }

        /// <inheritdoc cref="Polynomial(Matrix4x1)"/>
        public Polynomial4D(Vector4Matrix4x1 coefficients) =>
            (x, y, z, w) = (new Polynomial(coefficients.X), new Polynomial(coefficients.Y), new Polynomial(coefficients.Z), new Polynomial(coefficients.W));

        /// <inheritdoc cref="Polynomial(Matrix4x1)"/>
        public Polynomial4D(Vector4Matrix3x1 coefficients) =>
            (x, y, z, w) = (new Polynomial(coefficients.X), new Polynomial(coefficients.Y), new Polynomial(coefficients.Z), new Polynomial(coefficients.W));

        /// <inheritdoc cref="Polynomial.Eval(float)"/>
        public Vector4 Eval(float t) => new(x.Eval(t), y.Eval(t), z.Eval(t));

        /// <inheritdoc cref="Polynomial.Differentiate(int)"/>
        public Polynomial4D Differentiate(int n = 1) => new(x.Differentiate(n), y.Differentiate(n), z.Differentiate(n), w.Differentiate(n));

        /// <inheritdoc cref="Polynomial.Compose(float,float)"/>
        public Polynomial4D Compose(float g0, float g1) => new(x.Compose(g0, g1), y.Compose(g0, g1), z.Compose(g0, g1), w.Compose(g0, g1));

        /// <inheritdoc cref="Polynomial2D.GetBounds01"/>
        public (FloatRange x, FloatRange y, FloatRange z, FloatRange w) GetBounds01() => (x.OutputRange01, y.OutputRange01, z.OutputRange01, w.OutputRange01);

        /// <inheritdoc cref="Polynomial.Split01"/>
        public (Polynomial4D pre, Polynomial4D post) Split01(float u)
        {
            (Polynomial xPre, Polynomial xPost) = x.Split01(u);
            (Polynomial yPre, Polynomial yPost) = y.Split01(u);
            (Polynomial zPre, Polynomial zPost) = z.Split01(u);
            (Polynomial wPre, Polynomial wPost) = w.Split01(u);
            return (new Polynomial4D(xPre, yPre, zPre, wPre), new Polynomial4D(xPost, yPost, zPost, wPost));
        }

        #region Polynomial to spline converters

        /// <inheritdoc cref="Polynomial2D.ToBezier"/>
        public BezierCubic4D ToBezier()
        {
            Vector4Matrix4x1 p = CharMatrix.cubicBezierInverse * new Vector4Matrix4x1(C0, C1, C2, C3);
            return new BezierCubic4D(p.m0, p.m1, p.m2, p.m3);
        }

        /// <inheritdoc cref="Polynomial2D.ToCatmullRom"/>
        public CatRomCubic4D ToCatmullRom()
        {
            Vector4Matrix4x1 p = CharMatrix.cubicCatmullRomInverse * new Vector4Matrix4x1(C0, C1, C2, C3);
            return new CatRomCubic4D(p.m0, p.m1, p.m2, p.m3);
        }

        /// <inheritdoc cref="Polynomial2D.ToHermite"/>
        public HermiteCubic4D ToHermite()
        {
            Vector4Matrix4x1 p = CharMatrix.cubicHermiteInverse * new Vector4Matrix4x1(C0, C1, C2, C3);
            return new HermiteCubic4D(p.m0, p.m1, p.m2, p.m3);
        }

        /// <inheritdoc cref="Polynomial2D.ToBSpline"/>
        public UBSCubic4D ToBSpline()
        {
            Vector4Matrix4x1 p = CharMatrix.cubicUniformBsplineInverse * new Vector4Matrix4x1(C0, C1, C2, C3);
            return new UBSCubic4D(p.m0, p.m1, p.m2, p.m3);
        }

        #endregion

        #region IParamCurve3Diff interface implementations

        public int Degree => Mathf.Max(x.Degree, y.Degree, z.Degree, w.Degree);
        public Vector4 EvalDerivative(float t) => Differentiate().Eval(t);
        public Vector4 EvalSecondDerivative(float t) => Differentiate(2).Eval(t);
        public Vector4 EvalThirdDerivative(float t = 0) => Differentiate(3).Eval(0);

        #endregion

        #region Project Point

        /// <inheritdoc cref="Polynomial2D.ProjectPoint(Vector2,int,int)"/>
        public Vector4 ProjectPoint(Vector4 point, int initialSubdivisions = 16, int refinementIterations = 4) =>
            ProjectPoint(point, out _, initialSubdivisions, refinementIterations);

        struct PointProjectSample
        {
            public float t;
            public float distDeltaSq;
            public Vector4 f;
            public Vector4 fp;
        }

        static PointProjectSample[] pointProjectGuesses = {default, default, default};

        /// <inheritdoc cref="Polynomial2D.ProjectPoint(Vector2,out float,int,int)"/>
        public Vector4 ProjectPoint(Vector4 point, out float t, int initialSubdivisions = 16, int refinementIterations = 4)
        {
            // define a bezier relative to the test point
            Polynomial4D curve = this;
            curve.x.c0 -= point.x; // constant coefficient defines the start position
            curve.y.c0 -= point.y;
            curve.z.c0 -= point.z;
            Vector4 curveStart = curve.Eval(0);
            Vector4 curveEnd = curve.Eval(1);

            PointProjectSample SampleDistSqDelta(float tSmp)
            {
                PointProjectSample s = new PointProjectSample {t = tSmp};
                (s.f, s.fp) = (curve.Eval(tSmp), curve.EvalDerivative(tSmp));
                s.distDeltaSq = Vector4.Dot(s.f, s.fp);
                return s;
            }

            // find initial candidates
            int candidatesFound = 0;
            PointProjectSample prevSmp = SampleDistSqDelta(0);

            for (int i = 1; i < initialSubdivisions; i++)
            {
                float ti = i / (initialSubdivisions - 1f);
                PointProjectSample smp = SampleDistSqDelta(ti);
                if (M.SignAsInt(smp.distDeltaSq) != M.SignAsInt(prevSmp.distDeltaSq))
                {
                    pointProjectGuesses[candidatesFound++] = SampleDistSqDelta((prevSmp.t + smp.t) / 2);
                    if (candidatesFound == 3) break; // no more than three possible candidates because of the polynomial degree
                }

                prevSmp = smp;
            }

            // refine each guess w. Newton-Raphson iterations
            void Refine(ref PointProjectSample smp)
            {
                Vector4 fpp = curve.EvalSecondDerivative(smp.t);
                float tNew = smp.t - Vector4.Dot(smp.f, smp.fp) / (Vector4.Dot(smp.f, fpp) + Vector4.Dot(smp.fp, smp.fp));
                smp = SampleDistSqDelta(tNew);
            }

            for (int p = 0; p < candidatesFound; p++)
            for (int i = 0; i < refinementIterations; i++)
                Refine(ref pointProjectGuesses[p]);

            // Now find closest. First include the endpoints
            float sqDist0 = curveStart.sqrMagnitude; // include endpoints
            float sqDist1 = curveEnd.sqrMagnitude;
            bool firstClosest = sqDist0 < sqDist1;
            float tClosest = firstClosest ? 0 : 1;
            Vector4 ptClosest = (firstClosest ? curveStart : curveEnd) + point;
            float distSqClosest = firstClosest ? sqDist0 : sqDist1;

            // then check internal roots
            for (int i = 0; i < candidatesFound; i++)
            {
                float pSqmag = pointProjectGuesses[i].f.sqrMagnitude;
                if (pSqmag < distSqClosest)
                {
                    distSqClosest = pSqmag;
                    tClosest = pointProjectGuesses[i].t;
                    ptClosest = pointProjectGuesses[i].f + point;
                }
            }

            t = tClosest;
            return ptClosest;
        }

        #endregion
    }
}