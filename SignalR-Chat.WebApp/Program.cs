using NotReksaChat.Hubs;
using NotReksaChat.Services;
using NotReksaChat.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IOnline, Online>();
builder.Services.AddSingleton<IBanned, Banned>();

builder.Services.AddOptions<AdminSettings>().BindConfiguration(AdminSettings.AdminSettingsSection).ValidateDataAnnotations().ValidateOnStart();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();
