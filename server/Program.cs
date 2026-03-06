using Pinecone;
using server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenAIEmbeddingGenerator(
    modelId: "text-embedding-3-small",
    builder.Configuration["OPENAI_API_KEY"]!);

builder.Services.AddSingleton<PineconeClient>(sp =>
    new PineconeClient(builder.Configuration["PINECONE_API_KEY"]!));

builder.Services.AddScoped<IAuditParserService, AuditParserService>();
builder.Services.AddScoped<IFlowchartParserService, FlowchartParserService>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();