using Microsoft.EntityFrameworkCore;
using MyFirstApp.Data;
using MyFirstApp.Models;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// app.Map("map1",async (context)=>
// {
//    await context.Response.WriteAsync("In Map1");
// });

// app.Map("map2",async(context)=>
// {
//     await context.Response.WriteAsync("In map2 hey hey hey ");
// });

app.MapPost("/addproduct", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);

    await db.SaveChangesAsync();

    return Results.Ok(product);
});

app.Map("/allproducts", async (AppDbContext db, HttpContext context) =>
{
    var products = await db.Products.ToListAsync();

    await context.Response.WriteAsJsonAsync(products);
});

app.MapGet("/product/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound("Product not found");
    }

    return Results.Ok(product);
});

//Update product
app.MapPut("/product/{id}", async (int id, Product updatedProduct, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound("Product not found");
    }

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;

    await db.SaveChangesAsync();

    return Results.Ok(product);
});

app.MapDelete("/product/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound("Product not found");
    }

    db.Products.Remove(product);

    await db.SaveChangesAsync();

    return Results.Ok("Product deleted");
});

app.Run();
