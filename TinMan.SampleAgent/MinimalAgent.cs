/*
 * Created by Drew, 10/06/2010 03:01. 
 */
using TinMan;

class MinimalAgent : AgentBase<NaoBody>
{
  public MinimalAgent()
    : base(new NaoBody()) {}

  public override void Step(ISimulationContext context, PerceptorState state)
  {
    // TODO do cool stuff in here
  }

//  static void Main()
//  {
//    new Client().Run(new SampleAgent());
//  }
}
