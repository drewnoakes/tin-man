#light

module TinMan.Samples.FSharp

open TinMan

type GetSmart() =
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