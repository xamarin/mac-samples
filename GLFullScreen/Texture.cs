using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.OpenGL;

namespace GLFullScreen
{
        public class Texture : NSObject
        {
                int texId;
                int pboId;
                byte[] data;

                const int TEXTURE_WIDTH = 1024;
                const int TEXTURE_HEIGHT = 512;

                private Texture () : base()
                {
                }

                public Texture (string path) : base()
                {
                        GetImagaDataFromPath (path);
                        LoadTexture ();
                }

                void GetImagaDataFromPath (string path)
                {
                        int width, height;
                        NSImage src;
                        CGImage image;
                        CGContext context = null;
                        
                        data = new byte[TEXTURE_WIDTH * TEXTURE_HEIGHT * 4];
                        
                        src = new NSImage (path);
                        
                        image = src.AsCGImage (RectangleF.Empty, null, null);
                        width = image.Width;
                        height = image.Height;
                        
                        CGImageAlphaInfo ai = CGImageAlphaInfo.PremultipliedLast;
                        
                        context = new CGBitmapContext (data, width, height, 8, 4 * width, image.ColorSpace, ai);
                        
                        // Core Graphics referential is upside-down compared to OpenGL referential
                        // Flip the Core Graphics context here
                        // An alternative is to use flipped OpenGL texture coordinates when drawing textures
                        context.TranslateCTM (0, height);
                        context.ScaleCTM (1, -1);
                        
                        // Set the blend mode to copy before drawing since the previous contents of memory aren't used. 
                        // This avoids unnecessary blending.
                        context.SetBlendMode (CGBlendMode.Copy);
                        
                        context.DrawImage (new RectangleF (0, 0, width, height), image);
                }

                void LoadTexture ()
                {
                        GL.GenTextures (1, out texId);
                        GL.GenBuffers (1, out pboId);
                        
                        // Bind the texture
                        GL.BindTexture (TextureTarget.Texture2D, texId);
                        
                        // Bind the PBO
                        GL.BindBuffer (BufferTarget.PixelUnpackBuffer, pboId);
                        
                        
                        // Upload the texture data to the PBO
                        GL.BufferData (BufferTarget.PixelUnpackBuffer, new IntPtr (TEXTURE_WIDTH * TEXTURE_HEIGHT * 4 * sizeof(byte)), data, BufferUsageHint.StaticDraw);
                        
                        // Setup texture parameters
                        GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                        GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                        GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                        GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
                        
                        GL.PixelStore (PixelStoreParameter.UnpackRowLength, 0);
                        
                        // OpenGL likes the GL_BGRA + GL_UNSIGNED_INT_8_8_8_8_REV combination
                        // Use offset instead of pointer to indictate that we want to use data copied from a PBO 
                        GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TEXTURE_WIDTH, TEXTURE_HEIGHT, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                        data = null;
                        
                        GL.BindTexture (TextureTarget.Texture2D, 0);
                        GL.BindBuffer (BufferTarget.PixelUnpackBuffer, 0);
                }

                public int TextureName {
                        get { return texId; }
                }
        }
}

