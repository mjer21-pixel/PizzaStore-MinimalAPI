using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<PizzaDb>(opts => opts.UseInMemoryDatabase("PizzaDb"));
builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pizza Store API",
        Description = "Making the Pizzas you love",
        Version = "v1"
    });
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Pizza Store API");
    });
}

//GET all Pizzas
app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());

//ADD a new Pizza
app.MapPost("/pizza/{id}", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

//Find a Pizza by ID
app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id));

//Update a Pizza by ID
app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatePizza, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if(pizza != null)
    {
        pizza.Name = updatePizza.Name;
        pizza.Description = updatePizza.Description;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    else return Results.NotFound();    
});

//Delete a Pizza by ID
app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if(pizza != null)
    {
        db.Pizzas.Remove(pizza);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    else return Results.NotFound();
});
app.Run();
