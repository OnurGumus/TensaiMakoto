namespace TensaiMakoto.EF
open Microsoft.EntityFrameworkCore
open TensaiMakoto.App.Model

type PizzaStoreContext =
    inherit DbContext

    [<DefaultValue>]
    val mutable private specials : DbSet<PizzaSpecial>
    [<DefaultValue>]
    val mutable private toppings : DbSet<Topping>
    new () = {}
    new (options : DbContextOptions) = { inherit DbContext(options) }

    member this.Specials
        with get() = this.specials
        and private set v = this.specials <- v

    member this.Toppings
        with get() = this.toppings
        and private set v = this.toppings <- v