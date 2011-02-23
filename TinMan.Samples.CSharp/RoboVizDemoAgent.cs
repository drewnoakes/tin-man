using System;
using System.Drawing;
using TinMan;
using TinMan.RoboViz;

namespace TinManSamples.CSharp
{
    /// <summary>
    /// This agent is the TinMan equivalent implementation of the sample app provided by the
    /// RoboViz team on their webiste: https://sites.google.com/site/umroboviz
    /// <para/>
    /// It's purpose is to highlight a combination of static and animated elements
    /// and is not representative of how this API would ordinarily be used.
    /// See <see cref="LocalisationFilterVisualisationAgent" /> for a simpler, more likely example.
    /// </summary>
    public class RoboVizDemoAgent : AgentBase<NaoBody>
    {
        private Dot[] _waveDots;
        private Line[] _spinnerLines;

        public RoboVizDemoAgent()
            : base(new NaoBody())
        {}

        public override void Initialise()
        {
            var roboViz = new RoboVizRemote(this);
            roboViz.AgentText = "Hello World";
            roboViz.AgentTextColor = Color.DarkRed;

            // During agent initialisation, we construct all the shapes we're going to need, and arrange them in a hierarchy.
            // We assign references to certain dots and lines to fields so that we can animate them later during 'Think'.

            // Static shapes:
            var polygonVertices = new[]
                        {
                            new Vector3(0, 0, 0),
                            new Vector3(1, 0, 0),
                            new Vector3(1, 1, 0),
                            new Vector3(0, 3, 0),
                            new Vector3(-2, -2, 0)
                        };
            var fieldLines = new ShapeSet("fieldlines")
                           {
                               new Line(FieldGeometry.FlagLeftBottomPosition, FieldGeometry.FlagLeftTopPosition, 1, Color.LightGreen), 
                               new Line(FieldGeometry.FlagRightTopPosition, FieldGeometry.FlagRightBottomPosition, 1, Color.LightGreen), 
                           };
            for (var x = FieldGeometry.FieldXLeft; x <= FieldGeometry.FieldXRight; x++)
                fieldLines.Add(new Line(new Vector3(x, FieldGeometry.FieldYTop, 0), new Vector3(x, FieldGeometry.FieldYBottom, 0), 1, Color.LightGreen));

            roboViz.Add(new ShapeSet("static")
                        {
                            new ShapeSet("axes")
                                {
                                    new Line(Vector3.Origin, new Vector3(3, 0, 0), 3, Color.Red),
                                    new Line(Vector3.Origin, new Vector3(0, 3, 0), 3, Color.Green),
                                    new Line(Vector3.Origin, new Vector3(0, 0, 3), 3, Color.Blue)
                                },
                            new ShapeSet("circles")
                                {
                                    new Circle(-5, 0, 3, 2, Color.Blue),
                                    new Circle(5, 0, 3, 2, Color.Blue)
                                },
                            new ShapeSet("spheres")
                                {
                                    new Sphere(new Vector3(-5, 0, 2), 0.5, Color.Pink),
                                    new Sphere(new Vector3(5, 0, 2), 0.5, Color.Pink)
                                },
                            new ShapeSet("polygons")
                                {
                                    new Polygon(polygonVertices, Color.FromArgb(128, Color.White))
                                },
                            fieldLines,
                            new FieldAnnotation { Text = "LB", Color = Color.Orange, Position = FieldGeometry.FlagLeftBottomPosition },
                            new FieldAnnotation { Text = "LT", Color = Color.Orange, Position = FieldGeometry.FlagLeftTopPosition },
                            new FieldAnnotation { Text = "RB", Color = Color.Orange, Position = FieldGeometry.FlagRightBottomPosition },
                            new FieldAnnotation { Text = "LT", Color = Color.Orange, Position = FieldGeometry.FlagRightTopPosition },
                            new FieldAnnotation { Text = "(0,0)", Color = Color.White, Position = Vector3.Origin }
                        });

            // Animated shapes:
            var wave = new ShapeSet("wave");
            _waveDots = new Dot[60];
            for (var i = 0; i < 60; i++)
            {
                var p = i / 60.0f;
                var dot = new Dot(new Vector3(-9 + 18 * p, p * 12 - 6, 0), 5, Color.Black);
                _waveDots[i] = dot;
                wave.Add(dot);
            }

            var triangle = new ShapeSet("spinner");
            _spinnerLines = new[]
                                {
                                    new Line(Vector3.Origin, new Vector3(2, 0, 1.5), 5, Color.Yellow),
                                    new Line(new Vector3(2, 0, 1.5), new Vector3(2, 0, 0), 5, Color.Yellow),
                                    new Line(Vector3.Origin, new Vector3(2, 0, 0), 5, Color.Yellow),
                                };
            triangle.AddRange(_spinnerLines);
            
            roboViz.Add(new ShapeSet("animated") { wave, triangle });
        }

        public override void Think(PerceptorState state)
        {
            if (Console.KeyAvailable && char.ToUpper(Console.ReadKey(true).KeyChar) == 'Q')
                StopSimulation();

            var angle = state.SimulationTime.TotalSeconds * (Math.PI * 2);

            // Animate points wave
            for (var i = 0; i < 60; i++)
                _waveDots[i].Z = Math.Max(0, Math.Sin(angle + (i/60.0f)*18));

            // Animate triangle
            var x = Math.Cos(angle)*2;
            var y = Math.Sin(angle)*2;
            var z = Math.Cos(angle) + 1.5;
            _spinnerLines[0].X2 = _spinnerLines[1].X1 = x;
            _spinnerLines[0].Y2 = _spinnerLines[1].Y1 = y;
            _spinnerLines[0].Z2 = _spinnerLines[1].Z1 = z;
            _spinnerLines[1].X2 = _spinnerLines[2].X2 = x;
            _spinnerLines[1].Y2 = _spinnerLines[2].Y2 = y;
        }
    }
}