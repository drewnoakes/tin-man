
using System;
using System.IO;
using System.Collections.Generic;

namespace TinMan.PerceptorParsing {

    public sealed class Token
    {
        /// <summary>token kind</summary>
    	public int kind { get; set; }
    	/// <summary>token position in the source text (starting at 0)</summary>
    	public int pos { get; set; }     
    	/// <summary>token column (starting at 1)</summary>
    	public int col { get; set; }     
    	/// <summary>token line (starting at 1)</summary>
    	public int line { get; set; }    
    	/// <summary>token value</summary>
    	public string val { get; set; }  
    	/// <summary>ML 2005-03-11 Tokens are kept in linked list</summary>
    	public Token next { get; set; }  
    }
    
    public interface IBuffer
    {
        int Read ();
        int Peek ();
//    	string GetString (int beg, int end);
    	int Pos { get; set; }
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
        public int Read () {
            if (Pos == String.Length)
                return Buffer.EOF;
            return String[Pos++];
        }
        public int Peek () {
            return String[Pos];
        }
//    	public string GetString (int beg, int end) {
//    	    return String.Substring(beg, end - beg);
//    	}
    	public int Pos { get; set; }
    	public string String { get; private set; }
    }
    
    public sealed class StreamBuffer : IBuffer
    {
    	// This Buffer supports the following cases:
    	// 1) seekable stream (file)
    	//    a) whole stream in buffer
    	//    b) part of stream in buffer
    	// 2) non seekable stream (network, console)
    
    	const int MIN_BUFFER_LENGTH = 1024; // 1KB
    	const int MAX_BUFFER_LENGTH = MIN_BUFFER_LENGTH * 64; // 64KB
    	byte[] buf;         // input buffer
    	int bufStart;       // position of first byte in buffer relative to input stream
    	int bufLen;         // length of buffer
    	int fileLen;        // length of input stream (may change if the stream is no file)
    	int bufPos;         // current position in buffer
    	Stream stream;      // input stream (seekable)
    	
    	public StreamBuffer(Stream s) {
    		stream = s;
    		
    		if (stream.CanSeek) {
    			fileLen = (int) stream.Length;
    			bufLen = Math.Min(fileLen, MAX_BUFFER_LENGTH);
    			bufStart = Int32.MaxValue; // nothing in the buffer so far
    		} else {
    			fileLen = bufLen = bufStart = 0;
    		}
    
    		buf = new byte[(bufLen>0) ? bufLen : MIN_BUFFER_LENGTH];
    		if (fileLen > 0) 
    		    Pos = 0; // setup buffer to position 0 (start)
    		else 
    		    bufPos = 0; // index 0 is already after the file, thus Pos = 0 is invalid
    	}
    	
    //	protected Buffer(Buffer b) { // called in UTF8Buffer constructor
    //		buf = b.buf;
    //		bufStart = b.bufStart;
    //		bufLen = b.bufLen;
    //		fileLen = b.fileLen;
    //		bufPos = b.bufPos;
    //		stream = b.stream;
    //		// keep destructor from closing the stream
    //		b.stream = null;
    //	}
    
    	public int Read () {
    		if (bufPos < bufLen) {
    			return buf[bufPos++];
    		} else if (Pos < fileLen) {
    			Pos = Pos; // shift buffer start to Pos
    			return buf[bufPos++];
    		} else if (stream != null && !stream.CanSeek && ReadNextStreamChunk() > 0) {
    			return buf[bufPos++];
    		} else {
    			return Buffer.EOF;
    		}
    	}
    
    	public int Peek () {
    		int curPos = Pos;
    		int ch = Read();
    		Pos = curPos;
    		return ch;
    	}
    	
//    	public string GetString (int beg, int end) {
//    		int len = 0;
//    		char[] buf = new char[end - beg];
//    		int oldPos = Pos;
//    		Pos = beg;
//    		while (Pos < end) buf[len++] = (char) Read();
//    		Pos = oldPos;
//    		return new String(buf, 0, len);
//    	}
    
    	public int Pos {
    		get { return bufPos + bufStart; }
    		set {
    			if (value >= fileLen && stream != null && !stream.CanSeek) {
    				// Wanted position is after buffer and the stream
    				// is not seek-able e.g. network or console,
    				// thus we have to read the stream manually till
    				// the wanted position is in sight.
    				while (value >= fileLen && ReadNextStreamChunk() > 0);
    			}
    
    			if (value < 0 || value > fileLen) {
    				throw new FatalError("buffer out of bounds access, position: " + value);
    			}
    
    			if (value >= bufStart && value < bufStart + bufLen) { // already in buffer
    				bufPos = value - bufStart;
    			} else if (stream != null) { // must be swapped in
    				stream.Seek(value, SeekOrigin.Begin);
    				bufLen = stream.Read(buf, 0, buf.Length);
    				bufStart = value; bufPos = 0;
    			} else {
    				// set the position to the end of the file, Pos will return fileLen.
    				bufPos = fileLen - bufStart;
    			}
    		}
    	}
    	
    	// Read the next chunk of bytes from the stream, increases the buffer
    	// if needed and updates the fields fileLen and bufLen.
    	// Returns the number of bytes read.
    	private int ReadNextStreamChunk() {
    		int free = buf.Length - bufLen;
    		if (free == 0) {
    			// in the case of a growing input stream
    			// we can neither seek in the stream, nor can we
    			// foresee the maximum length, thus we must adapt
    			// the buffer size on demand.
    			byte[] newBuf = new byte[bufLen * 2];
    			Array.Copy(buf, newBuf, bufLen);
    			buf = newBuf;
    			free = bufLen;
    		}
    		int read = stream.Read(buf, bufLen, free);
    		if (read > 0) {
    			fileLen = bufLen = (bufLen + read);
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
    
    public sealed class Scanner
    {
    	const char EOL = '\n';
    	const int eofSym = 0; /* pdt */
	const int maxT = 48;
	const int noSym = 48;

    
    	public IBuffer buffer; // scanner buffer
    	
    	Token t;          // current token
    	int ch;           // current input character
    	int pos;          // byte position of current character
    	int col;          // column number of current character
    	int line;         // line number of current character
    	int oldEols;      // EOLs that appeared in a comment;
    	static readonly Dictionary<int,int> start; // maps first token character to start state
    
    	Token tokens;     // list of tokens already peeked (first token is a dummy)
    	Token pt;         // current peek token
    	
    	char[] tval = new char[128]; // text of current token
    	int tlen;         // length of current token
    	
    	static Scanner() {
    		start = new Dictionary<int,int>();
		for (int i = 48; i <= 57; ++i) start[i] = 2;
		for (int i = 65; i <= 90; ++i) start[i] = 5;
		for (int i = 97; i <= 122; ++i) start[i] = 5;
		start[45] = 1; 
		start[39] = 6; 
		start[40] = 58; 
		start[41] = 14; 
		start[Buffer.EOF] = -1;

    	}
    		
    	public Scanner (Stream s) {
    		buffer = new StreamBuffer(s);
    		Init();
    	}
    	
    	public Scanner (string s) {
    		buffer = new StringBuffer(s);
    		Init();
    	}
    	
    	public Scanner (IBuffer b) {
    		buffer = b;
    		Init();
    	}
    	
    	private void Init() {
    		pos = -1; line = 1; col = 0;
    		oldEols = 0;
    		NextCh();
/*
    		if (ch == 0xEF) { // check optional byte order mark for UTF-8
    			NextCh(); int ch1 = ch;
    			NextCh(); int ch2 = ch;
    			if (ch1 != 0xBB || ch2 != 0xBF) {
    				throw new FatalError(String.Format("illegal byte order mark: EF {0,2:X} {1,2:X}", ch1, ch2));
    			}
    			buffer = new UTF8Buffer(buffer); col = 0;
    			NextCh();
    		}
*/
    		pt = tokens = new Token();  // first token is a dummy
    	}
    	
    	private void NextCh() {
    		if (oldEols > 0) { ch = EOL; oldEols--; } 
    		else {
    			pos = buffer.Pos;
    			ch = buffer.Read(); col++;
    			// replace isolated '\r' by '\n' in order to make
    			// eol handling uniform across Windows, Unix and Mac
    			if (ch == '\r' && buffer.Peek() != '\n') ch = EOL;
    			if (ch == EOL) { line++; col = 0; }
    		}

    	}
    
    	private void AddCh() {
    		if (tlen >= tval.Length) {
    			char[] newBuf = new char[2 * tval.Length];
    			Array.Copy(tval, 0, newBuf, 0, tval.Length);
    			tval = newBuf;
    		}
    		if (ch != Buffer.EOF) {
			tval[tlen++] = (char) ch;
    			NextCh();
    		}
    	}
    



	    private void CheckLiteral() {
		switch (t.val) {
			case "now": t.kind = 8; break;
			case "left": t.kind = 12; break;
			case "right": t.kind = 13; break;
			case "t": t.kind = 14; break;
			case "pm": t.kind = 15; break;
			case "n": t.kind = 17; break;
			case "rt": t.kind = 18; break;
			case "a": t.kind = 20; break;
			case "ax": t.kind = 22; break;
			case "ax1": t.kind = 24; break;
			case "ax2": t.kind = 25; break;
			case "val": t.kind = 27; break;
			case "c": t.kind = 29; break;
			case "f": t.kind = 30; break;
			case "temp": t.kind = 32; break;
			case "battery": t.kind = 33; break;
			case "F1L": t.kind = 35; break;
			case "F2L": t.kind = 36; break;
			case "F1R": t.kind = 37; break;
			case "F2R": t.kind = 38; break;
			case "G1L": t.kind = 39; break;
			case "G2L": t.kind = 40; break;
			case "G1R": t.kind = 41; break;
			case "G2R": t.kind = 42; break;
			case "B": t.kind = 43; break;
			case "P": t.kind = 44; break;
			case "self": t.kind = 47; break;
			default: break;
		}
    	}
    
    	private Token NextToken() {
    		while (ch == ' ' ||
			false
		    ) NextCh();

    		int recKind = noSym;
    		int recEnd = pos;
    		t = new Token();
    		t.pos = pos; t.col = col; t.line = line; 
    		int state;
    		if (!start.TryGetValue(ch, out state))
    		    state = 0;
    		tlen = 0; AddCh();
    		
    		switch (state) {
    			case -1: { t.kind = eofSym; break; } // NextCh already done
    			case 0: {
    				if (recKind != noSym) {
    					tlen = recEnd - t.pos;
    					SetScannerBehindT();
    				}
    				t.kind = recKind; break;
    			} // NextCh already done
			case 1:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 2;}
				else {goto case 0;}
			case 2:
				recEnd = pos; recKind = 1;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 2;}
				else if (ch == '.') {AddCh(); goto case 3;}
				else {t.kind = 1; break;}
			case 3:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 4;}
				else {goto case 0;}
			case 4:
				recEnd = pos; recKind = 1;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 4;}
				else {t.kind = 1; break;}
			case 5:
				recEnd = pos; recKind = 2;
				if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 5;}
				else {t.kind = 2; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 6:
				if (ch == 39) {AddCh(); goto case 7;}
				else {goto case 0;}
			case 7:
				if (ch >= '!' && ch <= 39 || ch >= '*' && ch <= '~') {AddCh(); goto case 8;}
				else {goto case 0;}
			case 8:
				if (ch >= '!' && ch <= '&' || ch >= '*' && ch <= '~') {AddCh(); goto case 8;}
				else if (ch == 39) {AddCh(); goto case 9;}
				else {goto case 0;}
			case 9:
				if (ch >= '!' && ch <= '&' || ch >= '*' && ch <= '~') {AddCh(); goto case 8;}
				else if (ch == 39) {AddCh(); goto case 10;}
				else {goto case 0;}
			case 10:
				recEnd = pos; recKind = 3;
				if (ch >= '!' && ch <= '&' || ch >= '*' && ch <= '~') {AddCh(); goto case 8;}
				else if (ch == 39) {AddCh(); goto case 10;}
				else {t.kind = 3; break;}
			case 11:
				if (ch == 'o') {AddCh(); goto case 12;}
				else {goto case 0;}
			case 12:
				if (ch == 'l') {AddCh(); goto case 13;}
				else {goto case 0;}
			case 13:
				{t.kind = 4; break;}
			case 14:
				{t.kind = 5; break;}
			case 15:
				if (ch == 'm') {AddCh(); goto case 16;}
				else {goto case 0;}
			case 16:
				if (ch == 'e') {AddCh(); goto case 17;}
				else {goto case 0;}
			case 17:
				{t.kind = 6; break;}
			case 18:
				{t.kind = 9; break;}
			case 19:
				if (ch == 'n') {AddCh(); goto case 20;}
				else {goto case 0;}
			case 20:
				if (ch == 'u') {AddCh(); goto case 21;}
				else {goto case 0;}
			case 21:
				if (ch == 'm') {AddCh(); goto case 22;}
				else {goto case 0;}
			case 22:
				{t.kind = 10; break;}
			case 23:
				if (ch == 'a') {AddCh(); goto case 24;}
				else {goto case 0;}
			case 24:
				if (ch == 'm') {AddCh(); goto case 25;}
				else {goto case 0;}
			case 25:
				{t.kind = 11; break;}
			case 26:
				if (ch == 'R') {AddCh(); goto case 27;}
				else {goto case 0;}
			case 27:
				{t.kind = 16; break;}
			case 28:
				if (ch == 'C') {AddCh(); goto case 29;}
				else {goto case 0;}
			case 29:
				{t.kind = 19; break;}
			case 30:
				if (ch == 'J') {AddCh(); goto case 31;}
				else {goto case 0;}
			case 31:
				{t.kind = 21; break;}
			case 32:
				if (ch == 'J') {AddCh(); goto case 33;}
				else {goto case 0;}
			case 33:
				{t.kind = 23; break;}
			case 34:
				if (ch == 'C') {AddCh(); goto case 35;}
				else {goto case 0;}
			case 35:
				if (ch == 'H') {AddCh(); goto case 36;}
				else {goto case 0;}
			case 36:
				{t.kind = 26; break;}
			case 37:
				if (ch == 'R') {AddCh(); goto case 38;}
				else {goto case 0;}
			case 38:
				if (ch == 'P') {AddCh(); goto case 39;}
				else {goto case 0;}
			case 39:
				{t.kind = 28; break;}
			case 40:
				if (ch == 'e') {AddCh(); goto case 41;}
				else {goto case 0;}
			case 41:
				if (ch == 'n') {AddCh(); goto case 42;}
				else {goto case 0;}
			case 42:
				if (ch == 't') {AddCh(); goto case 43;}
				else {goto case 0;}
			case 43:
				if (ch == 'S') {AddCh(); goto case 44;}
				else {goto case 0;}
			case 44:
				if (ch == 't') {AddCh(); goto case 45;}
				else {goto case 0;}
			case 45:
				if (ch == 'a') {AddCh(); goto case 46;}
				else {goto case 0;}
			case 46:
				if (ch == 't') {AddCh(); goto case 47;}
				else {goto case 0;}
			case 47:
				if (ch == 'e') {AddCh(); goto case 48;}
				else {goto case 0;}
			case 48:
				{t.kind = 31; break;}
			case 49:
				if (ch == 'e') {AddCh(); goto case 50;}
				else {goto case 0;}
			case 50:
				if (ch == 'e') {AddCh(); goto case 51;}
				else {goto case 0;}
			case 51:
				{t.kind = 34; break;}
			case 52:
				if (ch == 'd') {AddCh(); goto case 53;}
				else {goto case 0;}
			case 53:
				{t.kind = 45; break;}
			case 54:
				if (ch == 'e') {AddCh(); goto case 55;}
				else {goto case 0;}
			case 55:
				if (ch == 'a') {AddCh(); goto case 56;}
				else {goto case 0;}
			case 56:
				if (ch == 'r') {AddCh(); goto case 57;}
				else {goto case 0;}
			case 57:
				{t.kind = 46; break;}
			case 58:
				recEnd = pos; recKind = 7;
				if (ch == 'p') {AddCh(); goto case 11;}
				else if (ch == 't') {AddCh(); goto case 59;}
				else if (ch == 'G') {AddCh(); goto case 60;}
				else if (ch == 'u') {AddCh(); goto case 19;}
				else if (ch == 'A') {AddCh(); goto case 61;}
				else if (ch == 'H') {AddCh(); goto case 30;}
				else if (ch == 'U') {AddCh(); goto case 32;}
				else if (ch == 'T') {AddCh(); goto case 34;}
				else if (ch == 'F') {AddCh(); goto case 37;}
				else if (ch == 'S') {AddCh(); goto case 49;}
				else if (ch == 'i') {AddCh(); goto case 52;}
				else if (ch == 'h') {AddCh(); goto case 54;}
				else {t.kind = 7; break;}
			case 59:
				if (ch == 'i') {AddCh(); goto case 15;}
				else if (ch == 'e') {AddCh(); goto case 23;}
				else {goto case 0;}
			case 60:
				if (ch == 'S') {AddCh(); goto case 18;}
				else if (ch == 'Y') {AddCh(); goto case 26;}
				else {goto case 0;}
			case 61:
				if (ch == 'C') {AddCh(); goto case 28;}
				else if (ch == 'g') {AddCh(); goto case 40;}
				else {goto case 0;}

    		}
    		t.val = new String(tval, 0, tlen);
    		return t;
    	}
    	
    	private void SetScannerBehindT() {
    		buffer.Pos = t.pos;
    		NextCh();
    		line = t.line; col = t.col;
    		for (int i = 0; i < tlen; i++) NextCh();
    	}
    	
    	// get the next token (possibly a token already seen during peeking)
    	public Token Scan () {
    		if (tokens.next == null) {
    			return NextToken();
    		} else {
    			pt = tokens = tokens.next;
    			return tokens;
    		}
    	}
    
    	// peek for the next token, ignore pragmas
    	public Token Peek () {
    		do {
    			if (pt.next == null) {
    				pt.next = NextToken();
    			}
    			pt = pt.next;
    		} while (pt.kind > maxT); // skip pragmas
    	
    		return pt;
    	}
    
    	// make sure that peeking starts at the current scan position
    	public void ResetPeek () { pt = tokens; }
    
    } // end Scanner

}