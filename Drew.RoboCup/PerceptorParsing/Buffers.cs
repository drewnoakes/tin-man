/*
 * Created by Drew, 10/05/2010 13:54.
 */
using System;
using System.IO;

namespace Drew.RoboCup.PerceptorParsing
{
    public interface IBuffer
    {
        /// <summary>
        /// Returns the next <c>char</c> value within the stream, and advances the position by one.
        /// If no further characters exist, returns <see cref="Buffer.EOF"/>.
        /// </summary>
        int Read();
        /// <summary>
        /// Returns the next <c>char</c> value within the stream without advancing the position.
        /// If no further characters exist, returns <see cref="Buffer.EOF"/>.
        /// </summary>
        int Peek();
    	int Pos { get; set; }
//    	string GetString(int beg, int end);
    }
    
    internal static class Buffer
    {
    	public const int EOF = char.MaxValue + 1;
    }
    
    public sealed class StringBuffer : IBuffer
    {
        public StringBuffer(string str) {
            String = str;
            Pos = 0;
        }
        public int Read() {
            if (Pos == String.Length)
                return Buffer.EOF;
            return String[Pos++];
        }
        public int Peek() {
            return String[Pos];
        }
//    	public string GetString(int beg, int end) {
//    	    return String.Substring(beg, end - beg);
//    	}
    	public int Pos { get; set; }
    	public string String { get; private set; }
    }
    
    public sealed class SeekableStreamBuffer : IBuffer
    {
        private readonly Stream _stream;
        public SeekableStreamBuffer(Stream stream) {
            if (stream==null)
                throw new ArgumentNullException("stream");
            if (!stream.CanSeek)
                throw new ArgumentException("Stream must be seekable.");
            _stream = stream;
        }
		public int Pos {
			get { return _stream.Position; }
			set { _stream.Position = value; }
		}
    	
		public int Read()
		{
		    var b = _stream.ReadByte();
		    if (b==-1)
		        return Buffer.EOF;
		    return b;
		}
    	
		public int Peek()
		{
		    var pos = _stream.Position;
	        var b = Read();
	        _stream.Position = pos;
	        return b;
		}
    	
//		public string GetString(int beg, int end)
//		{
//			var pos = _stream.Position;
//			_stream.Position = beg;
//			var len = end - beg;
//			var bytes = new byte[len];
//			_stream.Read(bytes, 0, len);
//			_stream.Position = pos;
//			// might not be ASCII... UTF8?
//			return System.Text.Encoding.ASCII.GetString(bytes);
//		}
    }
    
    public sealed class StreamBuffer : IBuffer
    {
    	// This Buffer supports the following cases:
    	// 1) seekable stream (file)
    	//    a) whole stream in buffer
    	//    b) part of stream in buffer
    	// 2) non seekable stream (network, console)
    
    	/// <summary>input stream</summary>
    	private readonly Stream _stream;
    	/// <summary>input buffer</summary>
    	private byte[] _buffer;
    	/// <summary>position of first byte in buffer relative to input stream</summary>
    	private int _bufStart;
    	/// <summary>length of buffer</summary>
    	private int _bufLen;
    	/// <summary>length of input stream (may change if the stream is no file)</summary>
    	private int _fileLen;
    	/// <summary>current position in buffer</summary>
    	private int _bufPos;
    	
    	public StreamBuffer(Stream stream) : this(stream, -1) {}
    	
    	// Use this constructor when you know the exact length of bytes that will be read
    	public StreamBuffer(Stream stream, int length) {
    	    if (stream==null)
    	        throw new ArgumentNullException("s");
    	    if (stream.CanSeek)
    	        throw new ArgumentException("Stream should not be seekable.  Use SeekableStreamBuffer instead.");
    		_stream = stream;
    		
    		if (length > 0) {
    			_fileLen = length;
    			_bufLen = length;
    			_bufStart = Int32.MaxValue; // nothing in the buffer so far
    		    _buffer = new byte[_bufLen];
    		    Pos = 0; // setup buffer to position 0 (start)
    		} else {
    			_fileLen = _bufLen = _bufStart = 0;
    		    _buffer = new byte[1024]; // start buffer at 1 kB
    		    _bufPos = 0; // index 0 is already after the file, thus Pos = 0 is invalid
    		}
    	}

    	public int Read () {
    		if (_bufPos < _bufLen) {
    			return _buffer[_bufPos++];
    		} else if (Pos < _fileLen) {
    			Pos = Pos; // shift buffer start to Pos
    			return _buffer[_bufPos++];
    		} else if (ReadNextStreamChunk() > 0) {
    			return _buffer[_bufPos++];
    		} else {
    			return Buffer.EOF;
    		}
    	}
    
    	public int Peek() {
    		int curPos = Pos;
    		int ch = Read();
    		Pos = curPos;
    		return ch;
    	}

//    	public string GetString(int beg, int end) {
//    		var buf = new char[end - beg];
//    		int oldPos = Pos;
//    		Pos = beg;
//    		int len = 0;
//    		while (Pos < end)
//    		    buf[len++] = (char)Read();
//    		Pos = oldPos;
//    		return new String(buf, 0, len);
//    	}

    	public int Pos {
    		get { return _bufPos + _bufStart; }
    		set {
    			if (value >= _fileLen && !_stream.CanSeek) {
    				// Wanted position is after buffer and the stream
    				// is not seek-able e.g. network or console,
    				// thus we have to read the stream manually till
    				// the wanted position is in sight.
    				while (value >= _fileLen && ReadNextStreamChunk() > 0);
    			}
    
    			if (value < 0 || value > _fileLen) {
    				throw new FatalError("buffer out of bounds access, position: " + value);
    			}
    
    			if (value >= _bufStart && value < _bufStart + _bufLen) { // already in buffer
    				_bufPos = value - _bufStart;
			    } else if (_stream.CanSeek) { // must be swapped in
    				_stream.Seek(value, SeekOrigin.Begin);
    				_bufLen = _stream.Read(_buffer, 0, _buffer.Length);
    				_bufStart = value; 
    				_bufPos = 0;
    			} else {
    				// set the position to the end of the file, Pos will return fileLen.
    				_bufPos = _fileLen - _bufStart;
    			}
    		}
    	}
    	
    	// Read the next chunk of bytes from the stream, increases the buffer
    	// if needed and updates the fields fileLen and bufLen.
    	// Returns the number of bytes read.
    	private int ReadNextStreamChunk() {
    		int free = _buffer.Length - _bufLen;
    		if (free == 0) {
    			// in the case of a growing input stream
    			// we can neither seek in the stream, nor can we
    			// foresee the maximum length, thus we must adapt
    			// the buffer size on demand.
    			byte[] newBuf = new byte[_bufLen * 2];
    			Array.Copy(_buffer, newBuf, _bufLen);
    			_buffer = newBuf;
    			free = _bufLen;
    		}
    		int read = _stream.Read(_buffer, _bufLen, free);
    		if (read > 0) {
    			_fileLen = _bufLen = (_bufLen + read);
    			return read;
    		}
    		// end of stream reached
    		return 0;
    	}
    }
    
    /*
    public class UTF8Buffer: Buffer {
    	public UTF8Buffer(Buffer b): base(b) {}
    
    	public override int Read() {
    		int ch;
    		do {
    			ch = base.Read();
    			// until we find a utf8 start (0xxxxxxx or 11xxxxxx)
    		} while ((ch >= 128) && ((ch & 0xC0) != 0xC0) && (ch != Buffer.EOF));
    		if (ch < 128 || ch == Buffer.EOF) {
    			// nothing to do, first 127 chars are the same in ascii and utf8
    			// 0xxxxxxx or end of file character
    		} else if ((ch & 0xF0) == 0xF0) {
    			// 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
    			int c1 = ch & 0x07; ch = base.Read();
    			int c2 = ch & 0x3F; ch = base.Read();
    			int c3 = ch & 0x3F; ch = base.Read();
    			int c4 = ch & 0x3F;
    			ch = (((((c1 << 6) | c2) << 6) | c3) << 6) | c4;
    		} else if ((ch & 0xE0) == 0xE0) {
    			// 1110xxxx 10xxxxxx 10xxxxxx
    			int c1 = ch & 0x0F; ch = base.Read();
    			int c2 = ch & 0x3F; ch = base.Read();
    			int c3 = ch & 0x3F;
    			ch = (((c1 << 6) | c2) << 6) | c3;
    		} else if ((ch & 0xC0) == 0xC0) {
    			// 110xxxxx 10xxxxxx
    			int c1 = ch & 0x1F; ch = base.Read();
    			int c2 = ch & 0x3F;
    			ch = (c1 << 6) | c2;
    		}
    		return ch;
    	}
    }
    */
}