using System.Text.Json;
using W2K.Common.Extensions;
using W2K.Common.Identity;
using W2K.Common.Utils;
using W2K.Common.ValueObjects;
using W2K.Identity.Entities;
using W2K.Identity.Enums;
using W2K.Identity.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace W2K.Identity.Persistence.Context;

public class IdentityDbSeed
{
    private List<Permission> _permissions = [];

    public async Task SeedAsync(IIdentityUnitOfWork data, ILogger logger)
    {
        var pipeline = CreatePipeline(logger, nameof(IdentityDbSeed));
        await pipeline.ExecuteAsync(async x =>
            {
                await SeedOfficesAsync(data);
                await SeedPermissionsAsync(data);
                await SeedRolesAsync(data);
            });
    }

    private static ResiliencePipeline CreatePipeline(ILogger logger, string prefix, int retries = 3, int delaySeconds = 5)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<SqlException>(),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,  // Adds a random factor to the delay
                MaxRetryAttempts = retries,
                Delay = TimeSpan.FromSeconds(delaySeconds),
                OnRetry = x =>
                {
                    logger.LogWarning(
                        x.Outcome.Exception,
                        "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Retries}",
                        prefix,
                        x.Outcome.Exception?.GetType().Name,
                        x.Outcome.Exception?.Message,
                        x.AttemptNumber,
                        retries);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    private static async Task SeedOfficesAsync(IIdentityUnitOfWork data)
    {
        bool saveChanges = false;
        var path = Path.Combine(AppContext.BaseDirectory, "Seed", "Identity", "offices.json");
        if (!string.IsNullOrEmpty(path))
        {
            var offices = ParseOfficesFromJson(path);
            if (offices is not null)
            {
                foreach (var office in offices)
                {
                    var exists = await data.Offices.AnyAsync(x => x.Code == office.Code);
                    if (!exists)
                    {
                        saveChanges = true;
                        data.Offices.Add(office);
                    }
                }
            }
        }
        if (saveChanges)
        {
            _ = await data.SaveEntitiesAsync();
        }
    }

    private static List<Office> ParseOfficesFromJson(string path)
    {
        var offices = new List<Office>();
        var officeElements = JsonUtil.ParseJsonFile<JsonElement>(path);
        foreach (var officeElement in officeElements.EnumerateArray())
        {
            var officeType = officeElement.GetPropertyValue<OfficeType>("Type");
            var code = officeElement.GetPropertyValue<string>("Code");
            var category = officeElement.GetPropertyValue<string>("Category");
            var name = officeElement.GetPropertyValue<string>("Name");
            var legalName = officeElement.GetPropertyValue<string>("LegalName");
            var phone = officeElement.GetPropertyValue<string>("Phone");
            var website = officeElement.GetPropertyValue<string>("Website");
            var taxId = officeElement.GetPropertyValue<string>("TaxId");
            var businessType = officeElement.GetPropertyValue<string>("BusinessType");
            var isEnrollmentCompleted = officeElement.GetPropertyValue<bool>("IsEnrollmentCompleted");
            var isReviewed = officeElement.GetPropertyValue<bool>("IsReviewed");
            var isApproved = officeElement.GetPropertyValue<bool>("IsApproved");
            var addressElement = officeElement.GetProperty("Address");
            var addressType = addressElement.GetPropertyValue<string>("Type");
            var address1 = addressElement.GetPropertyValue<string>("Address1");
            var address2 = addressElement.GetPropertyValue<string>("Address2");
            var city = addressElement.GetPropertyValue<string>("City");
            var state = addressElement.GetPropertyValue<string>("State");
            var zipCode = addressElement.GetPropertyValue<string>("ZipCode");
            var address = new Address(new Address.AddressInfo
            {
                Type = addressType,
                Address1 = address1,
                Address2 = address2,
                City = city,
                State = state,
                ZipCode = zipCode
            });

            var office = new Office(new Office.OfficeInfo
            {
                Type = officeType,
                Category = category,
                Code = code,
                Name = name,
                LegalName = legalName,
                Phone = phone,
                Website = website,
                TaxId = taxId,
                BusinessType = businessType,
                Address = address,
            });
            office.SetEnrollment(isEnrollmentCompleted);
            office.SetReviewApproved(isReviewed, isApproved);
            offices.Add(office);
        }
        return offices;
    }

    private async Task SeedPermissionsAsync(IIdentityUnitOfWork data)
    {
        bool saveChanges = false;
        var path = Path.Combine(AppContext.BaseDirectory, "Seed", "Identity", "permissions.json");
        if (!string.IsNullOrEmpty(path))
        {
            _permissions = await data.Permissions.GetAllAsync();
            var permissions = ParsePermissionsFromJson(path);
            if (permissions is not null)
            {
                foreach (var permission in permissions.Where(x => !_permissions.Exists(p => p.Name == x.Name)))
                {
                    saveChanges = true;
                    _permissions.Add(permission);
                    data.Permissions.Add(permission);
                }
            }
        }
        if (saveChanges)
        {
            _ = await data.SaveEntitiesAsync();
        }
    }

    private static List<Permission> ParsePermissionsFromJson(string path)
    {
        var permissions = new List<Permission>();
        var elements = JsonUtil.ParseJsonFile<JsonElement>(path);
        foreach (var element in elements.EnumerateArray())
        {
            var name = element.GetPropertyValue<string>("Name");
            var category = element.GetPropertyValue<string>("Category");
            var description = element.GetPropertyValue<string>("Description");
            var type = element.GetPropertyValue<string>("Type");
            permissions.Add(new Permission(category!, Enum.TryParse<PermissionType>(type, true, out var value) ? value : PermissionType.Standard, name!, description));
        }
        return permissions;
    }

    private async Task SeedRolesAsync(IIdentityUnitOfWork data)
    {
        bool saveChanges = false;
        var path = Path.Combine(AppContext.BaseDirectory, "Seed", "Identity", "roles.json");
        if (!string.IsNullOrEmpty(path))
        {
            var roles = ParseRolesFromJson(path);
            var dbRoles = await data.Roles.Include(x => x.Permissions).GetAllAsync();
            _permissions = await data.Permissions.GetAllAsync();
            if (roles is not null && dbRoles is not null)
            {
                foreach (var role in roles)
                {
                    var dbRole = dbRoles.Find(x => x.Name == role.Name);
                    if (dbRole is null)
                    {
                        saveChanges = true;
                        data.Roles.Add(role);
                    }
                    else
                    {
                        saveChanges |= dbRole.AddPermissions(role.Permissions);
                    }
                }
            }
        }
        if (saveChanges)
        {
            _ = await data.SaveEntitiesAsync();
        }
    }

    private List<Role> ParseRolesFromJson(string path)
    {
        var roles = new List<Role>();
        var roleElements = JsonUtil.ParseJsonFile<JsonElement>(path);

        foreach (var roleElement in roleElements.EnumerateArray())
        {
            var roleType = roleElement.GetPropertyValue<RoleType>("Type");
            var officeType = roleElement.GetPropertyValue<OfficeType>("OfficeType");
            var name = roleElement.GetPropertyValue<string>("Name");
            var department = roleElement.GetPropertyValue<string>("Department");
            var description = roleElement.GetPropertyValue<string>("Description");

            if (name is not null)
            {
                var rolePermissions = new List<Permission>();
                var permissionsExist = roleElement.TryGetProperty("PermissionNames", out JsonElement permissionNames);
                if (permissionsExist)
                {
                    foreach (var permissionName in permissionNames.EnumerateArray())
                    {
                        var permission = _permissions.Find(x => x.Name == permissionName.ToString());
                        if (permission is not null)
                        {
                            rolePermissions.Add(permission);
                        }
                    }
                }
                var role = new Role(roleType, officeType, null, name, department, description, rolePermissions);
                roles.Add(role);
            }
        }
        return roles;
    }
}
