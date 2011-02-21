using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Text;

namespace TinMan.RoboViz
{
    /// <summary>
    /// Base class for all RoboViz shapes.
    /// </summary>
    public abstract class Shape
    {
        private bool _isVisible = true;

        protected ShapeSet ShapeSet { get; private set; }

        /// <summary>Gets and sets a value that indicates whether this shape is visible or not.</summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                SetDirty();
            }
        }

        /// <summary>
        /// Removes this shape from its parent ShapeSet, and therefore the scene.  A removed shape may
        /// be re-added to the parent later, or to a different ShapeSet if desired.
        /// </summary>
        public void Remove()
        {
            if (ShapeSet == null)
                throw new InvalidOperationException("This Shape cannot be removed as it is not currently contained within a ShapeSet.");
            ShapeSet.Remove(this);
        }

        public abstract void Translate(Vector3 offset);

        internal abstract void SendMessage(UdpClient udpClient);

        internal void SetShapeSet(ShapeSet shapeSet)
        {
            if (shapeSet == null)
                throw new ArgumentNullException("shapeSet");
            if (ShapeSet != null)
                throw new InvalidOperationException(string.Format("Shape already belongs to ShapeSet '{0}' so cannot be added to '{1}'", ShapeSet.Path, shapeSet.Path));
            ShapeSet = shapeSet;
        }

        internal void ClearSetShapeSet()
        {
            if (ShapeSet == null)
                throw new InvalidOperationException("Shape does not belong to a ShapeSet.");
            ShapeSet = null;
        }

        protected void SetDirty()
        {
            if (ShapeSet != null)
                ShapeSet.IsDirty = true;
        }

        protected static void WriteDouble(byte[] buf, int offset, double d)
        {
            // TODO in some cases we truncate the last character -- we could round the number for better accuracy
            var s = d.ToString("#.00000");
            s = s.Substring(0, Math.Min(s.Length, 6));
            Debug.Assert(s.Length == 6);
            var byteCount = Encoding.ASCII.GetBytes(s, 0, 6, buf, offset);
            Debug.Assert(byteCount == 6);
        }

        protected static void WriteColor(byte[] buf, int offset, Color color, bool includeAlpha)
        {
            buf[offset++] = color.R;
            buf[offset++] = color.G;
            buf[offset++] = color.B;
            if (includeAlpha)
                buf[offset] = color.A;
        }
    }

    /// <summary>
    /// A coloured point in 3D space.  Its size remains the same regardless of its distance from the camera,
    /// hence the size unit is pixels.  Note that `X`,`Y`,`Z` are equivalent to `Position`, so use whichever
    /// is more convenient at the time.
    /// </summary>
    public sealed class Dot : Shape
    {
        private double _x;
        private double _y;
        private double _z;
        private double _pixelSize;
        private Color _color;

        /// <summary>Initialises a 5-pixel wide white <see cref="TinMan.RoboViz.Dot"/>.</summary>
        public Dot()
        {
            _pixelSize = 5;
            _color = Color.White;
        }

        public Dot(Vector3 position, double pixelSize, Color color)
        {
            Position = position;
            _pixelSize = pixelSize;
            _color = color;
        }

        #region Properties

        public double PixelSize
        {
            get { return _pixelSize; }
            set { _pixelSize = value; SetDirty(); }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; SetDirty(); }
        }

        public Vector3 Position
        {
            get { return new Vector3(X, Y, Z); }
            set { _x = value.X; _y = value.Y; _z = value.Z; SetDirty(); }
        }

        public double X
        {
            get { return _x; }
            set { _x = value; SetDirty(); }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; SetDirty(); }
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; SetDirty(); }
        }

        #endregion

        #region Overrides

        public override void Translate(Vector3 offset)
        {
            Position += offset;
        }

        internal override void SendMessage(UdpClient udpClient)
        {
            var pathBytes = ShapeSet.PathBytes;
            var numBytes = 30 + pathBytes.Length;
            var buf = new byte[numBytes];

            buf[0] = 1;
            buf[1] = 2;
            WriteDouble(buf, 2, _x);
            WriteDouble(buf, 8, _y);
            WriteDouble(buf, 14, _z);
            WriteDouble(buf, 20, PixelSize);
            WriteColor(buf, 26, Color, false);
            pathBytes.CopyTo(buf, 29);

            var bytesSentCount = udpClient.Send(buf, buf.Length);
            Debug.Assert(bytesSentCount == numBytes);
        }

        #endregion
    }

    /// <summary>
    /// A coloured line in 3D space which is finite in both directions -- i.e. it has two ends.
    /// Its thickness remains the same regardless of its distance from the camera, hence its thickness unit is pixels.
    /// Note that `X1`,`Y1`,`Z1` are equivalent to `End1` (likewise for `End2`), so use whichever is more convenient at the time.
    /// </summary>
    public sealed class Line : Shape
    {
        private double _x1;
        private double _y1;
        private double _z1;
        private double _x2;
        private double _y2;
        private double _z2;
        private double _pixelThickness;
        private Color _color;

        /// <summary>Initialises a 0-length <see cref="TinMan.RoboViz.Line"/> with both ends at the origin, drawn with a 1-pixel wide white line.</summary>
        public Line()
        {
            _pixelThickness = 1;
            _color = Color.White;
        }

        public Line(Vector3 end1, Vector3 end2, double pixelThickness, Color color)
        {
            End1 = end1;
            End2 = end2;
            PixelThickness = pixelThickness;
            Color = color;
        }

        #region Properties

        public Vector3 End1
        {
            get { return new Vector3(_x1, _y1, _z1); }
            set { _x1 = value.X; _y1 = value.Y; _z1 = value.Z; SetDirty(); }
        }

        public Vector3 End2
        {
            get { return new Vector3(_x2, _y2, _z2); }
            set { _x2 = value.X; _y2 = value.Y; _z2 = value.Z; SetDirty(); }
        }

        public double X1
        {
            get { return _x1; }
            set { _x1 = value; SetDirty(); }
        }

        public double Y1
        {
            get { return _y1; }
            set { _y1 = value; SetDirty(); }
        }

        public double Z1
        {
            get { return _z1; }
            set { _z1 = value; SetDirty(); }
        }

        public double X2
        {
            get { return _x2; }
            set { _x2 = value; SetDirty(); }
        }

        public double Y2
        {
            get { return _y2; }
            set { _y2 = value; SetDirty(); }
        }

        public double Z2
        {
            get { return _z2; }
            set { _z2 = value; SetDirty(); }
        }

        public double PixelThickness
        {
            get { return _pixelThickness; }
            set { _pixelThickness = value; SetDirty(); }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; SetDirty(); }
        }

        #endregion

        #region Overrides

        public override void Translate(Vector3 offset)
        {
            End1 += offset;
            End2 += offset;
        }

        internal override void SendMessage(UdpClient udpClient)
        {
            var pathBytes = ShapeSet.PathBytes;
            var numBytes = 48 + pathBytes.Length;
            var buf = new byte[numBytes];

            buf[0] = 1;
            buf[1] = 1;
            WriteDouble(buf, 2,  _x1);
            WriteDouble(buf, 8,  _y1);
            WriteDouble(buf, 14, _z1);
            WriteDouble(buf, 20, _x2);
            WriteDouble(buf, 26, _y2);
            WriteDouble(buf, 32, _z2);
            WriteDouble(buf, 38, PixelThickness);
            WriteColor(buf, 44, Color, false);
            pathBytes.CopyTo(buf, 47);

            var bytesSentCount = udpClient.Send(buf, buf.Length);
            Debug.Assert(bytesSentCount == numBytes);
        }

        #endregion
    }

    /// <summary>
    /// A borderless, filled polygon in 3D space formed from a list of vertices.
    /// </summary>
    public sealed class Polygon : Shape
    {
        private Color _color;
        private readonly List<Vector3> _vertices;

        /// <summary>Initialises a 0-vertex <see cref="Polygon"/> of white color.</summary>
        public Polygon()
        {
            _vertices = new List<Vector3>();
            _color = Color.White;
        }

        public Polygon(ICollection<Vector3> vertices, Color color)
        {
            if (vertices.Count > 255)
                throw new ArgumentException("Polygon may have no more than 255 vertices.");
            _vertices = new List<Vector3>(vertices);
            _color = color;
        }

        #region Properties & Vertex Manipulation Methods

        public Color Color
        {
            get { return _color; }
            set { _color = value; SetDirty(); }
        }

        public Vector3 this[int index]
        {
            get { return _vertices[index]; }
            set { _vertices[index] = value; SetDirty(); }
        }

        public void Add(Vector3  vertex)
        {
            _vertices.Add(vertex);
            SetDirty();
        }

        public void AddRange(IEnumerable<Vector3> vertices)
        {
            _vertices.AddRange(vertices);
            SetDirty();
        }

        public void RemoveAt(int index)
        {
            _vertices.RemoveAt(index);
            SetDirty();
        }

        public void InsertAt(int index, Vector3 vertex)
        {
            _vertices.Insert(index, vertex);
            SetDirty();
        }

        public void Clear()
        {
            _vertices.Clear();
            SetDirty();
        }

        #endregion

        #region Overrides

        public override void Translate(Vector3 offset)
        {
            if (offset == Vector3.Origin)
                return;
            for (var i = 0; i < _vertices.Count; i++)
                _vertices[i] += offset;
            SetDirty();
        }

        internal override void SendMessage(UdpClient udpClient)
        {
            var pathBytes = ShapeSet.PathBytes;
            var numBytes = 18 * _vertices.Count + 8 + pathBytes.Length;
            var buf = new byte[numBytes];

            buf[0] = 1;
            buf[1] = 4;
            buf[2] = (byte)_vertices.Count;
            WriteColor(buf, 3, Color, true);

            var offset = 7;
            foreach (var vertex in _vertices)
            {
                WriteDouble(buf, offset, vertex.X);
                offset += 6;
                WriteDouble(buf, offset, vertex.Y);
                offset += 6;
                WriteDouble(buf, offset, vertex.Z);
                offset += 6;
            }

            pathBytes.CopyTo(buf, offset);

            var bytesSentCount = udpClient.Send(buf, buf.Length);
            Debug.Assert(bytesSentCount == numBytes);
        }

        #endregion
    }

    /// <summary>
    /// An unfilled 2D circle, constrained to the horizontal plane of the field.  Its line thickness remains the same regardless
    /// of its distance from the camera, hence the thickness unit is pixels.
    /// </summary>
    /// <remarks>Note that circles are always drawn on the plane of the field, and hence no Z position (elevation) is specified.</remarks>
    public sealed class Circle : Shape
    {
        private double _centerX;
        private double _centerY;
        private double _radiusMetres;
        private double _pixelThickness;
        private Color _color;

        /// <summary>Initialises a 1-metre diameter <see cref="TinMan.RoboViz.Circle"/> centered at the origin, drawn with a 5-pixel wide white line.</summary>
        public Circle()
        {
            _radiusMetres = 0.5;
            _pixelThickness = 5;
            _color = Color.White;
        }

        public Circle(double x, double y, double radiusMetres, double pixelThickness, Color color)
        {
            CenterX = x;
            CenterY = y;
            RadiusMetres = radiusMetres;
            PixelThickness = pixelThickness;
            Color = color;
        }

        #region Properties

        public double CenterX
        {
            get { return _centerX; }
            set { _centerX = value; SetDirty(); }
        }

        public double CenterY
        {
            get { return _centerY; }
            set { _centerY = value; SetDirty(); }
        }

        public double RadiusMetres
        {
            get { return _radiusMetres; }
            set { _radiusMetres = value; SetDirty(); }
        }

        public double PixelThickness
        {
            get { return _pixelThickness; }
            set { _pixelThickness = value; SetDirty(); }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; SetDirty(); }
        }

        #endregion

        #region Overrides

        /// <remarks>Note that the translation of circles cannot lift them off the playing field.</remarks>
        public override void Translate(Vector3 offset)
        {
            _centerX += offset.X;
            _centerY += offset.X;
            SetDirty();
        }

        internal override void SendMessage(UdpClient udpClient)
        {
            var pathBytes = ShapeSet.PathBytes;
            var numBytes = 30 + pathBytes.Length;
            var buf = new byte[numBytes];

            buf[0] = 1;
            WriteDouble(buf, 2, CenterX);
            WriteDouble(buf, 8, CenterY);
            WriteDouble(buf, 14, RadiusMetres);
            WriteDouble(buf, 20, PixelThickness);
            WriteColor(buf, 26, Color, false);
            pathBytes.CopyTo(buf, 29);

            var bytesSentCount = udpClient.Send(buf, buf.Length);
            Debug.Assert(bytesSentCount == numBytes);
        }

        #endregion
    }

    /// <summary>
    /// A solid sphere in 3D space.  Note that `X`,`Y`,`Z` are equivalent to `Center`, so use whichever is more
    /// convenient at the time.
    /// </summary>
    public sealed class Sphere : Shape
    {
        private double _x;
        private double _y;
        private double _z;
        private double _radiusMetres;
        private Color _color;

        /// <summary>Initialises a white 1-metre diameter <see cref="Sphere"/> centered at the origin.</summary>
        public Sphere()
        {
            _radiusMetres = 0.5;
            _color = Color.White;
        }

        public Sphere(Vector3 center, double radiusMetres, Color color)
        {
            Center = center;
            _radiusMetres = radiusMetres;
            _color = color;
        }

        #region Properties

        public double RadiusMetres
        {
            get { return _radiusMetres; }
            set { _radiusMetres = value; SetDirty(); }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; SetDirty(); }
        }

        public Vector3 Center
        {
            get { return new Vector3(X, Y, Z); }
            set { _x = value.X; _y = value.Y; _z = value.Z; SetDirty(); }
        }

        public double X
        {
            get { return _x; }
            set { _x = value; SetDirty(); }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; SetDirty(); }
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; SetDirty(); }
        }

        #endregion

        #region Overrides

        public override void Translate(Vector3 offset)
        {
            Center += offset;
        }

        internal override void SendMessage(UdpClient udpClient)
        {
            var pathBytes = ShapeSet.PathBytes;
            var numBytes = 30 + pathBytes.Length;
            var buf = new byte[numBytes];

            buf[0] = 1;
            buf[1] = 3;
            WriteDouble(buf, 2, Center.X);
            WriteDouble(buf, 8, Center.Y);
            WriteDouble(buf, 14, Center.Z);
            WriteDouble(buf, 20, RadiusMetres);
            WriteColor(buf, 26, Color, false);
            pathBytes.CopyTo(buf, 29);

            var bytesSentCount = udpClient.Send(buf, buf.Length);
            Debug.Assert(bytesSentCount == numBytes);
        }

        #endregion
    }
}