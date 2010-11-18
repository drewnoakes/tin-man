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

using System.IO;
using System.Text;
using NUnit.Framework;

namespace TinMan
{
    [TestFixture]
    public sealed class SExpressionReaderTest {
        // TODO test for timeout, avoiding infinite loop
        [Test] public void Take() {
            var s = "((time 600.003))(RDS 0 1)((nd(nd))(nd(nd))(nd(nd))(nd(nd))(nd(nd(nd))(nd)(nd)(nd)(nd)(nd))(nd(nd(nd))(nd)(nd)(nd)(nd)(nd))(nd(nd))(nd(nd))(nd(nd))(nd(nd))(nd)(nd)(nd)(nd)(nd (SLT -0.665576 0.371382 -0.647367 0 -0.653323 -0.709255 0.264814 0 -0.360801 0.599193 0.714696 0 7.11823 0.881769 0.0402713 1)(nd)))";
            var reader = CreateReader(s);
            Assert.AreEqual("((time 600.003))", reader.Take());
            Assert.AreEqual("(RDS 0 1)", reader.Take());
            Assert.AreEqual("((nd(nd))(nd(nd))(nd(nd))(nd(nd))(nd(nd(nd))(nd)(nd)(nd)(nd)(nd))(nd(nd(nd))(nd)(nd)(nd)(nd)(nd))(nd(nd))(nd(nd))(nd(nd))(nd(nd))(nd)(nd)(nd)(nd)(nd (SLT -0.665576 0.371382 -0.647367 0 -0.653323 -0.709255 0.264814 0 -0.360801 0.599193 0.714696 0 7.11823 0.881769 0.0402713 1)(nd)))", 
                            reader.Take());
            
            reader = CreateReader("a b c");
            Assert.AreEqual(0, _stream.Position);
            Assert.AreEqual("a", reader.Take());
            Assert.AreEqual(2, _stream.Position);
            Assert.AreEqual("b", reader.Take());
            Assert.AreEqual(4, _stream.Position);
            Assert.AreEqual("c", reader.Take());
            
            reader = CreateReader("(a b c)(d e f)");
            Assert.IsTrue(reader.In(1));
            Assert.AreEqual("a", reader.Take());
            Assert.AreEqual("b", reader.Take());
            Assert.AreEqual("c", reader.Take());
        }
        
        [Test] public void Skip() {
            var reader = CreateReader("(0)(1)(2) (3) (4) (5) ");
            Assert.AreEqual(0, _stream.Position);
            Assert.IsTrue(reader.Skip(2));
            Assert.AreEqual(6, _stream.Position);
            Assert.AreEqual("(2)", reader.Take());
            Assert.IsTrue(reader.Skip(2));
            Assert.AreEqual("(5)", reader.Take());

            reader = CreateReader("a b(c1 c2)d e");
            Assert.IsTrue(reader.Skip(4));
            Assert.AreEqual("e", reader.Take());
            
            reader = CreateReader("a b  c  (d) e");
            Assert.AreEqual(0, _stream.Position);
            Assert.IsTrue(reader.Skip(2));
            Assert.AreEqual("c", reader.Take());
            Assert.IsTrue(reader.Skip(1));
            Assert.AreEqual("e", reader.Take());
            
            reader = CreateReader("a b c) d e");
            Assert.IsFalse(reader.Skip(4));
            Assert.AreEqual(6, _stream.Position);
        }

        [Test] public void In() {
            var s = "((((1))))";
            var reader = CreateReader(s);
            Assert.IsTrue(reader.In(2));
            Assert.AreEqual("((1))", reader.Take());
        }

        [Test] public void Out() {
            var reader = CreateReader("(((1))(2))");
            Assert.IsTrue(reader.In(3));
            Assert.AreEqual("1", reader.Take());
            Assert.IsTrue(reader.Out(2));
            Assert.AreEqual("(2)", reader.Take());
            
            Assert.IsFalse(CreateReader("a b c)").Out(2));
            Assert.IsTrue(CreateReader("a b c))").Out(2));
            
            reader = CreateReader("a)(b))c");
            Assert.IsTrue(reader.Out(2));
            Assert.AreEqual("c", reader.Take());
        }

        [Test] public void ParseBallLocation() {
            var s = "((time 600.003))(RDS 0 1)((nd(nd))(nd(nd))(nd(nd))(nd(nd))(nd(nd(nd))(nd)(nd)(nd)(nd)(nd))(nd(nd(nd))(nd)(nd)(nd)(nd)(nd))(nd(nd))(nd(nd))(nd(nd))(nd(nd))(nd)(nd)(nd)(nd)(nd (SLT -0.665576 0.371382 -0.647367 0 -0.653323 -0.709255 0.264814 0 -0.360801 0.599193 0.714696 0 7.11823 0.881769 0.0402713 1)(nd)))";
            var reader = CreateReader(s);
            Assert.IsTrue(reader.Skip(2));
            Assert.AreEqual(25, _stream.Position);
            Assert.IsTrue(reader.In(1));
            Assert.IsTrue(reader.Skip(14));
            Assert.IsTrue(reader.In(1));
            Assert.IsTrue(reader.Skip(1));
            Assert.AreEqual("(SLT -0.665576 0.371382 -0.647367 0 -0.653323 -0.709255 0.264814 0 -0.360801 0.599193 0.714696 0 7.11823 0.881769 0.0402713 1)",
                            reader.Take());
        }
        
        [Test] public void SkipToEnd() {
            // length is 10
            var reader = CreateReader("(1)(a b c)");
            reader.Skip(1);
            Assert.AreEqual(3, _stream.Position);
            reader.SkipToEnd();
            Assert.AreEqual(10, _stream.Position);
            reader.SkipToEnd();
            Assert.AreEqual(10, _stream.Position);
        }
        
        [Test] public void FromBallToAgent() {
            var sexp = CreateReader(")(nd StaticMesh (load models/soccerball.obj ) (sSc 0.042 0.042 0.042)(resetMaterials soccerball_rcs-soccerball.png)))(nd TRF (SLT 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1)(blah");
            Assert.IsTrue(sexp.Out(2));
            Assert.IsTrue(sexp.In(2));
            Assert.AreEqual("SLT", sexp.Take());
        }
        
        private MemoryStream _stream;
        private SExpressionReader CreateReader(string s) {
            _stream = new MemoryStream();
            var bytes = Encoding.ASCII.GetBytes(s);
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Position = 0;
            return new SExpressionReader(_stream, s.Length);
        }
    }
}
