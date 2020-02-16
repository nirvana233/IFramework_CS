/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-26
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Net
{
    public class BufferSegment
    {
        private byte[] _buffer;
        private int _offset;
        private int _count;
        public byte[] buffer { get { return _buffer; } set { _buffer = value; } }
        public int offset { get { return _offset; } set { _offset = value; } }
        public int count { get { return _count; } set { _count = value; } }
        public BufferSegment() { }
        public BufferSegment(byte[] buffer) : this(buffer, 0, buffer.Length) { }
        public BufferSegment(byte[] buffer, int count) : this(buffer, 0, count) { }
        public BufferSegment(byte[] buffer, int offset, int count)
        {
            this._buffer = buffer;
            this._offset = offset;
            this._count = count;
        }
    }

}
