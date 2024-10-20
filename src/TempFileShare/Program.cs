using TempFileShare.Extension;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreConfigurationServices(builder.Configuration);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
