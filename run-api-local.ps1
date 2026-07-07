$env:DOTNET_ROLL_FORWARD = 'Major'
$env:DOTNET_CLI_HOME = 'C:\Users\dinic\Documents\App financeiro\.dotnet-cli-home'
Set-Location 'C:\Users\dinic\Documents\App financeiro'
& 'C:\Program Files\dotnet\dotnet.exe' run --project 'C:\Users\dinic\Documents\App financeiro\src\AssistenteGastoDiario.Api\AssistenteGastoDiario.Api.csproj' --no-build --urls http://localhost:5088
