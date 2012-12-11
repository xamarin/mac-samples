//=====================================================================
// http://www.cocoa-mono.org
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
using System.Drawing;
using System.IO;

using MonoMac.Foundation;
using MonoMac.AppKit;

using MonoMac.OpenGL;

namespace GLSLShader
{
	public class Scene
	{

		int shaderProgram;
		int vertexShader;
		int fragmentShader;
		int geometryShader;

		public Scene () : base()
		{
			SetupShaders ();
		}

		// Resize And Initialize The GL Window 
		//      - See also the method in the MyOpenGLView Constructor about the NSView.NSViewGlobalFrameDidChangeNotification
		public void ResizeGLScene (RectangleF bounds)
		{
			// Reset The Current Viewport
			GL.Viewport (0, 0, (int)bounds.Size.Width, (int)bounds.Size.Height);
			// Select The Projection Matrix
			GL.MatrixMode (MatrixMode.Projection);
			// Reset The Projection Matrix
			GL.LoadIdentity ();

			GL.Ortho (0, bounds.Width, bounds.Height, 0, 0, 1);

			// Select The Modelview Matrix
			GL.MatrixMode (MatrixMode.Modelview);
			// Reset The Modelview Matrix
			GL.LoadIdentity ();
		}

		private void SetupShaders ()
		{

			// Create the shader objects
			vertexShader = GL.CreateShader (ShaderType.VertexShader);
			fragmentShader = GL.CreateShader (ShaderType.FragmentShader);
			geometryShader = GL.CreateShader (ShaderType.GeometryShaderExt);

			// Load the source into a string
			string vertexShaderSource = LoadShaderSource ("shader.vert");
			// Attach the loaded source string to the shader object
			GL.ShaderSource (vertexShader, vertexShaderSource);
			// Compile the shader
			GL.CompileShader (vertexShader);

			// Load the source into a string
			string fragmentShaderSource = LoadShaderSource ("shader.frag"); 
			// Attach the loaded source string to the shader object
			GL.ShaderSource (fragmentShader, fragmentShaderSource);
			// Compile the shader
			GL.CompileShader (fragmentShader);

			// Create a Program object
			shaderProgram = GL.CreateProgram ();

			// Attach our compiled shaders
			GL.AttachShader (shaderProgram, vertexShader);
			GL.AttachShader (shaderProgram, fragmentShader);

			// Set the parameters
			GL.ProgramParameter (shaderProgram, AssemblyProgramParameterArb.GeometryInputType, (int)All.Lines);	
			GL.ProgramParameter (shaderProgram, AssemblyProgramParameterArb.GeometryOutputType, (int)All.Line);

			// Set the max vertices
			int maxVertices;
			GL.GetInteger (GetPName.MaxGeometryOutputVertices, out maxVertices);
			GL.ProgramParameter (shaderProgram, AssemblyProgramParameterArb.GeometryVerticesOut, maxVertices);

			// Link the program
			GL.LinkProgram (shaderProgram);
			// Tell the GL Context to use the program
			GL.UseProgram (shaderProgram);

			// Output our shader object errors if there were problems 
			ShaderLog ("Vertex Shader:", vertexShader);
			ShaderLog ("Fragment Shader:", fragmentShader);
			ShaderLog ("Shader Program:", shaderProgram);
		}

		// Output the log of an object
		private void ShaderLog (string whichObj, int obj)
		{
			int infoLogLen = 0;
			var infoLog = "Is good to go.";

			GL.GetProgram (obj, ProgramParameter.InfoLogLength, out infoLogLen);

			if (infoLogLen > 0)
				infoLog = GL.GetProgramInfoLog (obj);

			Console.WriteLine ("{0} {1}", whichObj, infoLog);

		}

		// Load the source code of a GLSL program from the content
		private string LoadShaderSource (string name)
		{

			var path = NSBundle.MainBundle.ResourcePath + Path.DirectorySeparatorChar + "GLSL";
			var filePath = path + Path.DirectorySeparatorChar + name;
			StreamReader streamReader = new StreamReader (filePath);
			string text = streamReader.ReadToEnd ();
			streamReader.Close ();

			return text;

		}

		// This method renders our scene and where all of your drawing code will go.
		// The main thing to note is that we've factored the drawing code out of the NSView subclass so that
		// the full-screen and non-fullscreen views share the same states for rendering 
		public bool DrawGLScene ()
		{
			// Clear The Screen And The Depth Buffer
			GL.Clear (ClearBufferMask.ColorBufferBit);// | ClearBufferMask.DepthBufferBit);
			// Reset The Current Modelview Matrix
			GL.LoadIdentity ();

			// Obtain the viewport object so that we can obtain our width and height
			int[] viewport = new int[4];
			GL.GetInteger (GetPName.Viewport, viewport);			

			// Setup our vertex to output.  
			// Simple line from top to bottom of the screen
			GL.Begin (BeginMode.Lines);
				GL.Vertex2 (viewport [2] / 2, 0);
				GL.Vertex2 (viewport [2] / 2, viewport [3]);
			GL.End ();

			return true;
		}

	}
}

