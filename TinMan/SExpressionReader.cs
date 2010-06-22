#region License
/* 
 * This file is part of TinMan.
 *
 * TinMan is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TinMan is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with TinMan.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

// Copyright Drew Noakes, http://drewnoakes.com
// Created 22/06/2010 16:10

using System;
using System.IO;

namespace TinMan
{
	public sealed class SExpressionReader {
	    private readonly Stream _stream;
	    public int Offset { get; private set; }
	    public int MaximumOffset { get; private set; }

	    public SExpressionReader(Stream stream, int length) {
	        _stream = stream;
	        MaximumOffset = length - 1;
	        Offset = 0;
	    }

	    #region Reading and Pushing
	    
	    private int _pushedChar = -1;
	    private bool TryReadChar(out char c) {
	        if (_pushedChar!=-1) {
	            c = (char)_pushedChar;
	            _pushedChar = -1;
	            Offset++;
	            return true;
	        }
	        
	        if (Offset>MaximumOffset) {
	            c = char.MinValue;
	            return false;
	        }
	        
	        int i;
	        while ((i = _stream.ReadByte())==-1) {
	            System.Threading.Thread.Sleep(1);
	        }
	        Offset++;
	        c = (char)i;
	        return true;
	    }
	    private void Push(char c) {
	        if (_pushedChar!=-1)
	            throw new InvalidOperationException("A pushed character already exists.");
	        _pushedChar = c;
	        Offset--;
	    }
	    
	    #endregion
	    
	    private void SkipWhitespace() {
	        while (true) {
	            char c;
	            if (!TryReadChar(out c))
	                return;
	            
	            if (!char.IsWhiteSpace(c)) {
	                Push(c);
	                return;
	            }
	        }
	    }
	    enum State { None, Symbol, Sublist }
	    public bool Skip(int count) {
	        int openBraceCount = 0;
	        var state = State.None;
	        while (true) {
	            char c;
	            if (!TryReadChar(out c))
	                return false;
	            if (c=='(') {
	                switch (state) {
	                    case State.None:
	                        state = State.Sublist;
	                        openBraceCount++;
	                        break;
	                    case State.Symbol:
	                        count--;
	                        if (count==0) {
	                            Push(c);
	                            return true;
	                        }
	                        state = State.Sublist;
	                        openBraceCount++;
	                        break;
	                    case State.Sublist:
	                        openBraceCount++;
	                        break;
	                }
	            } else if (c==')') {
	                switch (state) {
	                    case State.None:
	                        Push(c);
	                        return false;
	                    case State.Symbol:
	                        count--;
	                        if (count==0) {
	                            Push(c);
	                            return true;
	                        }
	                        Push(c);
	                        return false;
	                    case State.Sublist:
	                        System.Diagnostics.Debug.Assert(openBraceCount > 0);
	                        openBraceCount--;
	                        if (openBraceCount==0) {
	                            count--;
	                            if (count==0)
	                                return true;
	                            state = State.None;
	                        }
	                        break;
	                }
	            } else if (char.IsWhiteSpace(c)) {
	                switch (state) {
	                    case State.None:
	                        break;
	                    case State.Symbol:
	                        count--;
	                        if (count==0)
	                            return true;
	                        state = State.None;
	                        break;
	                    case State.Sublist:
	                        break;
	                }
	            } else {
	                // regular character...
	                switch (state) {
	                    case State.None:
	                        state = State.Symbol;
	                        break;
	                    case State.Symbol:
	                        break;
	                    case State.Sublist:
	                        break;
	                }
	            }
	        }
	    }
	    public bool In(int levelCount) {
	        while (levelCount>0) {
	            char c;
	            if (!TryReadChar(out c))
	                return false;
	            if (c=='(')
	                levelCount--;
	        }
	        return true;
	    }
	    public bool Out(int levelCount) {
	        while (levelCount>0) {
	            char c;
	            if (!TryReadChar(out c))
	                return false;
	            if (c==')')
	                levelCount--;
	            else if (c=='(')
	                levelCount++;
	        }
	        return true;
	    }
	    public string Take() {
	        int openBraceCount = 0;
	        
	        SkipWhitespace();
	        
	        var sb = new System.Text.StringBuilder();
	        while (true) {
	            char c;
	            if (!TryReadChar(out c))
	                return sb.ToString();
	            if (c==')') {
	                if (openBraceCount==0) {
	                    Push(c);
	                    return sb.ToString();
	                }
	                openBraceCount--;
	                if (openBraceCount==0) {
	                    sb.Append(c);
	                    return sb.ToString();
	                }
	            } else if (c=='(') {
	                openBraceCount++;
	            } else if (char.IsWhiteSpace(c)) {
	                if (openBraceCount==0)
	                    return sb.ToString();
	            }
	            sb.Append(c);
	        }
	    }
	    
	    private readonly byte[] _rubbish = new byte[1024];
	    
	    public void SkipToEnd() {
	        // We're at offset and have to read maxOffset-offset extra bytes
	        while (Offset < MaximumOffset) {
	            int targetCount = Math.Min(_rubbish.Length, MaximumOffset - Offset);
	            int actualCount = _stream.Read(_rubbish, 0, targetCount);
	            Offset += actualCount;
	        }
	        Offset = int.MaxValue;
	    }
	}
}
