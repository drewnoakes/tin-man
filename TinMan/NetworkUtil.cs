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
// Created 06/05/2010 14:07

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using TinMan.PerceptorParsing;

namespace TinMan
{
	internal static class NetworkUtil {
        private static readonly Log _log = Log.Create();
	    public static void WriteStringWith32BitLengthPrefix(NetworkStream stream, string str) {
	        // Server uses 1-byte-per-character ASCII
	        byte[] bytes = Encoding.ASCII.GetBytes(str);
	        
	        // Prefix with the length of the message
	        WriteInt32(stream, (uint)bytes.Length);
	        
	        stream.Write(bytes, 0, bytes.Length);
	    }
	    public static void WriteInt32(NetworkStream stream, uint num) {
	        // Big endian - MSB first
	        var numBytes = new byte[4];
	        numBytes[3] = (byte)(num & 0xff);
	        numBytes[2] = (byte)(num >> 8 & 0xff);
	        numBytes[1] = (byte)(num >> 16 & 0xff);
	        numBytes[0] = (byte)(num >> 24 & 0xff);
	        stream.Write(numBytes, 0, numBytes.Length);
	    }
	    
	    public static int ReadInt32(NetworkStream stream) {
	        var lengthBytes = new byte[4];
	        stream.Read(lengthBytes, 0, 4);
	        int length = (lengthBytes[0] << 24) | (lengthBytes[1] << 16) | (lengthBytes[2] << 8) | (lengthBytes[3]);
	        return length;
	    }

        public static string ReadResponseString(NetworkStream stream, TimeSpan timeout) {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (!stream.DataAvailable) {
                if (stopwatch.Elapsed > timeout) {
                    _log.Warn("No response received within time limit.");
                    return null;
                }
                System.Threading.Thread.Sleep(5);
            }
            
            int length = NetworkUtil.ReadInt32(stream);
            var bytes = new byte[length];
            
            int totalBytesRead = 0;
            do {
                totalBytesRead += stream.Read(bytes, totalBytesRead, length - totalBytesRead);
            } while (totalBytesRead < length);
            
            return Encoding.ASCII.GetString(bytes);
        }
	}
}
