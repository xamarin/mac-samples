using AppKit;

namespace VisualEffectPlayground
{
	public static class ImageLoader
	{
		public static void LoadImage(NSImageView imageView, params string[] sources)
		{
			if (imageView != null && sources != null && sources.Length > 0)
			{
				foreach (var source in sources)
				{
					if (System.IO.File.Exists(source))
					{
						imageView.Image = new NSImage(source);
						break;
					}
				}
			}
		}
	}
}