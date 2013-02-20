using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Sundown
{
	public sealed class Renderer : IDisposable
	{
		const string Import = "@executable_path/../Resources/libsundown.dylib";

		enum mkd_extensions {
			MKDEXT_NO_INTRA_EMPHASIS = (1 << 0),
			MKDEXT_TABLES = (1 << 1),
			MKDEXT_FENCED_CODE = (1 << 2),
			MKDEXT_AUTOLINK = (1 << 3),
			MKDEXT_STRIKETHROUGH = (1 << 4),
			MKDEXT_SPACE_HEADERS = (1 << 6),
			MKDEXT_SUPERSCRIPT = (1 << 7),
			MKDEXT_LAX_SPACING = (1 << 8),
			
			ALL =
				MKDEXT_NO_INTRA_EMPHASIS |
				MKDEXT_TABLES |
				MKDEXT_FENCED_CODE |
				MKDEXT_AUTOLINK |
				MKDEXT_STRIKETHROUGH |
				MKDEXT_SPACE_HEADERS |
				MKDEXT_SUPERSCRIPT |
				MKDEXT_LAX_SPACING
		}

		static readonly int sd_callbacks_Size = 26 * IntPtr.Size; // 26 function pointers
		static readonly int html_renderopt_Size = 4 * 4 + IntPtr.Size; // 4 int32 + 1 pointer

		[DllImport (Import)]
		static extern void sdhtml_renderer (IntPtr callbacks, IntPtr htmlRenderOpts, uint render_flags);

		[DllImport (Import)]
		static extern IntPtr sd_markdown_new (mkd_extensions extensions, IntPtr max_nesting, IntPtr callbacks, IntPtr opaque);

		[DllImport (Import)]
		static extern void sd_markdown_free (IntPtr md);

		[DllImport (Import)]
		static extern void sd_markdown_render (IntPtr ob, byte [] document, IntPtr doc_size, IntPtr md);

		[DllImport (Import)]
		static extern IntPtr bufnew (IntPtr size);

		[DllImport (Import)]
		static extern void bufrelease (IntPtr buf);

		IntPtr handle;
		IntPtr sdCallbacks;
		IntPtr htmlRenderOpts;

		public Renderer ()
		{
			sdCallbacks = Marshal.AllocHGlobal (sd_callbacks_Size);
			htmlRenderOpts = Marshal.AllocHGlobal (html_renderopt_Size);

			sdhtml_renderer (sdCallbacks, htmlRenderOpts, 0);
			handle = sd_markdown_new (mkd_extensions.ALL, new IntPtr (16),  sdCallbacks, htmlRenderOpts);
		}

		public string Render (string str)
		{
			var ob = IntPtr.Zero;

			try {
				var ib = Encoding.UTF8.GetBytes (str);
				ob = bufnew (new IntPtr (64));

				sd_markdown_render (ob, ib, new IntPtr (ib.Length), handle);

				var obLen = Marshal.ReadInt32 (ob, IntPtr.Size);
				var obDataPtr = Marshal.ReadIntPtr (ob);
				var obBytes = new byte [obLen];
	
				// woo super slow marshalling!
				for (int i = 0; i < obLen; i++) {
					obBytes[i] = Marshal.ReadByte (obDataPtr, i);
				}

				return Encoding.UTF8.GetString (obBytes);
			} finally {
				if (ob != IntPtr.Zero) {
					bufrelease (ob);
				}
			}
		}

		~Renderer ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				sd_markdown_free (handle);
				handle = IntPtr.Zero;
			}

			if (sdCallbacks != IntPtr.Zero) {
				Marshal.FreeHGlobal (sdCallbacks);
				sdCallbacks = IntPtr.Zero;
			}

			if (htmlRenderOpts != IntPtr.Zero) {
				Marshal.FreeHGlobal (htmlRenderOpts);
				htmlRenderOpts = IntPtr.Zero;
			}
		}
	}
}