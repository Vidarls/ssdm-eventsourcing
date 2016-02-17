open FSharp.Control

type Because = Because of string
type Id = Id of string

type Event = 
| Born of string
| Named of string
| ChangedName of string * Because
| MovedTo of string * Because

type Message = 
| Event of Id * Event
| GetAll of Id * AsyncReplyChannel<Event list>

let eventStore = new MailboxProcessor<Message>(fun inbox ->
    let rec loop events = 
        async {
            let! received = inbox.Receive ()
            match received with
            | GetAll(id, replyChannel) -> 
                events 
                |> List.filter (fun (personId,_) -> personId = id)
                |> List.map (fun (_,e) -> e)
                |> List.rev
                |> replyChannel.Reply
                return! loop events
            | Event(id,event) -> 
                printfn "Saving %A for %A" event id 
                return! loop ((id,event) :: events)
        }
    loop [])
eventStore.Start ()



let save id event = (id,event) |> Event |> eventStore.Post
let whatHappenedTo id = eventStore.PostAndReply(fun channel ->
    GetAll (id, channel))
