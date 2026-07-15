# Assistente de Gasto Diario Flutter

Migracao da interface HTML/CSS/JavaScript para Flutter com foco inicial em Android.

## Status

Esta pasta implementa a **Etapa 1 - Interface** com dados simulados. A API ainda nao e consumida pelas telas.

O scaffold Android ja foi gerado com:

```powershell
flutter create --platforms=android .
```

## Como executar

Em uma maquina com Flutter, Android SDK e JDK moderno instalados:

```powershell
cd flutter_app
flutter pub get
dart format .
flutter analyze
flutter test
flutter run -d android
```

Nesta maquina, foi usado um Flutter SDK local em `C:\Users\dinic\.codex\flutter-sdk`. O build/run Android ainda depende de configurar:

- Android SDK / `ANDROID_HOME`;
- JDK moderno compativel com Gradle Android.

## Validacao feita nesta rodada

- `flutter pub get`: passou.
- `dart format .`: passou.
- `flutter analyze`: passou sem issues.
- `flutter test`: passou.
- `flutter build apk --debug`: bloqueado por ausencia de Android SDK.

## Dependencias

- `http`: cliente centralizado para a futura integracao com a API.
- `shared_preferences`: cache local dos ultimos dados carregados e sessao.
- `connectivity_plus`: deteccao de conectividade para modo offline.
- `intl`: formatacao de moeda em pt-BR.
- `flutter_lints`: lint padrao do projeto Flutter.

## Observacoes visuais

A interface usa tokens extraidos de `styles.css`: cores, raios, sombras, espacamentos e hierarquia de texto. A fonte HTML usa `Georgia` e `Trebuchet MS`; no Android essas familias podem nao existir no sistema. Antes de fechar fidelidade visual, o ideal e incluir arquivos `.ttf` equivalentes/licenciados ou aprovar uma substituta.
