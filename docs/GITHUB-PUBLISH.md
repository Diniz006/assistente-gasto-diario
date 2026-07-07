# Publicacao no GitHub

Este projeto esta pronto para ser enviado a um repositorio GitHub.

## Antes de publicar

Confira:

```powershell
git status
git log --oneline --max-count=5
```

O ideal e o `git status` aparecer limpo.

## Criar repositorio no GitHub

No GitHub, crie um repositorio novo. Sugestao de nome:

```txt
assistente-gasto-diario
```

Depois copie a URL HTTPS ou SSH do repositorio.

## Conectar o repositorio local

Usando HTTPS:

```powershell
git remote add origin https://github.com/SEU_USUARIO/assistente-gasto-diario.git
git push -u origin main
```

Usando SSH:

```powershell
git remote add origin git@github.com:SEU_USUARIO/assistente-gasto-diario.git
git push -u origin main
```

Se o remote `origin` ja existir:

```powershell
git remote set-url origin https://github.com/SEU_USUARIO/assistente-gasto-diario.git
git push -u origin main
```

## Configuracoes sensiveis

O arquivo `appsettings.json` esta configurado para desenvolvimento local.

Para producao, prefira variaveis de ambiente:

```powershell
ConnectionStrings__DefaultConnection="Host=...;Port=5432;Database=...;Username=...;Password=..."
Jwt__SigningKey="uma-chave-forte-e-privada"
```

Nao use a chave JWT local em producao.

## Links uteis apos rodar localmente

- App: http://localhost:5088/
- Swagger: http://localhost:5088/swagger/index.html
- Health: http://localhost:5088/health

