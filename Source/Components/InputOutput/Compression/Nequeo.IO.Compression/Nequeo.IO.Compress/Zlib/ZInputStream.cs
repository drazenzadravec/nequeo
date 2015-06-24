/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Nequeo.IO.Compression.Zlib
{
	public class ZInputStream
		: Stream
	{
		private const int BufferSize = 512;

		protected ZStream z = new ZStream();
		protected int flushLevel = JZlib.Z_NO_FLUSH;
		// TODO Allow custom buf
		protected byte[] buf = new byte[BufferSize];
		protected byte[] buf1 = new byte[1];
		protected bool compress;

		protected Stream input;
		protected bool closed;

		private bool nomoreinput = false;

		public ZInputStream(Stream input)
			: this(input, false)
		{
		}

		public ZInputStream(Stream input, bool nowrap)
		{
			this.input = input;
			this.z.inflateInit(nowrap);
			this.compress = false;
			this.z.next_in = buf;
			this.z.next_in_index = 0;
			this.z.avail_in = 0;
		}

		public ZInputStream(Stream input, int level)
		{
			this.input = input;
			this.z.deflateInit(level);
			this.compress = true;
			this.z.next_in = buf;
			this.z.next_in_index = 0;
			this.z.avail_in = 0;
		}

		/*public int available() throws IOException {
		return inf.finished() ? 0 : 1;
		}*/

		public sealed override bool CanRead { get { return !closed; } }
		public sealed override bool CanSeek { get { return false; } }
		public sealed override bool CanWrite { get { return false; } }

		public override void Close()
		{
			if (!closed)
			{
				closed = true;
				input.Close();
			}
		}

		public sealed override void Flush() {}

		public virtual int FlushMode
		{
			get { return flushLevel; }
			set { this.flushLevel = value; }
		}

		public sealed override long Length { get { throw new NotSupportedException(); } }
		public sealed override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		public override int Read(byte[]	b, int off, int len)
		{
			if (len==0)
				return 0;

			z.next_out = b;
			z.next_out_index = off;
			z.avail_out = len;

			int err;
			do
			{
				if (z.avail_in == 0 && !nomoreinput)
				{
					// if buffer is empty and more input is available, refill it
					z.next_in_index = 0;
					z.avail_in = input.Read(buf, 0, buf.Length); //(bufsize<z.avail_out ? bufsize : z.avail_out));

					if (z.avail_in <= 0)
					{
						z.avail_in = 0;
						nomoreinput = true;
					}
				}

				err = compress
					?	z.deflate(flushLevel)
					:	z.inflate(flushLevel);

				if (nomoreinput && err == JZlib.Z_BUF_ERROR)
					return 0;
				if (err != JZlib.Z_OK && err != JZlib.Z_STREAM_END)
					// TODO
//					throw new ZStreamException((compress ? "de" : "in") + "flating: " + z.msg);
					throw new IOException((compress ? "de" : "in") + "flating: " + z.msg);
				if ((nomoreinput || err == JZlib.Z_STREAM_END) && z.avail_out == len)
					return 0;
			} 
			while(z.avail_out == len && err == JZlib.Z_OK);
			//Console.Error.WriteLine("("+(len-z.avail_out)+")");
			return len - z.avail_out;
		}

		public override int ReadByte()
		{
			if (Read(buf1, 0, 1) <= 0)
				return -1;
			return buf1[0];
		}

//  public long skip(long n) throws IOException {
//    int len=512;
//    if(n<len)
//      len=(int)n;
//    byte[] tmp=new byte[len];
//    return((long)read(tmp));
//  }

		public sealed override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
		public sealed override void SetLength(long value) { throw new NotSupportedException(); }

		public virtual long TotalIn
		{
			get { return z.total_in; }
		}

		public virtual long TotalOut
		{
			get { return z.total_out; }
		}

		public sealed override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
	}
}
