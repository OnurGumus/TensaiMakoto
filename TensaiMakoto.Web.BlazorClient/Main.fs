module TensaiMakoto.Web.BlazorClient.Main

open Microsoft.AspNetCore.Components.Routing
open Microsoft.JSInterop
open Bolero.Remoting
open Elmish
open Services
open Bolero
open Newtonsoft.Json
open TensaiMakoto.App.Model
open System
open System.Threading
open System.Collections.Concurrent

let mutable currentSlide = 1

type Model = {
    IsInstructor : bool
    CurrentSlide : int
    IsSync : bool
}

and Message=
    | SlideIncreaseRequested
    | SlideDecreaseRequested
    | SlideDecreased
    | SlideIncreased
    | SlidePushed

let dispatchers  = ConcurrentDictionary<(Message -> unit),unit>()


let init isInstructor =
    {
        IsInstructor = isInstructor
        CurrentSlide = currentSlide
        IsSync = true
    }, Cmd.none



let update remote jsRuntime message model =
    let genericUpdate update subModel msg  msgFn pageFn =
        let subModel, cmd = update  msg subModel
        model, Cmd.map msgFn cmd


    match message, model with
    | SlideIncreaseRequested , _->
        Interlocked.Increment(&currentSlide) |> ignore
        for (d:Message -> unit) in dispatchers.Keys do
            d(SlidePushed)
        model,Cmd.none

    | SlideDecreaseRequested, _ ->
        Interlocked.Decrement(&currentSlide) |> ignore
        for (d:Message -> unit) in dispatchers.Keys do
            d(SlidePushed)
        model,Cmd.none

    | SlideIncreased , _->
        { model with CurrentSlide = model.CurrentSlide + 1 } , Cmd.none

    | SlideDecreased, _ ->
        {model with CurrentSlide = model.CurrentSlide - 1}, Cmd.none

    | SlidePushed, _ ->
        {model with CurrentSlide = currentSlide}, Cmd.none

open Bolero.Html
open BoleroHelpers

type MainLayout = Template<"wwwroot\MainLayout.html">


let view  (js: IJSRuntime) ( model : Model) dispatch =

    let content = text <| model.CurrentSlide.ToString()
    let leftClick, rightClick =
        if model.IsInstructor then
            SlideDecreaseRequested, SlideIncreaseRequested
        else
            SlideDecreased, SlideIncreased


    MainLayout()
        .OnLeftClick(fun _ -> dispatch leftClick)
        .OnRightClick(fun _ -> dispatch rightClick)
        .Body(content)
        .Elt()

open System
open Bolero.Templating.Client

open Microsoft.AspNetCore.Components

type MyApp() =
    inherit ProgramComponent<Model, Message>()
    let  dispatch =
        typeof<MyApp>
            .GetProperty("Dispatch", Reflection.BindingFlags.Instance ||| Reflection.BindingFlags.NonPublic)

    let mutable dispatcher: (Message -> unit) = Unchecked.defaultof<_>
    static member  Dispatchers = dispatchers

    interface IDisposable with
          member __.Dispose() =
            if dispatcher |> box |> isNull |> not then
                MyApp.Dispatchers.TryRemove(dispatcher) |> ignore

    [<Parameter>]
    member val InstructorMode : bool = false with get, set

    override this.OnAfterRenderAsync(firstRender) =
        let res = base.OnAfterRenderAsync(firstRender) |> Async.AwaitTask
        async{
            do! res
            if firstRender then
                dispatcher <- downcast dispatch.GetValue(this)
                MyApp.Dispatchers.TryAdd(dispatcher,()) |> ignore
            return ()
         }|> Async.StartImmediateAsTask :> _

    override this.Program =
        let remote = this.Remote<PizzaService>()
        let update = update remote (this.JSRuntime)
        Program.mkProgram (fun _ -> init this.InstructorMode) (update) (view this.JSRuntime)
#if DEBUG
        |> Program.withTrace(fun msg model -> System.Console.WriteLine(msg : Message))
        |> Program.withConsoleTrace
        |> Program.withErrorHandler
            (fun (x,y) ->
                Console.WriteLine("Error Message:" + x)
                Console.WriteLine("Exception:" + y.ToString()))

        |> Program.withHotReload
#endif

open Microsoft.AspNetCore.Components.Builder
open Microsoft.Extensions.DependencyInjection
open Bolero.Remoting.Client
type Startup() =
    member __.ConfigureServices(services: IServiceCollection) =
        services.AddRemoting()

    member __.Configure(app: IComponentsApplicationBuilder) =
        app.AddComponent<MyApp>("app")
