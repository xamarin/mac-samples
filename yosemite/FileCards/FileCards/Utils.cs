using System;
using Foundation;

namespace FileCards
{
	public static class Utils
	{
		public static bool IsDir (this NSUrl url)
		{
			NSError error;
			NSObject isDirNum;

			if (url.TryGetResource (NSUrl.IsDirectoryKey, out isDirNum, out error)) {
				bool isDir = ((NSNumber)isDirNum).BoolValue;
				return isDir;
			}

			throw new NSErrorException (error);
		}
	}
}