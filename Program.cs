using System;
using System.Collections.Generic;
using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Extensions;
using Aquila360.Attendance.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config =>
    {
        config
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    })
    .ConfigureServices(services =>
    {
        var exclusions = new List<Type>
        {
            typeof(CosmosService<>)
        };

        services.RegisterAllTypesAsScopedServicesInNamespaceAs<AttendanceByEmployeeCosmosService>(exclusions);
    })
    .Build();

host.Run();
