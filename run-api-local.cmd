@echo off
set DOTNET_ROLL_FORWARD=Major
set DOTNET_CLI_HOME=C:\Users\dinic\Documents\App financeiro\.dotnet-cli-home
cd /d "C:\Users\dinic\Documents\App financeiro"
"C:\Program Files\dotnet\dotnet.exe" run --project "C:\Users\dinic\Documents\App financeiro\src\AssistenteGastoDiario.Api\AssistenteGastoDiario.Api.csproj" --no-build --urls http://localhost:5088
