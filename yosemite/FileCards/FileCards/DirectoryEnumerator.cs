using System;
using System.Collections.Generic;

using Foundation;

namespace FileCards
{
	public class DirectoryEnumerable : IEnumerable<NSUrl>
	{
		readonly NSDirectoryEnumerator nativeEnumerator;

		public DirectoryEnumerable (NSDirectoryEnumerator nativeEnumerator)
		{
			this.nativeEnumerator = nativeEnumerator;
		}

		public IEnumerator<NSUrl> GetEnumerator ()
		{
			return new DirectoryEnumerator (nativeEnumerator);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return new DirectoryEnumerator (nativeEnumerator);
		}
	}

	internal class DirectoryEnumerator : IEnumerator<NSUrl>
	{
		readonly NSDirectoryEnumerator nativeEnumerator;

		public DirectoryEnumerator (NSDirectoryEnumerator nativeEnumerator)
		{
			this.nativeEnumerator = nativeEnumerator;
		}

		#region IEnumerator implementation

		public bool MoveNext ()
		{
			Current = nativeEnumerator.NextObject ();
			return Current != null;
		}

		public void Reset ()
		{
			throw new NotSupportedException ();
		}

		public object Current { get; private set; }

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			nativeEnumerator.Dispose ();
		}

		#endregion

		#region IEnumerator implementation

		NSUrl IEnumerator<NSUrl>.Current {
			get {
				return (NSUrl)Current;
			}
		}

		#endregion

	}
}