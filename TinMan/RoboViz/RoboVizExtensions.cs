using System;
using TinMan.Annotations;

namespace TinMan.RoboViz
{
    public static class RoboVizExtensions
    {
        /// <summary>
        /// Transforms <paramref name="line"/> according to <paramref name="transformationMatrix"/>.
        /// Thickness and colour values are copied, unchanged.
        /// </summary>
        /// <param name="transformationMatrix">The 3D transformation to apply to the line.</param>
        /// <param name="line">The line to be transformed.</param>
        /// <returns></returns>
        [NotNull, Pure]
        public static Line Transform([NotNull] this TransformationMatrix transformationMatrix, [NotNull] Line line)
        {
            if (transformationMatrix == null) 
                throw new ArgumentNullException("transformationMatrix");
            if (line == null) 
                throw new ArgumentNullException("line");

            return new Line(
                transformationMatrix.Transform(line.End1), 
                transformationMatrix.Transform(line.End2), 
                line.PixelThickness, 
                line.Color);
        }
    }
}