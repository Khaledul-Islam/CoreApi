﻿using AutoMapper;
using Config.Mapper.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Config.Mapper.Example;

namespace ServiceExtensions.Mapper;

public static class AutoMapperRegistration
{
    public static void InitializeAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        List<Profile> profileList = new()
        {
            new AuthenticationProfiles(),
            new TestProfiles(),
        };

        services.AddAutoMapper(config =>
        {
            config.AddProfiles(profileList);
        }, assemblies);
    }
}