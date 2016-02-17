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
| Subscribe of (Id*Event->unit)

let eventStore = new MailboxProcessor<Message>(fun inbox ->
    let rec loop events subscribers = 
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
                return! loop events subscribers
            | Event(id,event) -> 
                printfn "Saving %A for %A" event id 
                subscribers |> List.iter (fun h -> h (id,event))
                return! loop ((id,event) :: events) subscribers
            | Subscribe(handler) -> 
                printfn "Subscriber added: %A" handler
                return! loop events (handler::subscribers)
        }
    loop [] [])
eventStore.Start ()

let save id event = (id,event) |> Event |> eventStore.Post
let whatHappenedTo id = eventStore.PostAndReply(fun channel ->
    GetAll (id, channel))

type state = {
    Dob: string
    Name: string
    Address:string}

let emptyState = {
    Dob = ""
    Name = ""
    Address = ""}

let getState id = 
    whatHappenedTo id 
    |> List.fold (fun state event -> 
        let newState = 
            match event with
            | Born(date)              -> {state with Dob = date}
            | Named(name)             -> {state with Name = name}
            | ChangedName(name)       -> {state with Name = name}
            | AddressChanged(address) -> {state with Address = address}
            | Moved(address)          -> {state with Address = address}
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

let readStore = new System.Collections.Generic.Dictionary<Id,string>()

let readStoreHandler (id,event) = 
    printfn "Handling %A for %A" event id
    match event with
    | Named(name)       -> readStore.Add(id,name)
    | ChangedName(name) -> readStore.[id] <- name
    | _                 -> ()


let query name = 
    let result = readStore |> Seq.tryFind (fun item -> item.Value.ToLower().Contains(name))
    match result with 
    | Some(item) -> item.Key
    | None       -> failwith (sprintf "No match for %A found" name)

eventStore.Post (Subscribe readStoreHandler)
    
let id1 = Id "1";;
registerBirth id1 "01.01.2013"
