open FSharp.Control

type Id = Id of string

type Event = 
| Born of string
| Named of string
| ChangedName of string
| AddressChanged of string
| Moved of string

type Message = 
| Event of Id * Event
| GetAll of Id * AsyncReplyChannel<Event list>

let eventStore = new MailboxProcessor<Message>(fun inbox ->
    let rec loop events = 
        async {
            let! received = inbox.Receive ()
            match received with
            | GetAll(id, replyChannel) -> 
                printfn "Getting events for %A" id
                events 
                |> List.filter (fun (personId,_) -> personId = id)
                |> List.map (fun (_,event) -> event)
                |> List.rev
                |> replyChannel.Reply
                return! loop events
            | Event(id,event) -> 
                printfn "Saving %A for %A" event id 
                return! loop ((id,event) :: events)
            | _ ->
                return! loop events
        }
    loop [])
eventStore.Start ()

let save id event = (id,event) |> Event |> eventStore.Post
let whatHappenedTo id = eventStore.PostAndReply(fun channel ->
    GetAll (id, channel))

//Decision making part of app
type state = { Name: string }

let emptyState = { Name = "" }

let getState id = 
    whatHappenedTo id 
    |> List.fold (fun state event -> 
        let newState = 
            match event with
            | Named(name)             -> {state with Name = name}
            | ChangedName(name)       -> {state with Name = name}
            | _                       -> state
        printfn "new state: %A" newState
        newState) emptyState
       
let registerBirth id date = 
    let state = getState id
    if (state <> emptyState) then failwith "People can not be born twice"
    save id (Born date)

let registerName id name =
    let state = getState id
    if (state = emptyState) then failwith "Can not name a person not yet born"
    if (not (System.String.IsNullOrEmpty state.Name)) then failwith "Person already named, use change name"
    save id (Named name)

let changeName id name = 
    let state = getState id
    if (state = emptyState) then failwith "Can not change the name of a person not yet born"
    if (System.String.IsNullOrEmpty(state.Name)) then failwith "Can not change the name of a person who has no name"
    save id (ChangedName name)
    