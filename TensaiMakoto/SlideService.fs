namespace TensaiMakoto

open TensaiMakoto.App
open TensaiMakoto.App.Model
open System
open System.Threading.Tasks
open System.Collections.Generic
open System.Linq

type SlidesReadOnlyRepo () =

    interface IReadOnlyRepo<SlideCategory> with
        member _.Queryable =
            let slideList = [
                { Number = 1; Data = Text "一"}
                { Number = 2; Data = Text "二"}
                { Number = 3; Data = Text "三"}
                { Number = 4; Data = Text "四"}
                { Number = 5; Data = Text "五"}
                { Number = 6; Data = Text "六"}
                { Number = 7; Data = Text "七"}
                { Number = 8; Data = Text "八"}
                { Number = 9; Data = Text "九"}
                { Number = 10; Data = Text "十"}
            ]
            let category = { Name = "Default"; Slides = slideList}
            [category].AsQueryable()

        member _.ToListAsync query =
            query.ToList() :> IReadOnlyList<_> |> Task.FromResult


