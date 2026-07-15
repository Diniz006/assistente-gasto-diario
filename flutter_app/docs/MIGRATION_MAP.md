# Mapa da Migracao HTML -> Flutter

## Telas convertidas na Etapa 1

| HTML/CSS/JS | Flutter |
| --- | --- |
| Hero inicial + `auth-panel` | `features/authentication/presentation/auth_panel.dart` |
| `app-header` + `mobile-nav` | `features/dashboard/presentation/finance_app.dart` |
| `dashboard-grid` | `features/dashboard/presentation/dashboard_home.dart` |
| `limit-card` | `features/daily_safe_spending/presentation/daily_safe_spending_card.dart` |
| `summary-card` | `_SummaryCard` em `dashboard_home.dart` |
| `insights-panel` | `_InsightsPanel`, `_FlowBars`, `_CategoryBars` |
| `monthly-panel` | `_MonthlyPanel` |
| `quick-panel` | `features/transactions/presentation/transactions_panels.dart` |
| `expenses-panel` | `ExpensesPanel` |
| `incomes-panel` | `IncomesPanel` |
| `bills-panel` | `features/accounts/presentation/accounts_panel.dart` |
| `goals-panel` | `features/goals/presentation/goals_panel.dart` |
| `categories-panel` | `features/categories/presentation/categories_panel.dart` |
| `settings-panel` | `_SettingsPanel` em `dashboard_home.dart` |
| `export-panel` | `_ExportPanel` em `dashboard_home.dart` |
| `toast` e `offline-banner` | `_Toast` e `_OfflineBanner` em `finance_app.dart` |

## Estados de interface previstos

Todos devem continuar usando componentes estilizados do app:

- carregamento;
- sucesso;
- ausencia de dados;
- erro de conexao;
- servidor indisponivel;
- sessao expirada;
- modo offline.

`ApiClient` valida status HTTP e `Content-Type` antes de decodificar JSON. HTML inesperado é tratado como erro amigável.

## Diferencas inevitaveis nesta etapa

- CSS `backdrop-filter` foi aproximado com `BackdropFilter` do Flutter.
- `hover` do HTML não existe da mesma forma em Android; fica representado por `InkWell`/toque.
- `input type="date"`, `month` e `color` precisam virar pickers Flutter na próxima rodada de refinamento.
- Fontes `Georgia` e `Trebuchet MS` dependem de arquivos de fonte para fidelidade real no Android.
- O projeto Android nativo não foi gerado porque `flutter` não está instalado no PATH desta máquina.

## Proxima etapa

Depois da aprovação visual, conectar repositórios reais por feature usando `ApiClient` e `LocalCache`, preservando os cálculos financeiros na API.
