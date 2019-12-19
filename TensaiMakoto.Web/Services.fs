module Services
open Bolero.Remoting.Server
open TensaiMakoto.Web
open Microsoft.AspNetCore.Hosting
open TensaiMakoto.App
open TensaiMakoto.App.Model
open JWT.Builder
open JWT
open System.Security.Cryptography
open JWT.Algorithms
open System.Collections.Generic
open TensaiMakoto.Web.BlazorClient.Main

type public SlideService(ctx: IRemoteContext) =
        inherit RemoteHandler<BlazorClient.Services.SlideService>()

        member private _.GetService<'T>() : 'T =
            downcast ctx.HttpContext.RequestServices.GetService(typeof<'T>)

        member private this.GetItems<'T>() =
                let repo = this.GetService<IReadOnlyRepo<'T>>()
                async {
                      let! b =
                          repo.Queryable
                          |> repo.ToListAsync
                          |> Async.AwaitTask
                      return b |> List.ofSeq
                }
        override this.Handler = {
            getSlides =
                fun name -> async{
                    let! categories = this.GetItems<SlideCategory>()
                    return
                        (categories |> Seq.tryFind( fun c -> c.Name = name)).Value.Slides
                }
        }
