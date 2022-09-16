# NetLah.Extensions.SpaServices.Hosting - .NET Library

[NetLah.Extensions.SpaServices.Hosting](https://www.nuget.org/packages/NetLah.Extensions.SpaServices.Hosting/) is a library support setting ASP.NET Core SpaServices hosting from configuration.

## Nuget package

[![NuGet](https://img.shields.io/nuget/v/NetLah.Extensions.SpaServices.Hosting.svg?style=flat-square&label=nuget&colorB=00b200)](https://www.nuget.org/packages/NetLah.Extensions.SpaServices.Hosting/)

## Build Status

[![ASP.NETCore 6](https://github.com/NetLah/spaservices-hosting/actions/workflows/aspnet-core.yml/badge.svg)](https://github.com/NetLah/spaservices-hosting/actions/workflows/aspnet-core.yml)

## Getting started

### 1. Add/Update PackageReference to web .csproj

```
<ItemGroup>
  <PackageReference Include="NetLah.Extensions.SpaServices.Hosting" />
</ItemGroup>
```

### 2. Use SPA Hosting

```csharp
builder.AddSpaApp();
...
var app = builder.Build();

app.UseSpaApp(action: app => app.UseSerilogRequestLoggingLevel());

app.Run();
```
