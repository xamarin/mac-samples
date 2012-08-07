//
// AsyncTests.HttpClientTests.Test.RandomDataStream
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//
// Copyright 2012 Xamarin Inc. (http://www.xamarin.com)
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
using System.IO;

namespace AsyncTests.HttpClientTests.Test {

	public class RandomDataStream : Stream {
		Random random = new Random ();
		long requestedLength;
		long position;

		public RandomDataStream (long requestedLength)
		{
			this.requestedLength = requestedLength;
		}

		public override void Flush ()
		{
			;
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			var remaining = (int)Math.Min (count, requestedLength - position);
			if (remaining == 0)
				return 0;

			var data = new byte [count];
			random.NextBytes (data);
			Buffer.BlockCopy (data, 0, buffer, offset, remaining);

			position += remaining;
			return remaining;
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotSupportedException ();
		}

		public override void SetLength (long value)
		{
			throw new NotSupportedException ();
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

		public override bool CanRead {
			get { return true; }
		}

		public override bool CanSeek {
			get { return false; }
		}

		public override bool CanWrite {
			get { return false; }
		}

		public override long Length {
			get { return requestedLength; }
		}

		public override long Position {
			get { return position; }
			set {
				throw new NotSupportedException ();
			}
		}
	}
}
