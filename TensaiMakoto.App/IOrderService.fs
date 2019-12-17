namespace  TensaiMakoto.App

open System.Threading.Tasks
open TensaiMakoto.App.Model
open System

type IOrderService =
    abstract member PlaceOrder : order : Order -> Task<Result<string,string>>