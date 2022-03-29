namespace System.HttpProxy
{
    public class HttpBuffer
    {
        private int size = 2;
        public HttpBuffer()
        {
            _buffer = new byte[1024 * size];
        }
        public HttpBuffer(int size) : this()
        {
            this.size = size;
        }

        public int RealLength { get; set; }

        private byte[] _buffer;
        public byte[] Buffer
        {
            get
            {
                return _buffer;
            }
            set
            {
                _buffer = value;
            }
        }

        private byte[] _streams;
        public byte[] GetStream()
        {
            if (this._streams == null)
            {
                return new byte[0];
            }
            return this._streams;
        }

        public void Stream(int? receive = null)
        {
            if (this._streams == null)
            {
                return;
            }
            if (receive == null)
            {
                receive = _buffer.Length;
            }
            long len = (long)receive;
            len += this._streams.Length;
            byte[] newByte = new byte[len];
            this._streams.CopyTo(newByte, 0);
            for (int i = 0; i < receive; i++)
            {
                if(_buffer[i] == '\0')
                {
                    break;
                }
                newByte[this._streams.Length + i] = _buffer[i];
            }            
            this._streams = newByte;
        }

        public void Clear()
        {
            if (this._streams == null)
            {
                _streams = new byte[0];
            }            
            _buffer = new byte[1024 * size];
        }

        public void Dispose()
        {
            _streams = null;
            _buffer = null;
        }
    }
}