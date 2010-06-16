/*
 * Created by Drew, 28/05/2010 12:51.
 */

using System;

namespace Drew.RoboCup
{
    /// <summary>
    /// A function that calculates a transition between two values over time.
    /// </summary>
    /// <param name="t">The amount of elapsed time through the animation. Should be greater than zero and less than <paramref name="d"/>.</param>
    /// <param name="b">The animation's start value (from).</param>
    /// <param name="c">The animation's total change in value (to - from).</param>
    /// <param name="d">The total duration for the animation.</param>
    public delegate double EasingFunction(double t, double b, double c, double d);

    /// <summary>
    /// Static factory of functions that control easing.
    /// </summary>
    public static class Easing {
        #region Equations
    
        /// <summary>Easing equation function for a simple linear tweening, with no easing.</summary>
        private static double Linear(double t, double b, double c, double d) { return c*t/d + b; }
    
        #region Expo
    
        /// <summary>Easing equation function for an exponential (2^t) easing out: decelerating from zero velocity.</summary>
        private static double ExpoEaseOut(double t, double b, double c, double d) { return (t == d) ? b + c : c * (-Math.Pow(2, -10 * t / d) + 1) + b; }
    
        /// <summary>Easing equation function for an exponential (2^t) easing in: accelerating from zero velocity.</summary>
        private static double ExpoEaseIn(double t, double b, double c, double d) { return (t == 0) ? b : c * Math.Pow(2, 10 * (t / d - 1)) + b; }
    
        /// <summary>Easing equation function for an exponential (2^t) easing in/out: acceleration until halfway, then deceleration.</summary>
        private static double ExpoEaseInOut(double t, double b, double c, double d)
        {
            if (t == 0)
                return b;
    
            if (t == d)
                return b + c;
    
            if ((t /= d/2) < 1)
                return c/2*Math.Pow(2, 10*(t - 1)) + b;
    
            return c/2*(-Math.Pow(2, -10*--t) + 2) + b;
        }
    
        /// <summary>Easing equation function for an exponential (2^t) easing out/in: deceleration until halfway, then acceleration.</summary>
        private static double ExpoEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? ExpoEaseOut(t*2, b, c/2, d) : ExpoEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #region Circular
    
        /// <summary>Easing equation function for a circular (sqrt(1-t^2)) easing out: decelerating from zero velocity.</summary>
        private static double CircEaseOut(double t, double b, double c, double d) { return c*Math.Sqrt(1 - (t = t/d - 1)*t) + b; }
    
        /// <summary>Easing equation function for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.</summary>
        private static double CircEaseIn(double t, double b, double c, double d) { return -c*(Math.Sqrt(1 - (t /= d)*t) - 1) + b; }
    
        /// <summary>Easing equation function for a circular (sqrt(1-t^2)) easing in/out: acceleration until halfway, then deceleration.</summary>
        private static double CircEaseInOut(double t, double b, double c, double d) { return (t /= d/2) < 1 ? -c/2*(Math.Sqrt(1 - t*t) - 1) + b : c/2*(Math.Sqrt(1 - (t -= 2)*t) + 1) + b; }
    
        /// <summary>Easing equation function for a circular (sqrt(1-t^2)) easing in/out: acceleration until halfway, then deceleration.</summary>
        private static double CircEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? CircEaseOut(t*2, b, c/2, d) : CircEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #region Quad
    
        /// <summary>Easing equation function for a quadratic (t^2) easing out: decelerating from zero velocity.</summary>
        private static double QuadEaseOut(double t, double b, double c, double d) { return -c*(t /= d)*(t - 2) + b; }
    
        /// <summary>Easing equation function for a quadratic (t^2) easing in: accelerating from zero velocity.</summary>
        private static double QuadEaseIn(double t, double b, double c, double d) { return c*(t /= d)*t + b; }
    
        /// <summary>Easing equation function for a quadratic (t^2) easing in/out: acceleration until halfway, then deceleration.</summary>
        private static double QuadEaseInOut(double t, double b, double c, double d) { return (t /= d/2) < 1 ? c/2*t*t + b : -c/2*((--t)*(t - 2) - 1) + b; }
    
        /// <summary>Easing equation function for a quadratic (t^2) easing out/in: deceleration until halfway, then acceleration.
        private static double QuadEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? QuadEaseOut(t*2, b, c/2, d) : QuadEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #region Sine
    
        /// <summary>Easing equation function for a sinusoidal (sin(t)) easing out: decelerating from zero velocity.</summary>
        private static double SineEaseOut(double t, double b, double c, double d) { return c*Math.Sin(t/d*(Math.PI/2)) + b; }
    
        /// <summary>Easing equation function for a sinusoidal (sin(t)) easing in: accelerating from zero velocity.</summary>
        private static double SineEaseIn(double t, double b, double c, double d) { return -c*Math.Cos(t/d*(Math.PI/2)) + c + b; }
    
        /// <summary>Easing equation function for a sinusoidal (sin(t)) easing in/out: acceleration until halfway, then deceleration.</summary>
        private static double SineEaseInOut(double t, double b, double c, double d) { return (t /= d/2) < 1 ? c/2*(Math.Sin(Math.PI*t/2)) + b : -c/2*(Math.Cos(Math.PI*--t/2) - 2) + b; }
    
        /// <summary>Easing equation function for a sinusoidal (sin(t)) easing in/out: deceleration until halfway, then acceleration.</summary>
        private static double SineEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? SineEaseOut(t*2, b, c/2, d) : SineEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #region Cubic
    
        /// <summary>Easing equation function for a cubic (t^3) easing out: decelerating from zero velocity.</summary>
        private static double CubicEaseOut(double t, double b, double c, double d) { return c*((t = t/d - 1)*t*t + 1) + b; }
    
        /// <summary>Easing equation function for a cubic (t^3) easing in: accelerating from zero velocity.
        private static double CubicEaseIn(double t, double b, double c, double d) { return c*(t /= d)*t*t + b; }
    
        /// <summary>Easing equation function for a cubic (t^3) easing in/out: acceleration until halfway, then deceleration.
        private static double CubicEaseInOut(double t, double b, double c, double d) { return (t /= d/2) < 1 ? c/2*Math.Pow(t, 3) + b : c/2*((t -= 2)*t*t + 2) + b; }
    
        /// <summary>Easing equation function for a cubic (t^3) easing out/in: deceleration until halfway, then acceleration.
        private static double CubicEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? CubicEaseOut(t*2, b, c/2, d) : CubicEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #region Quartic
    
        /// <summary>Easing equation function for a quartic (t^4) easing out: decelerating from zero velocity.
        private static double QuartEaseOut(double t, double b, double c, double d) { return -c*((t = t/d - 1)*Math.Pow(t, 3) - 1) + b; }
    
        /// <summary>Easing equation function for a quartic (t^4) easing in: accelerating from zero velocity.
        private static double QuartEaseIn(double t, double b, double c, double d) { return c*(t /= d)*Math.Pow(t, 3) + b; }
    
        /// <summary>Easing equation function for a quartic (t^4) easing in/out: acceleration until halfway, then deceleration.
        private static double QuartEaseInOut(double t, double b, double c, double d) { return (t /= d/2) < 1 ? c/2*Math.Pow(t, 4) + b : -c/2*((t -= 2)*t*t*t - 2) + b; }
    
        /// <summary>Easing equation function for a quartic (t^4) easing out/in: deceleration until halfway, then acceleration.
        private static double QuartEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? QuartEaseOut(t*2, b, c/2, d) : QuartEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #region Quintic
    
        /// <summary>Easing equation function for a quintic (t^5) easing out: decelerating from zero velocity.
        private static double QuintEaseOut(double t, double b, double c, double d) { return c*((t = t/d - 1)*Math.Pow(t, 4) + 1) + b; }
    
        /// <summary>Easing equation function for a quintic (t^5) easing in: accelerating from zero velocity.
        private static double QuintEaseIn(double t, double b, double c, double d) { return c * (t /= d) * Math.Pow(t, 4) + b; }
    
        /// <summary>Easing equation function for a quintic (t^5) easing in/out: acceleration until halfway, then deceleration.
        private static double QuintEaseInOut(double t, double b, double c, double d) { return (t /= d/2) < 1 ? c/2*Math.Pow(t, 5) + b : c/2*((t -= 2)*Math.Pow(t, 4) + 2) + b; }
    
        /// <summary>Easing equation function for a quintic (t^5) easing in/out: acceleration until halfway, then deceleration.</summary>
        private static double QuintEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? QuintEaseOut(t*2, b, c/2, d) : QuintEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #region Elastic
    
        /// <summary>Easing equation function for an elastic (exponentially decaying sine wave) easing out: decelerating from zero velocity.</summary>
        private static double ElasticEaseOut(double t, double b, double c, double d)
        {
            if ((t /= d) == 1)
                return b + c;
    
            double p = d*.3;
            double s = p/4;
    
            return (c*Math.Pow(2, -10*t)*Math.Sin((t*d - s)*(2*Math.PI)/p) + c + b);
        }
    
        /// <summary>Easing equation function for an elastic (exponentially decaying sine wave) easing in: accelerating from zero velocity.</summary>
        private static double ElasticEaseIn(double t, double b, double c, double d)
        {
            if ((t /= d) == 1)
                return b + c;
    
            double p = d*.3;
            double s = p/4;
    
            return -(c*Math.Pow(2, 10*(t -= 1))*Math.Sin((t*d - s)*(2*Math.PI)/p)) + b;
        }
    
        /// <summary>Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: acceleration until halfway, then deceleration.</summary>
        private static double ElasticEaseInOut(double t, double b, double c, double d)
        {
            if ((t /= d/2) == 2)
                return b + c;
    
            double p = d*(.3*1.5);
            double s = p/4;
    
            if (t < 1)
                return -.5*(c*Math.Pow(2, 10*(t -= 1))*Math.Sin((t*d - s)*(2*Math.PI)/p)) + b;
            return c*Math.Pow(2, -10*(t -= 1))*Math.Sin((t*d - s)*(2*Math.PI)/p)*.5 + c + b;
        }
    
        /// <summary>Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: deceleration until halfway, then acceleration.</summary>
        private static double ElasticEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? ElasticEaseOut(t*2, b, c/2, d) : ElasticEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #region Bounce
    
        /// <summary>Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.</summary>
        private static double BounceEaseOut(double t, double b, double c, double d)
        {
            if ((t /= d) < (1/2.75))
                return c*(7.5625*t*t) + b;
            else if (t < (2/2.75))
                return c*(7.5625*(t -= (1.5/2.75))*t + .75) + b;
            else if (t < (2.5/2.75))
                return c*(7.5625*(t -= (2.25/2.75))*t + .9375) + b;
            else
                return c*(7.5625*(t -= (2.625/2.75))*t + .984375) + b;
        }
    
        /// <summary>Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.</summary>
        private static double BounceEaseIn(double t, double b, double c, double d) { return c - BounceEaseOut(d - t, 0, c, d) + b; }
    
        /// <summary>Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.</summary>
        private static double BounceEaseInOut(double t, double b, double c, double d) { return t < d/2 ? BounceEaseIn(t*2, 0, c, d)*.5 + b : BounceEaseOut(t*2 - d, 0, c, d)*.5 + c*.5 + b; }
    
        /// <summary>Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: deceleration until halfway, then acceleration.</summary>
        private static double BounceEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? BounceEaseOut(t*2, b, c/2, d) : BounceEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #region Back
    
        /// <summary>Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: decelerating from zero velocity.</summary>
        private static double BackEaseOut(double t, double b, double c, double d) { return c*((t = t/d - 1)*t*((1.70158 + 1)*t + 1.70158) + 1) + b; }
    
        /// <summary>Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.</summary>
        private static double BackEaseIn(double t, double b, double c, double d) { return c*(t /= d)*t*((1.70158 + 1)*t - 1.70158) + b; }
    
        /// <summary>Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: acceleration until halfway, then deceleration.</summary>
        private static double BackEaseInOut(double t, double b, double c, double d)
        {
            double s = 1.70158;
            if ((t /= d/2) < 1)
                return c/2*(t*t*(((s *= (1.525)) + 1)*t - s)) + b;
            return c/2*((t -= 2)*t*(((s *= (1.525)) + 1)*t + s) + 2) + b;
        }
    
        /// <summary>Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: deceleration until halfway, then acceleration.</summary>
        private static double BackEaseOutIn(double t, double b, double c, double d) { return t < d/2 ? BackEaseOut(t*2, b, c/2, d) : BackEaseIn((t*2) - d, b + c/2, c/2, d); }
    
        #endregion
    
        #endregion

        /// <summary>
        /// Gets the <see cref="EasingFunction"/> for <paramref name="easeType"/>.
        /// </summary>
        public static EasingFunction GetFunction(EaseType easeType)
        {
            switch (easeType) {
                default: return Linear;
                case EaseType.BackEaseIn: return BackEaseIn;
                case EaseType.BackEaseInOut: return BackEaseInOut;
                case EaseType.BackEaseOut: return BackEaseOut;
                case EaseType.BackEaseOutIn: return BackEaseOutIn;
                case EaseType.BounceEaseIn: return BounceEaseIn;
                case EaseType.BounceEaseInOut: return BounceEaseInOut;
                case EaseType.BounceEaseOut: return BounceEaseOut;
                case EaseType.BounceEaseOutIn: return BounceEaseOutIn;
                case EaseType.CircEaseIn: return CircEaseIn;
                case EaseType.CircEaseInOut: return CircEaseInOut;
                case EaseType.CircEaseOut: return CircEaseOut;
                case EaseType.CircEaseOutIn: return CircEaseOutIn;
                case EaseType.CubicEaseIn: return CubicEaseIn;
                case EaseType.CubicEaseInOut: return CubicEaseInOut;
                case EaseType.CubicEaseOut: return CubicEaseOut;
                case EaseType.CubicEaseOutIn: return CubicEaseOutIn;
                case EaseType.ElasticEaseIn: return ElasticEaseIn;
                case EaseType.ElasticEaseInOut: return ElasticEaseInOut;
                case EaseType.ElasticEaseOut: return ElasticEaseOut;
                case EaseType.ElasticEaseOutIn: return ElasticEaseOutIn;
                case EaseType.ExpoEaseIn: return ExpoEaseIn;
                case EaseType.ExpoEaseInOut: return ExpoEaseInOut;
                case EaseType.ExpoEaseOut: return ExpoEaseOut;
                case EaseType.ExpoEaseOutIn: return ExpoEaseOutIn;
                case EaseType.QuadEaseIn: return QuadEaseIn;
                case EaseType.QuadEaseInOut: return QuadEaseInOut;
                case EaseType.QuadEaseOut: return QuadEaseOut;
                case EaseType.QuadEaseOutIn: return QuadEaseOutIn;
                case EaseType.QuartEaseIn: return QuartEaseIn;
                case EaseType.QuartEaseInOut: return QuartEaseInOut;
                case EaseType.QuartEaseOut: return QuartEaseOut;
                case EaseType.QuartEaseOutIn: return QuartEaseOutIn;
                case EaseType.QuintEaseIn: return QuintEaseIn;
                case EaseType.QuintEaseInOut: return QuintEaseInOut;
                case EaseType.QuintEaseOut: return QuintEaseOut;
                case EaseType.QuintEaseOutIn: return QuintEaseOutIn;
                case EaseType.SineEaseIn: return SineEaseIn;
                case EaseType.SineEaseInOut: return SineEaseInOut;
                case EaseType.SineEaseOut: return SineEaseOut;
                case EaseType.SineEaseOutIn: return SineEaseOutIn;
            }
        }
    }

    /// <summary>Enumeration of all easing types.</summary>
    public enum EaseType
    {
        Linear,
        QuadEaseOut,
        QuadEaseIn,
        QuadEaseInOut,
        QuadEaseOutIn,
        ExpoEaseOut,
        ExpoEaseIn,
        ExpoEaseInOut,
        ExpoEaseOutIn,
        CubicEaseOut,
        CubicEaseIn,
        CubicEaseInOut,
        CubicEaseOutIn,
        QuartEaseOut,
        QuartEaseIn,
        QuartEaseInOut,
        QuartEaseOutIn,
        QuintEaseOut,
        QuintEaseIn,
        QuintEaseInOut,
        QuintEaseOutIn,
        CircEaseOut,
        CircEaseIn,
        CircEaseInOut,
        CircEaseOutIn,
        SineEaseOut,
        SineEaseIn,
        SineEaseInOut,
        SineEaseOutIn,
        ElasticEaseOut,
        ElasticEaseIn,
        ElasticEaseInOut,
        ElasticEaseOutIn,
        BounceEaseOut,
        BounceEaseIn,
        BounceEaseInOut,
        BounceEaseOutIn,
        BackEaseOut,
        BackEaseIn,
        BackEaseInOut,
        BackEaseOutIn
    }
}