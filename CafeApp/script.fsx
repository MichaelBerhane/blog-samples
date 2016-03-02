#r "packages/Chessie/lib/net40/Chessie.dll"
#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#load "Domain.fs"
#load "Commands.fs"
#load "Events.fs"
#load "Aggregates.fs"
#load "Errors.fs"
#load "Handlers.fs"
#load "DomainExtensions.fs"

open Domain
open Events
open Aggregates
open Errors
open Handlers
open Commands
open System
open Chessie.ErrorHandling
open FSharp.Data
open DomainExtensions

let coke = DrinksItem {
    MenuNumber = 10
    Price = 2.5m
    Name = "Coke"
}
let salad = FoodItem {
  MenuNumber = 8
  Price = 5m
  Name = "Salad"
}
let pizza = FoodItem {
  MenuNumber = 9
  Price = 10m
  Name = "Pizza"
}
let lemonade = DrinksItem {
  MenuNumber = 11
  Name = "Lemonade"
  Price = 1.5m
}
let tab = {
  Id = System.Guid.NewGuid()
  TableNumber = 1
}

let serveCoke = ServeDrinks (coke, tab.Id)
let serveSalad = ServeFood (salad, tab.Id)
let prepareSalad = PrepareFood (salad, tab.Id)
let serveLemonade = ServeDrinks (lemonade, tab.Id)
let servePizza = ServeFood (pizza, tab.Id)
let preparePizza = PrepareFood (pizza, tab.Id)
let lift f m cmd =
  match m with
  | Ok((state,event),_) ->
    f state cmd
  | Bad errors -> errors |> List.head |> fail

let result initalState commands =
  let x, xs =
    match commands with
    | [] -> OpenTab tab, []
    | x::xs -> x,xs
  let stateM = evolve initalState x
  xs |> List.fold (lift evolve) stateM

let placeOrder = PlaceOrder {
  FoodItems = [salad;pizza]
  DrinksItems = [coke]
  TabId = tab.Id
}
let closeTab = {Tab = tab; Amount = 17.5m} |> CloseTab
let commands =
  [ OpenTab tab]
    //placeOrder
    //serveCoke
    //prepareSalad
    //preparePizza
    //serveSalad
    //servePizza
    //closeTab]
let r = result (ClosedTab None) commands
let state,_ = returnOrFail r