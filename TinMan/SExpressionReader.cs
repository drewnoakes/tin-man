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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace TinMan
{
    public sealed class SExpressionReader
    {
        private readonly Stream _stream;
        private int _bytesRead; // { get; set; }
        public int Length { get; private set; }

        public SExpressionReader(Stream stream, int length)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length", length, "Must be greater than zero.");
            _stream = stream;
            Length = length;
        }

        #region Reading and Pushing

        private int _pushedChar = -1;

        private bool TryReadChar(out char c)
        {
            if (_pushedChar != -1)
            {
                c = (char)_pushedChar;
                _pushedChar = -1;
                return true;
            }

            if (_bytesRead == Length)
            {
                c = char.MinValue;
                return false;
            }

            int i;
            while ((i = _stream.ReadByte()) == -1)
            {
                Thread.Sleep(1);
            }
            _bytesRead++;
            c = (char)i;
            return true;
        }

        private void Push(char c)
        {
            if (_pushedChar != -1)
                throw new InvalidOperationException("A pushed character already exists.");
            _pushedChar = c;
        }

        #endregion

        private void SkipWhitespace()
        {
            while (true)
            {
                char c;
                if (!TryReadChar(out c))
                    return;

                if (!char.IsWhiteSpace(c))
                {
                    Push(c);
                    return;
                }
            }
        }

        private enum State
        {
            None,
            Symbol,
            Sublist
        }

        public bool Skip(int count)
        {
            int openBraceCount = 0;
            State state = State.None;
            while (true)
            {
                char c;
                if (!TryReadChar(out c))
                    return false;
                if (c == '(')
                {
                    switch (state)
                    {
                        case State.None:
                            state = State.Sublist;
                            openBraceCount++;
                            break;
                        case State.Symbol:
                            count--;
                            if (count == 0)
                            {
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
                }
                else if (c == ')')
                {
                    switch (state)
                    {
                        case State.None:
                            Push(c);
                            return false;
                        case State.Symbol:
                            count--;
                            if (count == 0)
                            {
                                Push(c);
                                return true;
                            }
                            Push(c);
                            return false;
                        case State.Sublist:
                            Debug.Assert(openBraceCount > 0);
                            openBraceCount--;
                            if (openBraceCount == 0)
                            {
                                count--;
                                if (count == 0)
                                    return true;
                                state = State.None;
                            }
                            break;
                    }
                }
                else if (char.IsWhiteSpace(c))
                {
                    switch (state)
                    {
                        case State.None:
                            break;
                        case State.Symbol:
                            count--;
                            if (count == 0)
                                return true;
                            state = State.None;
                            break;
                        case State.Sublist:
                            break;
                    }
                }
                else
                {
                    // regular character...
                    switch (state)
                    {
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

        public bool In(int levelCount)
        {
            while (levelCount > 0)
            {
                char c;
                if (!TryReadChar(out c))
                    return false;
                if (c == '(')
                    levelCount--;
            }
            return true;
        }

        public bool Out(int levelCount)
        {
            while (levelCount > 0)
            {
                char c;
                if (!TryReadChar(out c))
                    return false;
                if (c == ')')
                    levelCount--;
                else if (c == '(')
                    levelCount++;
            }
            return true;
        }

        public string Take()
        {
            int openBraceCount = 0;

            SkipWhitespace();

            var sb = new StringBuilder();
            while (true)
            {
                char c;
                if (!TryReadChar(out c))
                    return sb.ToString();
                if (c == ')')
                {
                    if (openBraceCount == 0)
                    {
                        Push(c);
                        return sb.ToString();
                    }
                    openBraceCount--;
                    if (openBraceCount == 0)
                    {
                        sb.Append(c);
                        return sb.ToString();
                    }
                }
                else if (c == '(')
                {
                    openBraceCount++;
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (openBraceCount == 0)
                        return sb.ToString();
                }
                sb.Append(c);
            }
        }

        private readonly byte[] _rubbish = new byte[1024];

        public void SkipToEnd()
        {
            while (_bytesRead < Length)
            {
                int diff = Math.Min(_rubbish.Length, Length - _bytesRead);
                int read = _stream.Read(_rubbish, 0, diff);
                _bytesRead += read;
            }
            Debug.Assert(_bytesRead == Length);
        }
    }
}