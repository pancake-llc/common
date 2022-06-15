// Do not manually edit - this file is generated by MCodegen.cs

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Pancake.Common {

	/// <summary>An optimized uniform 1D Cubic b-spline segment, with 4 control points</summary>
	[Serializable] public struct UBSCubic1D : IParamSplineSegment<Polynomial,Matrix4x1> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		[SerializeField] Matrix4x1 pointMatrix;
		[NonSerialized] Polynomial curve;
		[NonSerialized] bool validCoefficients;

		/// <summary>Creates a uniform 1D Cubic b-spline segment, from 4 control points</summary>
		/// <param name="p0">The first point of the B-spline hull</param>
		/// <param name="p1">The second point of the B-spline hull</param>
		/// <param name="p2">The third point of the B-spline hull</param>
		/// <param name="p3">The fourth point of the B-spline hull</param>
		public UBSCubic1D( float p0, float p1, float p2, float p3 ) => (pointMatrix,curve,validCoefficients) = (new Matrix4x1(p0, p1, p2, p3),default,false);

		public Polynomial Curve {
			get {
				if( validCoefficients )
					return curve; // no need to update
				validCoefficients = true;
				return curve = new Polynomial(
					(1/6f)*P0+(2/3f)*P1+(1/6f)*P2,
					(-P0+P2)/2,
					(1/2f)*P0-P1+(1/2f)*P2,
					-(1/6f)*P0+(1/2f)*P1-(1/2f)*P2+(1/6f)*P3
				);
			}
		}
		public Matrix4x1 PointMatrix {[MethodImpl( INLINE )] get => pointMatrix; [MethodImpl( INLINE )] set => _ = ( pointMatrix = value, validCoefficients = false ); }
		/// <summary>The first point of the B-spline hull</summary>
		public float P0{ [MethodImpl( INLINE )] get => pointMatrix.m0; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m0 = value, validCoefficients = false ); }
		/// <summary>The second point of the B-spline hull</summary>
		public float P1{ [MethodImpl( INLINE )] get => pointMatrix.m1; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m1 = value, validCoefficients = false ); }
		/// <summary>The third point of the B-spline hull</summary>
		public float P2{ [MethodImpl( INLINE )] get => pointMatrix.m2; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m2 = value, validCoefficients = false ); }
		/// <summary>The fourth point of the B-spline hull</summary>
		public float P3{ [MethodImpl( INLINE )] get => pointMatrix.m3; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m3 = value, validCoefficients = false ); }
		/// <summary>Get or set a control point position by index. Valid indices from 0 to 3</summary>
		public float this[ int i ] {
			get => i switch { 0 => P0, 1 => P1, 2 => P2, 3 => P3, _ => throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" ) };
			set { switch( i ){ case 0: P0 = value; break; case 1: P1 = value; break; case 2: P2 = value; break; case 3: P3 = value; break; default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" ); }}
		}
		public static bool operator ==( UBSCubic1D a, UBSCubic1D b ) => a.pointMatrix == b.pointMatrix;
		public static bool operator !=( UBSCubic1D a, UBSCubic1D b ) => !( a == b );
		public bool Equals( UBSCubic1D other ) => P0.Equals( other.P0 ) && P1.Equals( other.P1 ) && P2.Equals( other.P2 ) && P3.Equals( other.P3 );
		public override bool Equals( object obj ) => obj is UBSCubic1D other && pointMatrix.Equals( other.pointMatrix );
		public override int GetHashCode() => pointMatrix.GetHashCode();
		public override string ToString() => $"({pointMatrix.m0}, {pointMatrix.m1}, {pointMatrix.m2}, {pointMatrix.m3})";

		public static explicit operator BezierCubic1D( UBSCubic1D s ) =>
			new BezierCubic1D(
				(1/6f)*s.P0+(2/3f)*s.P1+(1/6f)*s.P2,
				(2/3f)*s.P1+(1/3f)*s.P2,
				(1/3f)*s.P1+(2/3f)*s.P2,
				(1/6f)*s.P1+(2/3f)*s.P2+(1/6f)*s.P3
			);
		public static explicit operator HermiteCubic1D( UBSCubic1D s ) =>
			new HermiteCubic1D(
				(1/6f)*s.P0+(2/3f)*s.P1+(1/6f)*s.P2,
				(-s.P0+s.P2)/2,
				(1/6f)*s.P1+(2/3f)*s.P2+(1/6f)*s.P3,
				(-s.P1+s.P3)/2
			);
		public static explicit operator CatRomCubic1D( UBSCubic1D s ) =>
			new CatRomCubic1D(
				s.P0+(1/6f)*s.P1-(1/3f)*s.P2+(1/6f)*s.P3,
				(1/6f)*s.P0+(2/3f)*s.P1+(1/6f)*s.P2,
				(1/6f)*s.P1+(2/3f)*s.P2+(1/6f)*s.P3,
				(1/6f)*s.P0-(1/3f)*s.P1+(1/6f)*s.P2+s.P3
			);
		/// <summary>Returns a linear blend between two b-spline curves</summary>
		/// <param name="a">The first spline segment</param>
		/// <param name="b">The second spline segment</param>
		/// <param name="t">A value from 0 to 1 to blend between <c>a</c> and <c>b</c></param>
		public static UBSCubic1D Lerp( UBSCubic1D a, UBSCubic1D b, float t ) =>
			new(
				M.Lerp( a.P0, b.P0, t ),
				M.Lerp( a.P1, b.P1, t ),
				M.Lerp( a.P2, b.P2, t ),
				M.Lerp( a.P3, b.P3, t )
			);
	}
}
