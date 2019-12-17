﻿namespace TensaiMakoto

open System.Runtime.CompilerServices
open Microsoft.Extensions.DependencyInjection
open TensaiMakoto.App
open TensaiMakoto.App.Model

[<Extension>]
type EFExtensions() =
    [<Extension>]
    static member inline SetupServices(services: IServiceCollection) = 
        services
            .AddScoped<IOrderService,OrderService>()
            .AddScoped<OrderService>(fun p -> downcast p.GetService<IOrderService>())
            .AddScoped<IReadOnlyRepo<Order>,OrderReadOnlyRepo>()
