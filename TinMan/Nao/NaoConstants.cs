/*
 * Created by Drew, 11/05/2010 00:32.
 */
using System;

namespace Drew.RoboCup.Nao
{
    public static class NaoConstants
    {
        // HJ1  (neck) -120, 120
        // HJ2  (head) -45, 45
        // *AJ1 (shoulder) -120, 120
        // RAJ2 (upperarm) -95, 1
        // LAJ2 (upperarm) -1, 95
        // *AJ3 (elbow) -120, 120
        // RAJ4 (lowerarm) -1, 90
        // LAJ4 (lowerarm) -90, 1
        // *LJ1 (hip1) -90, 1
        // RLJ2 (hip2) -45, 25
        // LLJ2 (hip2) -25, 45
        // *LJ3 (thigh) -25, 100
        // *LJ4 (knee) -130, 1
        // *LJ5 (ankle) -45, 75
        // RLJ6 (foot) -25, 45
        // LLJ6 (foot) -45, 20 
    }
    
    public sealed class HingeJoint 
    {
        private double _angle;
        public double MinAngle { get; private set; }
        public double MaxAngle { get; private set; }
        
        public HingeJoint(string perceptorLabel, string effectorLabel, double minAngle, double maxAngle) {
            Angle = 0;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
        }
   
        public double Angle {
            get { return _angle; }
            set {
                _angle = Math.Max(MinAngle, Math.Min(MaxAngle, value));
            }
        }
    }
}
