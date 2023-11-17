using SignalRMVCApp.SignalRHub;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors( policy => policy.WithOrigins("http://localhost:4200"). AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseRouting();

app.UseAuthorization();

app.MapHub<ChatHub>("/chat-hub");
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
