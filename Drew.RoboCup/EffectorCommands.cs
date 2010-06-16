/*
 * Created by Drew, 17/05/2010 10:35.
 */
using System;

namespace Drew.RoboCup
{
    /*
     * GENERAL EFFECTORS
     * 
     * Create Effector
     *    Format:  (scene <filename>)
     *    Message: (scene rsg/agent/nao/nao.rsg)
     * HingeJoint Effector
     *    Format:  (<name> <ax>)
     *    Example: (lae3 5.3)
     *    Second parameter is the change in angle of the joint.
     * UniversalJoint Effector
     *    Format:  (<name> <ax1> <ax2>)
     *    Message: (lae1 -2.3 1.2)
     *    The second and third parameter contain the change in angle of the two joints.
     * 
     * SOCCER EFFECTORS
     * 
     * Init Effector
     *    Format:  (init (unum <playernumber>)(teamname <yourteamname>))
     *    Example: (init (unum 1)(teamname FHO))
     * Beam Effector
     *    Format:  (beam <x> <y> <rot>)
     *    Example: (beam 10.0 -10.0 0.0)
     *    For positioning player before game starts.  'rot' defines initial facing.
     * Say Effector
     *    Format:  (say <message>)
     *    Example: (say ``helloworld'')
     */
    
    public interface IEffectorCommand {
        string EffectorLabel { get; }
        string CommandExpression { get; }
    }
}
