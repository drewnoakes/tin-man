using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using TinMan;
using TinMan.RoboViz;

namespace TinManSamples.CSharp
{
    public sealed class LocaliserVisualisationAgent : AgentBase<NaoBody>
    {
        private const int NumberOfPositionsToShow = 10;

        private readonly ILocalisationFilter _localiser;
        private Circle _bestGuessCircle;
        private Dot[] _filterDots;

        public LocaliserVisualisationAgent(ILocalisationFilter localiser)
            : base(new NaoBody())
        {
            _localiser = localiser;
        }

        public override void Initialise()
        {
            var roboViz = CreateRoboVizRemote(new RoboVizOptions());

            // create a circle to indicate the best guess at our position, hidden initially
            _bestGuessCircle = new Circle(0, 0, 0.25, 2, Color.Blue) { IsVisible = false };
            
            // create a swarm of dots to show the N best guesses
            _filterDots = new Dot[NumberOfPositionsToShow];
            for (var i = 0; i < NumberOfPositionsToShow; i++)
                _filterDots[i] = new Dot(Vector3.Origin, 3, Color.Transparent);

            // add them to the RoboViz remote using the group name 'localisation'
            // (this name will be prefixed with the side and agent number automatically)
            roboViz.Add(new ShapeSet("localisation") { _bestGuessCircle });
        }

        public override void Think(PerceptorState state)
        {
            // update with new state and see if anything changed
            if (_localiser.Update(state))
            {
                var guesses = _localiser.GetBestPositions();

                // show/hide and move our circle to show the best guess
                var bestGuess = guesses.First();
                _bestGuessCircle.CenterX = bestGuess.Position.X;
                _bestGuessCircle.CenterY = bestGuess.Position.Y;
                _bestGuessCircle.IsVisible = bestGuess.Weight >= 0.3; // threshold

                // move the swarm of dots to show the N best positions
                // we use their confidence/weighting value to control the alpha channel
                var i = 0;
                foreach (var guess in guesses.Take(NumberOfPositionsToShow))
                {
                    var alpha = (byte)((1 - guess.Weight)*255);
                    _filterDots[i].Color = Color.FromArgb(alpha, Color.White);
                    _filterDots[i].Position = guess.Position;
                    i++;
                }
            }
        }
    }

    /// <summary>Defines a particle filter.</summary>
    public interface ILocalisationFilter
    {
        bool Update(PerceptorState state);
        IEnumerable<WeightedPosition> GetBestPositions();
    }

    public struct WeightedPosition
    {
        public double Weight { get; set; }
        public Vector3 Position { get; set; }
    }
}