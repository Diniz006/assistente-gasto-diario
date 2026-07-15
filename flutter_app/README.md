# Assistente de Gasto Diario Flutter

Migração da interface HTML/CSS/JavaScript para Flutter com foco inicial em Android.

## Status

Esta pasta implementa a **Etapa 1 - Interface** com dados simulados. A API ainda não é consumida pelas telas.

## Como executar

O Flutter não está disponível no PATH desta máquina durante a criação. Em uma máquina com Flutter instalado:

```powershell
cd flutter_app
flutter pub get
flutter run -d android
```

Se a pasta Android ainda não existir, gere o scaffold nativo antes:

```powershell
cd flutter_app
flutter create --platforms=android .
flutter pub get
flutter run -d android
```

## Dependências

- `http`: cliente centralizado para a futura integração com a API.
- `shared_preferences`: cache local dos últimos dados carregados e sessão.
- `connectivity_plus`: detecção de conectividade para modo offline.
- `intl`: formatação de moeda em pt-BR.
- `flutter_lints`: lint padrão do projeto Flutter.

## Observações visuais

A interface usa tokens extraídos de `styles.css`: cores, raios, sombras, espaçamentos e hierarquia de texto. A fonte HTML usa `Georgia` e `Trebuchet MS`; no Android essas famílias podem não existir no sistema. Antes de fechar fidelidade visual, o ideal é incluir arquivos `.ttf` equivalentes/licenciados ou aprovar uma substituta.
