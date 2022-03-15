using NotReksaChat.Hubs;
using NotReksaChat.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
builder.Services.AddSignalR();
builder.Services.AddTransient<Online>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();
