# COMMON REPO
- This is my library source.

## DOTNET
- Common library for dotnet.
- Create on : 2022-12-14.
- Create by : PhucNH

### PhucNH.Commons.Extensions
- Source code container for my dotnet extensions.
- Command line install report generator:
```
dotnet tool install -g dotnet-reportgenerator-globaltool
```

#### Source
- HashExtension : class static have method support hash a value, include Hash and HashMac (MD5, SHA1, SHA256, SHA384, SHA512).
- HttpExtension : class static have method support call api (GET, POST, PUT, DELETE, ...).
-Command line:
```
dotnet new classlib --framework netstandard2.0 --name PhucNH.Commons.Extensions
dotnet new classlib --framework netstandard2.0 --name PhucNH.Commons.Bases
```

#### Test
- Command line:
```
cd dotnet
dotnet add ./PhucNH.Commons.Extensions.Tests/PhucNH.Commons.Extensions.Tests.csproj reference ./PhucNH.Commons.Extensions/PhucNH.Commons.Extensions.csproj
dotnet add ./PhucNH.Commons.Extensions.Tests/PhucNH.Commons.Extensions.Tests.csproj reference ./PhucNH.Commons.Bases/PhucNH.Commons.Bases.csproj
dotnet test ./dotnet/PhucNH.Commons.Extensions.Tests --collect:"XPlat Code Coverage"
sh show_test_result.sh
```
- HashExtensionTest : class have unit test for class HashExtension.