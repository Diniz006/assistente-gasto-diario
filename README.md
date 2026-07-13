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
  - acompanhar um onboarding guiado dos primeiros passos;
  - configurar renda mensal e ciclo;
  - editar configuracoes financeiras pelo painel;
  - ver dashboard com limite diario seguro;
  - visualizar graficos do ciclo com entradas, dinheiro guardado e saidas;
  - visualizar gastos por categoria;
  - comparar resumo mensal com tendencia dos ultimos meses;
  - cadastrar, listar, editar e excluir rendas do ciclo;
  - criar, editar e ocultar categorias;
  - editar registros por modal interno no frontend;
  - lancar despesa rapidamente com data, categoria, pagamento e observacao;
  - ver historico de despesas do ciclo;
  - filtrar e buscar despesas por periodo, texto, categoria e pagamento;
  - exportar despesas, rendas e resumo mensal em CSV;
  - cadastrar e listar contas fixas;
  - criar metas financeiras e registrar contribuicoes;
  - editar e excluir despesas, contas fixas e metas pelo frontend.

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
4. Cadastre uma renda do ciclo.
5. Cadastre uma conta fixa.
6. Lance uma despesa rapida.
7. Crie uma meta financeira.
8. Edite ou exclua algum item para conferir o painel recalculado.

## GitHub

Para publicar este projeto no GitHub, siga o guia:

[docs/GITHUB-PUBLISH.md](docs/GITHUB-PUBLISH.md)

O workflow `.github/workflows/dotnet-build.yml` roda restore e build em pull requests e pushes para `main`.

