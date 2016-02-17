(**
- title : FsReveal 
- description : Introduction to FsReveal
- author : Karlkim Suwanmongkol
- theme : Sky
- transition : default

***

### Eventsourcing

> The story of your state.

***

### Vidar Løvbrekke Sømme

> A jack of many trades at Vetserve AS

****

[kitten?]

***

### Everything is a series of events

*)
let person = {
  dob: System.DateTime 
  name: string
  address: string
  phone: string
}
(**

***

### Because everything has a story

*)
let person = [
  Born('01-01-1990');
  Named('John Smith');
  MovedTo('1st Mystreet, MyCity', Reason "Childhood home");
  MovedTo('23rd NewStreet, MyCity', Reason "First place of my own");
  GotMobilePhone('11223344');
  ChangedPhoneNo('22334455');
  ChangedName('Karl Nilsso', Reason "Witness protection");
]
(**

 
***

### Storing current state is lossy compression

***

> * But you don't know the whole story!
* "Det er historieløst" (It's historyless)

***

### A ledger of everything

Accountants have been doing eventsourcing for centuries (or maybe millenia)

***

### Demo time

***

### Event all the things

***

Same state change can be represented with different event types, capturing context and intent.

> Address corrected vs customer moved.

***


demo time

***

intermission

***




[Draft plan, not to be published]:

* Introducing myself
* A very-short-explanation and some historic / non-it examples.
* A simple demo illustrating ES in its simplest form
* Pros Cons and possibilities
* Extending demo to show some of the possibilites in practice.

[Break]

* Clearing up some common mixups with acronyms (CQRS,DDD,ES, SAGA, BUS, and the list goes on..).
* Tooling (or lack of), plumbing and admin systems.
* Cons and tradeoffs
* Explaining why I decided to use Eventsourcing for some projects.
* Why I succeeded
* And why I kind of regret doing it.


### What is FsReveal?

- Generates [reveal.js](http://lab.hakim.se/reveal-js/#/) presentation from [markdown](http://daringfireball.net/projects/markdown/)
- Utilizes [FSharp.Formatting](https://github.com/tpetricek/FSharp.Formatting) for markdown parsing

***

### Reveal.js

- A framework for easily creating beautiful presentations using HTML.  
  
> **Atwood's Law**: any application that can be written in JavaScript, will eventually be written in JavaScript.

***

### FSharp.Formatting

- F# tools for generating documentation (Markdown processor and F# code formatter).
- It parses markdown and F# script file and generates HTML or PDF.
- Code syntax highlighting support.
- It also evaluates your F# code and produce tooltips.

***

### Syntax Highlighting

#### F# (with tooltips)

*)
let a = 5
let factorial x = [1..x] |> List.reduce (*)
let c = factorial a
(** 
`c` is evaluated for you
*)
(*** include-value: c ***)
(**

--- 

#### More F#

*)
[<Measure>] type sqft
[<Measure>] type dollar
let sizes = [|1700<sqft>;2100<sqft>;1900<sqft>;1300<sqft>|]
let prices = [|53000<dollar>;44000<dollar>;59000<dollar>;82000<dollar>|] 
(**

#### `prices.[0]/sizes.[0]`

*)
(*** include-value: prices.[0]/sizes.[0] ***)
(**

---

#### C#

    [lang=cs]
    using System;


    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello, world!");
        }
    }


---

#### JavaScript

    [lang=js]
    function copyWithEvaluation(iElem, elem) {
      return function (obj) {
          var newObj = {};
          for (var p in obj) {
              var v = obj[p];
              if (typeof v === "function") {
                  v = v(iElem, elem);
              }
              newObj[p] = v;
          }
          if (!newObj.exactTiming) {
              newObj.delay += exports._libraryDelay;
          }
          return newObj;
      };
    }

---

#### Haskell
 
    [lang=haskell]
    recur_count k = 1 : 1 : zipWith recurAdd (recur_count k) (tail (recur_count k))
            where recurAdd x y = k * x + y

    main = do
      argv <- getArgs
      inputFile <- openFile (head argv) ReadMode
      line <- hGetLine inputFile
      let [n,k] = map read (words line)
      printf "%d\n" ((recur_count k) !! (n-1))


*code from [NashFP/rosalind](https://github.com/NashFP/rosalind/blob/master/mark_wutka%2Bhaskell/FIB/fib_ziplist.hs)*

---

### SQL
 
    [lang=sql]
    select * 
    from 
      (select 1 as Id union all select 2 union all select 3) as X 
    where Id in (@Ids1, @Ids2, @Ids3)

*sql from [Dapper](https://code.google.com/p/dapper-dot-net/)* 

***

**Bayes' Rule in LaTeX**

$ \Pr(A|B)=\frac{\Pr(B|A)\Pr(A)}{\Pr(B|A)\Pr(A)+\Pr(B|\neg A)\Pr(\neg A)} $

***

### The Reality of a Developer's Life 

**When I show my boss that I've fixed a bug:**
  
![When I show my boss that I've fixed a bug](http://www.topito.com/wp-content/uploads/2013/01/code-07.gif)
  
**When your regular expression returns what you expect:**
  
![When your regular expression returns what you expect](http://www.topito.com/wp-content/uploads/2013/01/code-03.gif)
  
*from [The Reality of a Developer's Life - in GIFs, Of Course](http://server.dzone.com/articles/reality-developers-life-gifs)*

*)
