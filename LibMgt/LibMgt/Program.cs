using LibMgt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


var app = builder.Build();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    );

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
