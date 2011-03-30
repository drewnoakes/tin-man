using System.Drawing;

using TinMan;
using TinMan.RoboViz;

namespace TinManSamples.CSharp
{
    public sealed class LocalisationSimpleVisualisationAgent : AgentBase<NaoBody>
    {
        private readonly ILocaliser _localiser;
        private Dot _locationDot;

        public LocalisationSimpleVisualisationAgent(ILocaliser localiser)
            : base(new NaoBody())
        {
            _localiser = localiser;
        }

        public override void OnInitialise()
        {
            // Request that a RoboViz remote is created.
            // It will be disposed of automatically when the agent exits.
            var roboViz = new RoboVizRemote(this);

            // Create a 5-pixel, blue dot to indicate the agent's calculated position, hidden initially.
            // Keep a reference to this object so that we can manipulate it later.
            _locationDot = new Dot { PixelSize = 5, Color = Color.Blue, IsVisible = false };

            // Add it to the RoboViz remote using the group name 'localisation'.
            // This name will be prefixed with the side and agent number automatically.
            roboViz.Add(new ShapeSet("localisation") { _locationDot });
        }

        public override void Think(PerceptorState state)
        {
            // Provide the localiser with the latest state and see if it has a position for us.
            if (_localiser.Update(state))
            {
                // Update the dot's position.  This will automatically be reflected in the RoboViz UI.
                _locationDot.Position = _localiser.Position;

                // Make sure it's visible now that we have a position.
                _locationDot.IsVisible = true;
            }
        }
    }

    public interface ILocaliser
    {
        bool Update(PerceptorState state);
        Vector3 Position { get; }
    }
}