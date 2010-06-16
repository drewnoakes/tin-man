/*
 * Created by Drew, 10/06/2010 03:01.
 */
using System;
using System.Collections.Generic;

namespace TinMan.SampleBot
{
    internal sealed class SampleAgent : IAgent, IUserInteractiveAgent {
        public string RsgPath { get { return Nao.RsgPath; }}
        // TODO what happens if we send 0
        // TODO what happens if we send negative while inertia moving positive
        
        private bool _beamToGoal = true;
        private int _stableCount = 0;
        private int _moveCount = 0;
        private AngularSpeed _impulseSpeed = AngularSpeed.FromDegreesPerSecond(5) ;
        private Angle _lastAngle;
        private TimeSpan _lastSimulationTime;
        private Hinge _hinge = NaoHinge.HJ1;
        public IEnumerable<IEffectorCommand> Step(PerceptorState state) {
            if (_beamToGoal) {
                _beamToGoal = false;
                return new[] { new BeamCommand(-FieldGeometry.FieldXLength/2, 0, Angle.Zero) };
            }
            Angle currentAngle = state.GetHingeAngle(_hinge);
            AngularSpeed speedLastCycle = (currentAngle - _lastAngle) / (state.SimulationTime - _lastSimulationTime);
            AngularSpeed speedToRequest;
            if (_moveCount>0) {
                speedToRequest = _impulseSpeed;
                _stableCount = 3;
                _moveCount--;
            } else {
                speedToRequest = AngularSpeed.Zero;
                if (speedLastCycle==AngularSpeed.Zero && _stableCount>0)
                    _stableCount--;
            }
            _lastAngle = currentAngle;
            _lastSimulationTime = state.SimulationTime;
            
            if (_stableCount!=0) {
                Console.WriteLine("{0:###0.00###}, {1:0.00}, {2}, {3}", 
                                  state.SimulationTime.TotalSeconds,
                                  currentAngle.Degrees, 
                                  speedLastCycle.DegreesPerSecond, 
                                  speedToRequest.DegreesPerSecond);
            }
            
            return new [] { new MoveHingeCommand(_hinge, speedToRequest) };
        }
        
        public void HandleUserInput(char key) {
            if (key=='B') {
                _beamToGoal = true;
                Console.WriteLine("Beaming to goal");
            }
            if (key=='-') {
                _impulseSpeed = -_impulseSpeed;
                Console.WriteLine("Speed: {0}", _impulseSpeed.DegreesPerSecond);
            }
            if (key=='>') {
                _impulseSpeed += AngularSpeed.FromDegreesPerSecond(1);
                Console.WriteLine("Speed: {0}", _impulseSpeed.DegreesPerSecond);
            }
            if (key=='<') {
                _impulseSpeed -= AngularSpeed.FromDegreesPerSecond(1);
                Console.WriteLine("Speed: {0}", _impulseSpeed.DegreesPerSecond);
            }
            if (key>='1' && key<='9') {
                Console.WriteLine("Time,Angle,LastSpeed,SpeedRequest");
                _moveCount = key - '0';
                _lastAngle = Angle.NaN;
            }
        }
    }
    
    internal static class Program {
        public static void Main() {
            new Client().Run(new SampleAgent());
        }
    }
}