﻿module TensaiMakoto.Web.BlazorClient.Services

open Bolero.Remoting
open TensaiMakoto.App.Model
open Common
open System

type public PizzaService =
    {
        getSpecials : unit -> Async<PizzaSpecial list>
        getToppings : unit -> Async<Topping list>
        getOrders : string -> Async<Order list>
        getOrderWithStatuses : string -> Async<OrderWithStatus list>
        getOrderWithStatus : string * string -> Async<OrderWithStatus option>
        placeOrder : string * Order -> Async<Result<string,string>>
        signIn : string * string -> Async<Result<Authentication,string>>
        renewToken : string -> Async<Result<Authentication,string>>
    }
    interface IRemoteService with
        member __.BasePath = "/pizzas"