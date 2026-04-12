using SistemaTarjetas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Registrar servicios
builder.Services.AddScoped<IAutenticadorService, AutenticadorService>();
builder.Services.AddScoped<IAutorizadorService, AutorizadorService>();  // ← Agrega esta línea

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "movimientosCuenta",
    pattern: "MovimientosCuenta/{action}/{numeroCuenta?}",
    defaults: new { controller = "MovimientosCuenta", action = "MovimientosCuenta" });


app.Run();