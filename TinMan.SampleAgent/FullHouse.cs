/*
 * Created by Drew, 12/06/2010 06:35.
 */
using System.Threading;

using TinMan;

class HerdAgent : AgentBase<NaoBody>
{
  public HerdAgent()
    : base(new NaoBody()) {}

  public override void Step(ISimulationContext context, PerceptorState state)
  {
    if (context.TeamName=="Team1") {
      Body.LAJ1.Speed = AngularSpeed.FromDegreesPerSecond(0.01);
      Body.RAJ1.Speed = AngularSpeed.FromDegreesPerSecond(0.01);
    }
  }
}

class FullHouse
{
  static void Main()
  {
    CreateTeam(1, "Team1");
    CreateTeam(1, "Team2");
  }
  
  public static void CreateTeam(int agentCount, string teamName) {
    for (int i=0; i<agentCount; i++) {
      new Thread(() => new Client{TeamName=teamName}.Run(new HerdAgent())).Start();
      // give the server a chance to catch its breath
      Thread.Sleep(2000);
    }
  }
}
