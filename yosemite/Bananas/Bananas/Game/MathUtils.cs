using System;
using SceneKit;

namespace Bananas
{
	public static class MathUtils
	{
		public static SCNMatrix4 TranslateWithVector (this SCNMatrix4 matrix, SCNVector3 vector)
		{
			matrix.M41 += matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M31 * vector.Z;
			matrix.M42 += matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M32 * vector.Z;
			matrix.M43 += matrix.M13 * vector.X + matrix.M23 * vector.Y + matrix.M33 * vector.Z;
			return matrix;
		}

		public static SCNVector3 GetPosition (this SCNMatrix4 matrix)
		{
			return new SCNVector3 (matrix.M42, matrix.M43, matrix.M44);
		}

		public static SCNMatrix4 SetPosition (this SCNMatrix4 matrix, SCNVector3 vector)
		{
			matrix.M41 = vector.X; 
			matrix.M42 = vector.Y;
			matrix.M43 = vector.Z;

			return matrix;
		}

		public static nfloat RandomPercent ()
		{
			var rnd = new Random ();
			return rnd.Next (0, 100) * 0.01f;
		}

		public static SCNMatrix4 Interpolate (SCNMatrix4 scnm0, SCNMatrix4 scnmf, nfloat factor)
		{
			SCNVector4 p0 = scnm0.Row3;
			SCNVector4 pf = scnmf.Row3;

			var q0 = scnm0.ToQuaternion ();
			var qf = scnmf.ToQuaternion ();

			SCNVector4 pTmp = Lerp (p0, pf, factor);
			SCNQuaternion qTmp = SCNQuaternion.Slerp (q0, qf, (float)factor);
			SCNMatrix4 rTmp = qTmp.ToMatrix4 ();

			SCNMatrix4 transform = new SCNMatrix4 (
				                       rTmp.M11, rTmp.M12, rTmp.M13, 0.0f,
				                       rTmp.M21, rTmp.M22, rTmp.M23, 0.0f,
				                       rTmp.M31, rTmp.M32, rTmp.M33, 0.0f,
				                       pTmp.X, pTmp.Y, pTmp.Z, 1.0f
			                       );

			return transform;
		}

		public static SCNVector4 Lerp (SCNVector4 vectorStart, SCNVector4 vectorEnd, nfloat t)
		{
			var v = new SCNVector4 (
				        vectorStart.X + ((vectorEnd.X - vectorStart.X) * t),
				        vectorStart.Y + ((vectorEnd.Y - vectorStart.Y) * t),
				        vectorStart.Z + ((vectorEnd.Z - vectorStart.Z) * t),
				        vectorStart.W + ((vectorEnd.W - vectorStart.W) * t)
			        );

			return v;
		}

		public static SCNVector3 GetMaxVector (SCNVector3 vectorLeft, SCNVector3 vectorRight)
		{
			SCNVector3 max = vectorLeft;
			if (vectorRight.X > vectorLeft.X)
				max.X = vectorRight.X;
			if (vectorRight.Y > vectorLeft.Y)
				max.Y = vectorRight.Y;
			if (vectorRight.Z > vectorLeft.Z)
				max.Z = vectorRight.Z;
			return max;
		}

		public static SCNVector3 GetMinVector (SCNVector3 vectorLeft, SCNVector3 vectorRight)
		{
			SCNVector3 min = vectorLeft;
			if (vectorRight.X < vectorLeft.X)
				min.X = vectorRight.X;
			if (vectorRight.Y < vectorLeft.Y)
				min.Y = vectorRight.Y;
			if (vectorRight.Z < vectorLeft.Z)
				min.Z = vectorRight.Z;
			return min;
		}

		public static SCNQuaternion ToQuaternion (this SCNMatrix4 matrix)
		{
			nfloat trace = 1 + matrix.M11 + matrix.M22 + matrix.M33;
			nfloat s = 0;
			nfloat x = 0;
			nfloat y = 0;
			nfloat z = 0;
			nfloat w = 0;
			
			if (trace > 0.0000001) {
				s = (float)Math.Sqrt (trace) * 2;
				x = (matrix.M23 - matrix.M32) / s;
				y = (matrix.M31 - matrix.M13) / s;
				z = (matrix.M12 - matrix.M21) / s;
				w = 0.25f * s;
			} else {
				if (matrix.M11 > matrix.M22 && matrix.M11 > matrix.M33) {
					// Column 0: 
					s = (float)Math.Sqrt (1.0 + matrix.M11 - matrix.M22 - matrix.M33) * 2;
					x = 0.25f * s;
					y = (matrix.M12 + matrix.M21) / s;
					z = (matrix.M31 + matrix.M13) / s;
					w = (matrix.M23 - matrix.M32) / s;
				} else if (matrix.M22 > matrix.M33) {
					// Column 1: 
					s = (float)Math.Sqrt (1.0 + matrix.M22 - matrix.M11 - matrix.M33) * 2;
					x = (matrix.M12 + matrix.M21) / s;
					y = 0.25f * s;
					z = (matrix.M23 + matrix.M32) / s;
					w = (matrix.M31 - matrix.M13) / s;
				} else {
					// Column 2:
					s = (float)Math.Sqrt (1.0 + matrix.M33 - matrix.M11 - matrix.M22) * 2;
					x = (matrix.M31 + matrix.M13) / s;
					y = (matrix.M23 + matrix.M32) / s;
					z = 0.25f * s;
					w = (matrix.M12 - matrix.M21) / s;
				}
			}

			return new SCNQuaternion (x, y, z, w);
		}

		public static SCNMatrix4 ToMatrix4 (this SCNQuaternion quaternion)
		{
			quaternion.Normalize ();

			nfloat x = quaternion.X;
			nfloat y = quaternion.Y;
			nfloat z = quaternion.Z;
			nfloat w = quaternion.W;

			nfloat doubleX = x + x;
			nfloat doubleY = y + y;
			nfloat doubleZ = z + z;
			nfloat doubleW = w + w;

			return new SCNMatrix4 (
				1.0f - doubleY * y - doubleZ * z,
				doubleX * y + doubleW * z,
				doubleX * z - doubleW * y,
				0.0f,
				doubleX * y - doubleW * z,
				1.0f - doubleX * x - doubleZ * z,
				doubleY * z + doubleW * x,
				0.0f,
				doubleX * z + doubleW * y,
				doubleY * z - doubleW * x,
				1.0f - doubleX * x - doubleY * y,
				0.0f,
				0.0f,
				0.0f,
				0.0f,
				1.0f 
			);
		}
	}
}

