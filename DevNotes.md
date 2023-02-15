# Developer Notes

## Overview
- TBD

## Todo
- TBD

## Future and Backlogs
- TBD

## Other

### dotnet pack solution error NETSDK1194

Bug with new SDK 7.0.200, error NETSDK1194: The "--output" option isn't supported when building a solution

Resolution: change to dotnet pack C# csproj

```
Run dotnet pack -c Release -o ./artifacts/packages/ --no-build --verbosity normal
MSBuild version 17.5.0-preview-23061-01+040e2a90e for .NET
Build started 2/15/2023 6:37:31 AM.
     1>Project "/Users/runner/work/spaservices-hosting/spaservices-hosting/NetLah.SpaServices.sln" on node 1 (pack target(s)).
     1>ValidateSolutionConfiguration:
         Building solution configuration "Release|Any CPU".
     1>/Users/runner/.dotnet/sdk/7.0.200/Current/SolutionFile/ImportAfter/Microsoft.NET.Sdk.Solution.targets(36,5): error NETSDK1194: The "--output" option isn't supported when building a solution. [/Users/runner/work/spaservices-hosting/spaservices-hosting/NetLah.SpaServices.sln]
     1>Done Building Project "/Users/runner/work/spaservices-hosting/spaservices-hosting/NetLah.SpaServices.sln" (pack target(s)) -- FAILED.

Build FAILED.

       "/Users/runner/work/spaservices-hosting/spaservices-hosting/NetLah.SpaServices.sln" (pack target) (1) ->
       (_CheckForSolutionLevelOutputPath target) -> 
         /Users/runner/.dotnet/sdk/7.0.200/Current/SolutionFile/ImportAfter/Microsoft.NET.Sdk.Solution.targets(36,5): error NETSDK1194: The "--output" option isn't supported when building a solution. [/Users/runner/work/spaservices-hosting/spaservices-hosting/NetLah.SpaServices.sln]
```

