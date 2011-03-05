// 
// Copyright (c) 2011 Kenneth J. Pouncey
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Text;

using MonoMac.OpenGL;

namespace GLFullScreen
{
	/*
	 * This shape class tries to be a drop in replacement for the OpenGL Utilities Sphere class
	 * without all the different definition parameters.
	 * 
	 * glSphere - Smooth, Textured, DrawStyle of Line
	 * 
	 * You can control whether to use fill, line or point by specifying that via 
	 *              if (wireFrame)
	 *			GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Line);
	 *		else
	 *			GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Fill);
	 *
	 * There have been an enhancment where it allows you specify a parameter
	 * to compile the sphere as an OpenGL Display List.
	 * 
	 */
	public class Sphere : IDisposable
	{
		const int CACHE_SIZE = 240;

		public struct Vertex
		{
			public Vector2d TexCoord;
			public Vector3d Normal;
			public Vector3d Position;

			public Vertex (Vector2d texcoord, Vector3d normal, Vector3d position)
			{
				TexCoord = texcoord;
				Normal = normal;
				Position = position;
			}
		}

		protected Vertex[] Vertices;

		#region Display List

		private bool isUseDisplayList;
		private int myDisplayListHandle = 0;

		#endregion Display List

		int i, j;
		double[] sinCache1a = new double[CACHE_SIZE];
		double[] cosCache1a = new double[CACHE_SIZE];
		double[] sinCache2a = new double[CACHE_SIZE];
		double[] cosCache2a = new double[CACHE_SIZE];
		double[] sinCache1b = new double[CACHE_SIZE];
		double[] cosCache1b = new double[CACHE_SIZE];
		double[] sinCache2b = new double[CACHE_SIZE];
		double[] cosCache2b = new double[CACHE_SIZE];
		double angle;
		double zLow, zHigh;
		double sintemp1 = 0;
		double sintemp2 = 0;
		double sintemp3 = 0;
		double costemp3 = 0;
		double sintemp4 = 0;
		double costemp4 = 0;

		int start, finish;

		double radius;
		int slices;
		int stacks;


		public Sphere (double radius, int slices, int stacks, bool useDisplayList)
		{
			if (slices >= CACHE_SIZE)
				slices = CACHE_SIZE - 1;
			if (stacks >= CACHE_SIZE)
				stacks = CACHE_SIZE - 1;
			if (slices < 2 || stacks < 1 || radius < 0.0)
				throw new Exception ("Invalid parameters to Sphere creation");
			
			this.radius = radius;
			this.slices = slices;
			this.stacks = stacks;
			
			isUseDisplayList = useDisplayList;
			
			// This is allocating to much space and needs to be looked at 
			// to manage memory better.
			Vertices = new Vertex[(slices * stacks) * 3];
			
			InitSphere ();
			
		}

		public void InitSphere ()
		{
			for (i = 0; i < slices; i++) {
				angle = 2 * Math.PI * i / slices;
				sinCache1a[i] = Math.Sin (angle);
				cosCache1a[i] = Math.Cos (angle);
				sinCache2a[i] = sinCache1a[i];
				cosCache2a[i] = cosCache1a[i];
			}
			
			for (j = 0; j <= stacks; j++) {
				angle = Math.PI * j / stacks;
				sinCache2b[j] = Math.Sin (angle);
				cosCache2b[j] = Math.Cos (angle);

				sinCache1b[j] = radius * Math.Sin (angle);
				cosCache1b[j] = radius * Math.Cos (angle);
				
			}
			
			// Make sure it comes to a point 
			sinCache1b[0] = 0;
			sinCache1b[stacks] = 0;
			
			sinCache1a[slices] = sinCache1a[0];
			cosCache1a[slices] = cosCache1a[0];
			sinCache2a[slices] = sinCache2a[0];
			cosCache2a[slices] = cosCache2a[0];
			
			int vertexIndex = 0;
			start = 0;
			finish = stacks;

			for (j = start; j < finish; j++) {
				zLow = cosCache1b[j];
				zHigh = cosCache1b[j + 1];
				sintemp1 = sinCache1b[j];
				sintemp2 = sinCache1b[j + 1];
				
				sintemp3 = sinCache2b[j + 1];
				costemp3 = cosCache2b[j + 1];
				sintemp4 = sinCache2b[j];
				costemp4 = cosCache2b[j];
				
				for (i = 0; i <= slices; i++) {
					
					Vector3d vv = new Vector3d (sintemp2 * sinCache1a[i], sintemp2 * cosCache1a[i], zHigh);
					Vector3d nv = new Vector3d (sinCache2a[i] * sintemp3, cosCache2a[i] * sintemp3, costemp3);
					Vector2d uvv = new Vector2d (1 - (float)i / slices, 1 - (float)(j + 1) / stacks);
					
					Vertices[vertexIndex].Position = vv;
					Vertices[vertexIndex].Normal = nv;
					Vertices[vertexIndex].TexCoord = uvv;
					vertexIndex++;
					
					Vector3d vh = new Vector3d (sintemp1 * sinCache1a[i], sintemp1 * cosCache1a[i], zLow);
					Vector3d nh = new Vector3d (sinCache2a[i] * sintemp4, cosCache2a[i] * sintemp4, costemp4);
					Vector2d uvh = new Vector2d (1 - (float)i / slices, 1 - (float)j / stacks);
					
					Vertices[vertexIndex].Position = vh;
					Vertices[vertexIndex].Normal = nh;
					Vertices[vertexIndex].TexCoord = uvh;
					vertexIndex++;
				}
			}
			
		}

		/// <summary>
		/// Creates and compiles a display list if not present yet. Requires an OpenGL context.
		/// </summary>
		public void Draw ()
		{
			if (!isUseDisplayList) {
				DrawImmediateMode ();
			} else if (myDisplayListHandle == 0) {
				if (Vertices == null)
					throw new Exception ("Cannot draw null Vertex Array.");
				myDisplayListHandle = GL.GenLists (1);
				GL.NewList (myDisplayListHandle, ListMode.CompileAndExecute);
				DrawImmediateMode ();
				GL.EndList ();
			} else
				GL.CallList (myDisplayListHandle);
		}

		private void DrawImmediateMode ()
		{
			GL.Begin (BeginMode.QuadStrip);
			{
				foreach (Vertex v in Vertices) {
					GL.TexCoord2 (v.TexCoord.X, v.TexCoord.Y);
					GL.Normal3 (v.Normal.X, v.Normal.Y, v.Normal.Z);
					GL.Vertex3 (v.Position.X, v.Position.Y, v.Position.Z);
				}
				
			}
			GL.End ();
		}

		#region IDisposable Members

		/// <summary>
		/// Removes reference to VertexArray.
		/// Deletes the Display List, so it requires an OpenGL context.
		/// The instance is effectively destroyed.
		/// </summary>
		public void Dispose ()
		{
			if (Vertices != null)
				Vertices = null;
			if (myDisplayListHandle != 0) {
				GL.DeleteLists (myDisplayListHandle, 1);
				myDisplayListHandle = 0;
			}
		}
		
		#endregion
	}
	
}
