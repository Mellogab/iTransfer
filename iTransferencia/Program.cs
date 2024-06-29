using iTransferencia.Core;
using iTransferencia.Core.Entities.Adapters;
using iTransferencia.Core.Patterns;
using iTransferencia.Core.Repository;
using iTransferencia.Core.Services;
using iTransferencia.Core.UseCases.Accounts.UpdateBalances;
using iTransferencia.Core.UseCases.Bacen.NotifyBacen;
using iTransferencia.Core.UseCases.Transfers.ExecuteTransfer;
using iTransferencia.Core.UseCases.Transfers.GetAllTransfers;
using iTransferencia.Infrastructure.AutoMapper;
using iTransferencia.Infrastructure.DbContext;
using iTransferencia.Infrastructure.EntityFrameworkDataAccess.Repositories;
using iTransferencia.Infrastructure.Patterns.Sagas;
using iTransferencia.Infrastructure.Services;
using iTransferencia.Presenters;
using Microsoft.EntityFrameworkCore;
using iTransferencia.Infrastructure.Configurations;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
IConfigurationRoot configuration = configurationBuilder.Build();

/**
 * 1. Database Configuration Injections
 * **/

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = configuration["ConnectionStrings:SqlServer"];
    options.UseSqlServer(connectionString);
});

/**
 * 2. Repositories Injections
 * **/

builder.Services.AddTransient<ITransferRepository, TransferRepository>();
builder.Services.AddTransient<IIdempotenceRepository, IdempotenceRepository>();

/**
 * 3. UseCase Injections
 * **/

builder.Services.AddTransient<IGetAllTransfers, GetAllTransfersUseCase>();
builder.Services.AddTransient<INotifyBacenUseCase, NotifyBacenUseCase>();
builder.Services.AddTransient<IExecuteTransferUseCase, ExecuteTransferUseCase>();
builder.Services.AddTransient<IUpdateBalancesUseCase, UpdateBalancesUseCase>();

builder.Services.AddTransient<iTransferencia.Core.Presenters.DefaultPresenter<UseCaseResponseMessage>>();
builder.Services.AddTransient(typeof(iTransferencia.Core.Presenters.DefaultPresenter<>), typeof(iTransferencia.Core.Presenters.DefaultPresenter<>));
builder.Services.AddTransient<DefaultPresenter<UseCaseResponseMessage>>();
builder.Services.AddTransient(typeof(DefaultPresenter<>), typeof(DefaultPresenter<>));

/**
 * 4. ClientsAPIs Injections
 * **/

builder.Services.AddSingleton(builder.Configuration.GetSection("ClientsAPI:BacenAPI").Get<BacenAPI>());
builder.Services.AddSingleton(builder.Configuration.GetSection("ClientsAPI:ClientAPI").Get<ClientAPI>());
builder.Services.AddSingleton(builder.Configuration.GetSection("ClientsAPI:AccountAPI").Get<AccountAPI>());
builder.Services.AddSingleton(builder.Configuration.GetSection("ClientsAPI:TransferAPI").Get<TransferAPI>());
builder.Services.AddSingleton(builder.Configuration.GetSection("ClientsAPI").Get<ClientsAPI>());
builder.Services.AddSingleton(builder.Configuration.GetSection("CircuitBreakerPolicySettings").Get<CircuitBreaker>());
builder.Services.AddSingleton(builder.Configuration.GetSection("RetryPolicySettings").Get<Retry>());

/**
 * 5. Services Injections
 * **/
builder.Services.AddSingleton<IHttpRequestService, HttpRequestService>();
builder.Services.AddSingleton<IClientService, iTransferencia.Infrastructure.Services.ClientService>();
builder.Services.AddSingleton<IAccountService, iTransferencia.Infrastructure.Services.AccountService>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

builder.Services.AddMemoryCache();

/**
 * 6. AutoMapper
 * **/

builder.Services.AddAutoMapper(mapper =>
{
    mapper.AddProfile<MappingProfile>();
});

/**
 * 8. Patterns
 * **/

builder.Services.AddSingleton<ISagaOrquestrator, SagaOrquestrator>();

/**
 * 8. Configure logs for application
 * **/
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();