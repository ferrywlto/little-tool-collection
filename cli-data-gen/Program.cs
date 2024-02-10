using Cocona;

var builder = CoconaApp.CreateBuilder();
var app = builder.Build();

app.AddCommand("test", () => Console.WriteLine("test"));

app.Run();