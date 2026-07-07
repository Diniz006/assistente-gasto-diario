# Assistente de Gasto Diario

MVP de assistente financeiro pessoal para responder uma pergunta simples: quanto posso gastar hoje sem comprometer contas fixas, despesas, renda e metas?

## Status atual

- Backend em ASP.NET Core 8.
- Banco PostgreSQL local.
- Autenticacao JWT.
- Swagger habilitado.
- Frontend estatico servido pela propria API.
- Fluxos ja disponiveis na tela:
  - criar conta e login;
  - configurar renda mensal e ciclo;
  - ver dashboard com limite diario seguro;
  - lancar despesa rapidamente;
  - ver historico de despesas do ciclo;
  - cadastrar e listar contas fixas.

## Estrutura

```txt
src/
  AssistenteGastoDiario.Api/             API, Swagger e frontend estatico
  AssistenteGastoDiario.Application/     DTOs, interfaces e servicos de regra
  AssistenteGastoDiario.Domain/          Entidades e enums de dominio
  AssistenteGastoDiario.Infrastructure/  EF Core, PostgreSQL, auth e servicos
docs/
  GITHUB-PUBLISH.md                      Guia para publicar no GitHub
```

## Requisitos

- PostgreSQL rodando localmente.
- Banco `assistente_gasto_diario` criado.
- .NET SDK 8 instalado.

Configuracao padrao do banco:

```txt
Host=localhost
Port=5432
Database=assistente_gasto_diario
Username=postgres
Password=postgres
```

## Como rodar

Na pasta do projeto:

```powershell
.\start-api-local.cmd
```

Depois abra:

- App: http://localhost:5088/
- Swagger: http://localhost:5088/swagger/index.html
- Health: http://localhost:5088/health

Se preferir rodar no terminal visivel:

```powershell
.\run-api-local.ps1
```

## Migrations

As migrations ja existem no projeto. Para aplicar no banco:

```powershell
$env:DOTNET_ROLL_FORWARD='Major'
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef.exe" database update --project src/AssistenteGastoDiario.Infrastructure --startup-project src/AssistenteGastoDiario.Api
```

Se estiver usando o SDK global instalado em `C:\Program Files\dotnet`, o projeto tambem possui `global.json` apontando para o SDK 8.

## Fluxo de teste manual

1. Acesse `http://localhost:5088/`.
2. Crie uma conta.
3. Configure renda mensal e ciclo.
4. Cadastre uma conta fixa.
5. Lance uma despesa rapida.
6. Confira o limite diario e o historico atualizados.

## GitHub

Para publicar este projeto no GitHub, siga o guia:

[docs/GITHUB-PUBLISH.md](docs/GITHUB-PUBLISH.md)

O workflow `.github/workflows/dotnet-build.yml` roda restore e build em pull requests e pushes para `main`.

