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

    public sealed class ZStream{

        private const int MAX_WBITS=15;        // 32K LZ77 window
        private const int DEF_WBITS=MAX_WBITS;

        private const int Z_NO_FLUSH=0;
        private const int Z_PARTIAL_FLUSH=1;
        private const int Z_SYNC_FLUSH=2;
        private const int Z_FULL_FLUSH=3;
        private const int Z_FINISH=4;

        private const int MAX_MEM_LEVEL=9;

        private const int Z_OK=0;
        private const int Z_STREAM_END=1;
        private const int Z_NEED_DICT=2;
        private const int Z_ERRNO=-1;
        private const int Z_STREAM_ERROR=-2;
        private const int Z_DATA_ERROR=-3;
        private const int Z_MEM_ERROR=-4;
        private const int Z_BUF_ERROR=-5;
        private const int Z_VERSION_ERROR=-6;

        public byte[] next_in;     // next input byte
        public int next_in_index;
        public int avail_in;       // number of bytes available at next_in
        public long total_in;      // total nb of input bytes read so far

        public byte[] next_out;    // next output byte should be put there
        public int next_out_index;
        public int avail_out;      // remaining free space at next_out
        public long total_out;     // total nb of bytes output so far

        public String msg;

        internal Deflate dstate; 
        internal Inflate istate; 

        internal int data_type; // best guess about the data type: ascii or binary

        public long adler;
        internal Adler32 _adler=new Adler32();

        public int inflateInit(){
            return inflateInit(DEF_WBITS);
        }
        public int inflateInit(bool nowrap){
            return inflateInit(DEF_WBITS, nowrap);
        }
        public int inflateInit(int w){
            return inflateInit(w, false);
        }

        public int inflateInit(int w, bool nowrap){
            istate=new Inflate();
            return istate.inflateInit(this, nowrap?-w:w);
        }

        public int inflate(int f){
            if(istate==null) return Z_STREAM_ERROR;
            return istate.inflate(this, f);
        }
        public int inflateEnd(){
            if(istate==null) return Z_STREAM_ERROR;
            int ret=istate.inflateEnd(this);
            istate = null;
            return ret;
        }
        public int inflateSync(){
            if(istate == null)
                return Z_STREAM_ERROR;
            return istate.inflateSync(this);
        }
        public int inflateSetDictionary(byte[] dictionary, int dictLength){
            if(istate == null)
                return Z_STREAM_ERROR;
            return istate.inflateSetDictionary(this, dictionary, dictLength);
        }

        public int deflateInit(int level){
            return deflateInit(level, MAX_WBITS);
        }
        public int deflateInit(int level, bool nowrap){
            return deflateInit(level, MAX_WBITS, nowrap);
        }
        public int deflateInit(int level, int bits){
            return deflateInit(level, bits, false);
        }
        public int deflateInit(int level, int bits, bool nowrap){
            dstate=new Deflate();
            return dstate.deflateInit(this, level, nowrap?-bits:bits);
        }
        public int deflate(int flush){
            if(dstate==null){
                return Z_STREAM_ERROR;
            }
            return dstate.deflate(this, flush);
        }
        public int deflateEnd(){
            if(dstate==null) return Z_STREAM_ERROR;
            int ret=dstate.deflateEnd();
            dstate=null;
            return ret;
        }
        public int deflateParams(int level, int strategy){
            if(dstate==null) return Z_STREAM_ERROR;
            return dstate.deflateParams(this, level, strategy);
        }
        public int deflateSetDictionary (byte[] dictionary, int dictLength){
            if(dstate == null)
                return Z_STREAM_ERROR;
            return dstate.deflateSetDictionary(this, dictionary, dictLength);
        }

        // Flush as much pending output as possible. All deflate() output goes
        // through this function so some applications may wish to modify it
        // to avoid allocating a large strm->next_out buffer and copying into it.
        // (See also read_buf()).
        internal void flush_pending(){
            int len=dstate.pending;

            if(len>avail_out) len=avail_out;
            if(len==0) return;

            if(dstate.pending_buf.Length<=dstate.pending_out ||
                next_out.Length<=next_out_index ||
                dstate.pending_buf.Length<(dstate.pending_out+len) ||
                next_out.Length<(next_out_index+len)){
                //      System.out.println(dstate.pending_buf.length+", "+dstate.pending_out+
                //			 ", "+next_out.length+", "+next_out_index+", "+len);
                //      System.out.println("avail_out="+avail_out);
            }

            System.Array.Copy(dstate.pending_buf, dstate.pending_out,
                next_out, next_out_index, len);

            next_out_index+=len;
            dstate.pending_out+=len;
            total_out+=len;
            avail_out-=len;
            dstate.pending-=len;
            if(dstate.pending==0){
                dstate.pending_out=0;
            }
        }

        // Read a new buffer from the current input stream, update the adler32
        // and total number of bytes read.  All deflate() input goes through
        // this function so some applications may wish to modify it to avoid
        // allocating a large strm->next_in buffer and copying from it.
        // (See also flush_pending()).
        internal int read_buf(byte[] buf, int start, int size) {
            int len=avail_in;

            if(len>size) len=size;
            if(len==0) return 0;

            avail_in-=len;

            if(dstate.noheader==0) {
                adler=_adler.adler32(adler, next_in, next_in_index, len);
            }
            System.Array.Copy(next_in, next_in_index, buf, start, len);
            next_in_index  += len;
            total_in += len;
            return len;
        }

        public void free(){
            next_in=null;
            next_out=null;
            msg=null;
            _adler=null;
        }
    }
}