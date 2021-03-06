﻿using LibraryApi.Controllers;
using LibraryApi.Domain;
using LibraryApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace LibraryApiIntegrationTests
{
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's ApplicationDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<LibraryDataContext>));

                var employeeIdGeneratorDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IGenerateEmployeeIds)
                    );

                var lookupOnCallDeveloperDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ILookupOnCallDevelopers));

                if(lookupOnCallDeveloperDescriptor != null)
                {
                    services.Remove(lookupOnCallDeveloperDescriptor);
                }

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                if (employeeIdGeneratorDescriptor != null)
                {
                    services.Remove(employeeIdGeneratorDescriptor);
                }
                services.AddTransient<ILookupOnCallDevelopers, FakeTeamsDeveloperLookup>();
                services.AddTransient<IGenerateEmployeeIds, TestingEmployeeIdGenerator>();
                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<LibraryDataContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<LibraryDataContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                        // Utilities.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }
}