module TensaiMakoto.Web.BlazorClient.Services

open Bolero.Remoting
open TensaiMakoto.App.Model
open Common
open System

type public SlideService =
    {
        getSlides : string -> Async<Slide list>
    }
    interface IRemoteService with
        member __.BasePath = "/slides"