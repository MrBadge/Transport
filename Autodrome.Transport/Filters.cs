using System;
using System.Collections.Generic;
using System.Text;

namespace Autodrom
{
    public struct FilterResult
    {
        public FilterResult(int _offset, int _size)
        {
            offset = _offset;
            size = _size;
        }

        private int offset; // message offset or end of garbage. 0 means signature not found
        private int size;   // header + message + crc. 0 means invalid

        public int Offset { get { return offset; } }
        public int Size { get { return size; } }
        public bool Found { get { return (offset >= 0); } }
        public bool Complete { get { return (offset >= 0) && (size >= 0); } }
        public bool Valid { get { return (offset >= 0) && (size > 0); } }
    }

    public delegate FilterResult FilterFunction(byte[] data, int offset, int size);

    class StreamFilter
    {
        static public FilterResult AcceptAll(byte[] data, int offset, int size)
        {
            return new FilterResult(offset, size);
        }
        static public FilterResult RejectAll(byte[] data, int offset, int size)
        {
            return new FilterResult(-1, 0);
        }
    }
}
