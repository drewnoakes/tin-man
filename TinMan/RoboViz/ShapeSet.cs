using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TinMan.RoboViz
{
    public sealed class ShapeSet : IEnumerable
    {
        private readonly List<Shape> _shapes = new List<Shape>();
        private readonly List<ShapeSet> _subsets = new List<ShapeSet>();
        private readonly string _name;
        private ShapeSet _parentSet;
        private RoboVizRemote _parentRoot;

        public ShapeSet(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Trim().Length == 0)
                throw new ArgumentException("Must be non blank.", "name");
            if (name.IndexOf('.') != -1)
                throw new ArgumentException("Cannot contain the '.' character.  If you wish to represent hierarchy, use nested ShapeSet objects.", "name");

            _name = name;
        }

        public IEnumerable<ShapeSet> SubSets
        {
            get { return _subsets; }
        }

        private string _path;
        private byte[] _pathBytes;

        public string Path
        {
            get
            {
                if (_path == null)
                {
                    if (_parentSet != null)
                        _path = _parentSet.Path + '.' + _name;
                    else if (_parentRoot != null)
                        _path = _parentRoot.Path.Length == 0 ? _name : _parentRoot.Path + '.' + _name;
                    else
                        throw new InvalidOperationException("Cannot determine path for a ShapeSet that does not have a RoboVizRemote at the root of its hierarchy.");
                }
                return _path;
            }
        }

        public byte[] PathBytes
        {
            get 
            {
                if (_pathBytes == null)
                    _pathBytes = Encoding.ASCII.GetBytes(Path);
                return _pathBytes;
            }
        }

        public void Add(Shape shape)
        {
            if (shape == null)
                throw new ArgumentNullException("shape");
            _shapes.Add(shape);
            shape.SetShapeSet(this);
            IsDirty = true;
        }

        public void AddRange(IEnumerable<Shape> shapes)
        {
            foreach (var shape in shapes)
                Add(shape);
        }

        public void Add(ShapeSet childSubSet)
        {
            if (childSubSet == null)
                throw new ArgumentNullException("childSubSet");
            _subsets.Add(childSubSet);
            childSubSet.SetParent(this);
        }

        /// <summary>
        /// Translates any contained Shapes and/or nested ShapeSets by the specified offset.
        /// </summary>
        /// <param name="offset"></param>
        public void Translate(Vector3 offset)
        {
            if (offset == Vector3.Origin)
                return;
            foreach (var shape in _shapes)
                shape.Translate(offset);
            foreach (var subset in _subsets)
                subset.Translate(offset);
        }

        internal bool IsDirty { get; set; }

        private void SetParent(ShapeSet parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (_parentSet != null || _parentRoot != null)
                throw new InvalidOperationException("Parent has already been set.");
            _parentSet = parent;
        }

        internal void SetParent(RoboVizRemote parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (_parentSet != null || _parentRoot != null)
                throw new InvalidOperationException("Parent has already been set.");
            _parentRoot = parent;
        }

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            // This method exists only so that we can use the C# initialisation syntax.  Eg:
            // new ShapeSet("foo") {  }
            throw new NotImplementedException();
        }

        #endregion

        public void FlushMessages(UdpClient udpClient)
        {
            foreach (var shape in _shapes)
            {
                if (shape.IsVisible)
                    shape.SendMessage(udpClient);
            }
            
            foreach (var set in _subsets)
                set.FlushMessages(udpClient);
            
            IsDirty = false;
        }

        public void Remove(Shape shape)
        {
            var success = _shapes.Remove(shape);
            if (!success)
                throw new ArgumentException("Cannot remove the Shape as it is not contained in this ShapeSet.");
            IsDirty = true;
        }
    }
}