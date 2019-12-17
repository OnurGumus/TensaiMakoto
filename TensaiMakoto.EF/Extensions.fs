namespace TensaiMakoto.EF

open System.Runtime.CompilerServices
open Microsoft.Extensions.DependencyInjection
open Microsoft.EntityFrameworkCore
open System
open TensaiMakoto.App
open TensaiMakoto.App.Model

[<Extension>]
type EFExtensions() =
    [<Extension>]
    static member inline AddEF(services: IServiceCollection, connString : string) =
        fun (options : DbContextOptionsBuilder) ->
            connString
            |> options.UseSqlite
            |> ignore
        |>  services.AddDbContext<PizzaStoreContext>
        |> ignore
        services.AddScoped(typedefof<IReadOnlyRepo<_>>, typedefof<ReadOnlyRepo<_>>)