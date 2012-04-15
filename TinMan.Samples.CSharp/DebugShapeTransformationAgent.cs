using TinMan;
using TinMan.RoboViz;

namespace TinManSamples.CSharp
{
    public sealed class DebugShapeTransformationAgent : AgentBase<NaoBody>
    {
        private RoboVizRemote _debugger;
        private readonly Line[] _lines;
        private readonly ShapeSet _shapeSet;

        public DebugShapeTransformationAgent() : base(new NaoBody())
        {
            _lines = new[]
                         {
                             new Line(new Vector3(0, 0, 0), new Vector3(0, 1, 0)), 
                             new Line(new Vector3(0, 1, 0), new Vector3(1, 1, 0)), 
                             new Line(new Vector3(1, 1, 0), new Vector3(1, 0, 0)), 
                             new Line(new Vector3(1, 0, 0), new Vector3(0, 0, 0)),
                             new Line(new Vector3(0, 0, 0), new Vector3(0, 0, 1)), 
                             new Line(new Vector3(0, 0, 1), new Vector3(1, 0, 1)), 
                             new Line(new Vector3(1, 0, 1), new Vector3(1, 0, 0)), 
                             new Line(new Vector3(1, 0, 0), new Vector3(0, 0, 0)),
                             new Line(new Vector3(0, 0, 0), new Vector3(0, 0, 1)), 
                             new Line(new Vector3(0, 0, 1), new Vector3(0, 1, 1)), 
                             new Line(new Vector3(0, 1, 1), new Vector3(0, 1, 0)), 
                             new Line(new Vector3(0, 1, 0), new Vector3(0, 0, 0)),

                             new Line(new Vector3(0, 0, 1), new Vector3(0, 1, 1)), 
                             new Line(new Vector3(0, 1, 1), new Vector3(1, 1, 1)), 
                             new Line(new Vector3(1, 1, 1), new Vector3(1, 0, 1)), 
                             new Line(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                             new Line(new Vector3(0, 1, 0), new Vector3(0, 1, 1)), 
                             new Line(new Vector3(0, 1, 1), new Vector3(1, 1, 1)), 
                             new Line(new Vector3(1, 1, 1), new Vector3(1, 1, 0)), 
                             new Line(new Vector3(1, 1, 0), new Vector3(0, 1, 0)),
                             new Line(new Vector3(1, 0, 0), new Vector3(1, 0, 1)), 
                             new Line(new Vector3(1, 0, 1), new Vector3(1, 1, 1)), 
                             new Line(new Vector3(1, 1, 1), new Vector3(1, 1, 0)), 
                             new Line(new Vector3(1, 1, 0), new Vector3(1, 0, 0))
                         };
            _shapeSet = new ShapeSet("Cube");
        }

        public override void OnInitialise()
        {
            _debugger = new RoboVizRemote(this);
            _debugger.Add(_shapeSet);
            base.OnInitialise();
        }

        public override void Think(PerceptorState state)
        {
            var timeAngle = Angle.FromRadians(state.SimulationTime.TotalSeconds);

            var transformation = TransformationMatrix.Identity.Translate(Vector3.Origin.WithZ(1)).RotateX(timeAngle).RotateY(timeAngle).Translate(Vector3.Origin.WithX(1)).RotateZ(timeAngle);

            _shapeSet.Clear();

            foreach (var line in _lines)
                _shapeSet.Add(transformation.Transform(line));
        }
    }
}