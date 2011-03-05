using System;
using MonoMac.OpenGL;

namespace GLFullScreen
{
	public class Geometry {
		// This creates a symmetric frustum.
		// It converts to 6 params (l, r, b, t, n, f) for glFrustum()
		// from given 4 params (fovy, aspect, near, far)
		public static void Perspective (double fovY, double aspectRatio, double front, double back)
		{
		    const double DEG2RAD = Math.PI / 180;

		    // tangent of half fovY
		    double tangent = Math.Tan (fovY/2 * DEG2RAD);

		    // half height of near plane
		    double height = front * tangent;

		    // half width of near plane
		    double width = height * aspectRatio;
		
		    // params: left, right, bottom, top, near, far
		    GL.Frustum(-width, width, -height, height, front, back);
		}
	}
}
