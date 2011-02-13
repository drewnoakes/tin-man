#light

module TinMan.Samples.FSharp

open TinMan

type GetSmart() =
    let body = new NaoBody()
    let mutable isAlive = true
    let mutable context = null
    interface IAgent with
        member this.Body = body :> IBody
        member this.Context
            with get() = context
            and set(value) = context <- value
        member this.IsAlive = isAlive
        member this.Think(state) =
            // print the head's current angle
            printfn "%s" (body.HJ1.Angle.ToString())
            // request the head move to the left at 1 degree/sec
            body.HJ1.DesiredSpeed <- AngularSpeed.FromDegreesPerSecond(-1.0)
            // return unit as interface has void return type
            ()
        member this.Initialise() = ()
        member this.Shutdown() = ()

type AgentToo() =
    inherit AgentBase<NaoBody>(new NaoBody()) with
        override this.Think(state) =
            // return unit as interface has void return type
            ()
    
let body = new NaoBody()
let myAgent = {
    new AgentBase<NaoBody>(body) with
        override this.Think(state) =
            // return unit as interface has void return type
            ()
}

let agent = new GetSmart();
let host = new AgentHost()
//host.HostName <- "localhost"
host.Run(agent) |> ignore