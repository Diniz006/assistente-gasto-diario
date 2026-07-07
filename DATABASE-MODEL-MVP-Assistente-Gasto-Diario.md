# Modelo de Banco de Dados - MVP do Assistente de Gasto Diario

Este documento transforma o PRD e o backlog em uma base de dados simples, realista e pronta para implementacao do MVP manual.

O foco e:

- separar os dados por usuario autenticado;
- permitir calculos do ciclo financeiro atual;
- armazenar renda, contas fixas, despesas, metas e alertas;
- manter o modelo simples o bastante para PostgreSQL e, com pequenos ajustes, para SQLite.

## 1. Visao geral do modelo

O MVP nao depende de Open Finance, integracoes bancarias ou automacoes complexas. Por isso, o banco pode ser organizado em torno de poucos objetos centrais:

- `users`: autentica o usuario.
- `financial_settings`: configura o ciclo financeiro e preferencias basicas.
- `categories`: categoriza receitas, despesas, contas e metas.
- `incomes`: registra entradas de dinheiro.
- `fixed_bills`: registra contas fixas recorrentes.
- `expenses`: registra gastos manuais.
- `financial_goals`: registra metas.
- `goal_contributions`: registra aportes feitos em metas.
- `monthly_budgets`: registra limite planejado por categoria no ciclo.
- `alerts`: registra alertas gerados pelo sistema.
- `cycle_summaries`: guarda o fechamento/resumo de cada ciclo.

### Premissas importantes

- Cada tabela operacional deve ter `user_id`.
- Cada usuario enxerga apenas seus proprios dados.
- O ciclo financeiro e calculado a partir de uma configuracao por usuario.
- O sistema calcula o ciclo atual com base em `cycle_start_day` e na data atual.
- Os valores monetarios devem usar `numeric(12,2)` em PostgreSQL.
- Para SQLite, o mesmo modelo funciona bem trocando `UUID` por `TEXT` e mantendo os demais tipos simples.

## 2. Relacionamentos em texto

```text
users
  1 -> 1 financial_settings
  1 -> N categories
  1 -> N incomes
  1 -> N fixed_bills
  1 -> N expenses
  1 -> N financial_goals
  1 -> N goal_contributions
  1 -> N monthly_budgets
  1 -> N alerts
  1 -> N cycle_summaries

categories
  1 -> N incomes
  1 -> N fixed_bills
  1 -> N expenses
  1 -> N financial_goals
  1 -> N monthly_budgets

financial_goals
  1 -> N goal_contributions

cycle_summaries
  pertence a 1 usuario e guarda um snapshot do ciclo
```

### Leitura rapida do modelo

O `financial_settings` define o comportamento do ciclo.
As tabelas transacionais (`incomes`, `fixed_bills`, `expenses`, `goal_contributions`) alimentam calculos do dashboard.
O `cycle_summaries` guarda o fechamento do periodo e permite historico sem recalcular tudo do zero.

## 3. Regras centrais que o banco precisa suportar

O banco deve permitir calcular:

- ciclo financeiro atual;
- dias restantes no ciclo;
- renda do ciclo;
- total de contas fixas;
- total de despesas;
- total planejado para metas;
- saldo disponivel;
- gasto diario seguro;
- progresso de metas;
- status de contas fixas;
- alertas simples;
- resumo do ciclo.

### Regra de ciclo financeiro

O ciclo nao depende apenas do mes-calendario.

Exemplo:

- dia de inicio configurado: 10
- data atual: 20/07
- ciclo atual: 10/07 a 09/08

O sistema deve calcular:

- `cycle_start_date`
- `cycle_end_date`
- `days_remaining`

Recomendacao de implementacao:

- se a data atual for maior ou igual ao dia de inicio no mes corrente, o ciclo comeca neste mes;
- caso contrario, o ciclo comeca no mes anterior;
- o fim do ciclo e o dia anterior ao proximo inicio;
- se o dia configurado nao existir no mes, usar a ultima data valida do mes para montagem do periodo.

## 4. Tipos e enums sugeridos

### `category_type`

- `income`
- `expense`
- `bill`
- `goal`

### `fixed_bill_status`

- `pending`
- `paid`
- `overdue`

### `goal_status`

- `active`
- `completed`
- `paused`
- `canceled`

### `goal_priority`

- `low`
- `medium`
- `high`

### `alert_type`

- `daily_limit_exceeded`
- `low_balance`
- `overdue_bill`
- `goal_completed`
- `general`

### `payment_method`

- `cash`
- `debit_card`
- `credit_card`
- `pix`
- `bank_transfer`
- `other`

Observacao: `pix` pode aparecer apenas como forma de pagamento manual, sem qualquer integracao real.

## 5. Descricao das tabelas

---

## 5.1 `users`

### Objetivo
Armazenar os dados de autenticao do usuario.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `name` | `varchar(120)` | sim | - | Nome do usuario |
| `email` | `varchar(255)` | sim | - | Unico |
| `password_hash` | `varchar(255)` | sim | - | Hash da senha |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |
| `updated_at` | `timestamptz` | sim | `now()` | Atualizacao |
| `deleted_at` | `timestamptz` | nao | `null` | Opcional, para soft delete futuro |

### Chave primaria
- `id`

### Chaves estrangeiras
- nenhuma

### Indices recomendados
- `unique` em `email`

### Regras de negocio
- o e-mail deve ser unico e normalizado;
- o usuario autenticado e o dono de todos os dados vinculados a ele;
- em caso de soft delete, o acesso deve ser bloqueado pela camada de aplicacao.

---

## 5.2 `financial_settings`

### Objetivo
Guardar configuracoes financeiras do usuario, principalmente o ciclo financeiro.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `monthly_income_default` | `numeric(12,2)` | nao | `0` | Renda mensal principal |
| `cycle_start_day` | `smallint` | sim | `1` | Dia de inicio do ciclo |
| `currency_code` | `char(3)` | sim | `'BRL'` | Ex.: BRL |
| `month_closure_day` | `smallint` | nao | `null` | Opcional, para futuras regras |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |
| `updated_at` | `timestamptz` | sim | `now()` | Atualizacao |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`

### Campos obrigatorios
- `user_id`
- `cycle_start_day`
- `currency_code`

### Campos opcionais
- `monthly_income_default`
- `month_closure_day`

### Valores padrao
- `monthly_income_default = 0`
- `cycle_start_day = 1`
- `currency_code = 'BRL'`

### Indices recomendados
- `unique` em `user_id`

### Regras de negocio
- cada usuario deve ter no maximo uma configuracao financeira ativa;
- o ciclo atual sera calculado a partir de `cycle_start_day`;
- o app deve suportar ciclos que atravessam meses diferentes.

---

## 5.3 `categories`

### Objetivo
Classificar receitas, despesas, contas e metas.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `name` | `varchar(80)` | sim | - | Nome da categoria |
| `type` | `category_type` | sim | - | Tipo da categoria |
| `color` | `varchar(20)` | nao | `null` | Opcional para interface |
| `icon` | `varchar(50)` | nao | `null` | Opcional para UI |
| `is_default` | `boolean` | sim | `false` | Indica categoria padrao criada no onboarding |
| `is_active` | `boolean` | sim | `true` | Controla uso sem apagar historico |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |
| `updated_at` | `timestamptz` | sim | `now()` | Atualizacao |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`

### Campos obrigatorios
- `user_id`
- `name`
- `type`

### Campos opcionais
- `color`
- `icon`

### Valores padrao
- `is_default = false`
- `is_active = true`

### Indices recomendados
- `index` em `user_id`
- `index` em `(user_id, type)`
- `unique` em `(user_id, name)` para evitar duplicidade simples

### Regras de negocio
- cada usuario tem suas proprias categorias;
- o sistema pode criar categorias padrao no onboarding;
- excluir categoria deve ser evitado se houver dependencia historica; prefira desativar.

---

## 5.4 `incomes`

### Objetivo
Registrar entradas de dinheiro do usuario.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `category_id` | `uuid` | nao | `null` | FK para `categories` |
| `description` | `varchar(160)` | sim | - | Ex.: salario, extra, freelancer |
| `amount` | `numeric(12,2)` | sim | - | Valor recebido |
| `received_on` | `date` | sim | - | Data de recebimento |
| `is_recurring` | `boolean` | sim | `false` | Indica recorrencia manual |
| `notes` | `text` | nao | `null` | Observacao opcional |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |
| `updated_at` | `timestamptz` | sim | `now()` | Atualizacao |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`
- `category_id -> categories.id`

### Campos obrigatorios
- `user_id`
- `description`
- `amount`
- `received_on`

### Campos opcionais
- `category_id`
- `notes`

### Valores padrao
- `is_recurring = false`

### Indices recomendados
- `index` em `(user_id, received_on)`
- `index` em `category_id`

### Regras de negocio
- a renda do ciclo pode ser composta por mais de um lancamento em `incomes`;
- se o produto quiser tratar renda fixa como unica fonte principal, isso pode ser configurado em `financial_settings`, mas os lancamentos continuam em `incomes`;
- o calculo do saldo deve considerar apenas as receitas dentro do ciclo atual.

---

## 5.5 `fixed_bills`

### Objetivo
Representar contas fixas recorrentes como aluguel, internet, luz e parcelas.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `category_id` | `uuid` | nao | `null` | FK para `categories` |
| `name` | `varchar(120)` | sim | - | Nome da conta |
| `amount` | `numeric(12,2)` | sim | - | Valor da conta |
| `due_day` | `smallint` | sim | - | Dia de vencimento |
| `status` | `fixed_bill_status` | sim | `'pending'` | Status atual da conta |
| `payment_date` | `date` | nao | `null` | Data de pagamento |
| `is_recurring_monthly` | `boolean` | sim | `true` | Conta recorrente mensal |
| `auto_include_in_cycle` | `boolean` | sim | `true` | Se entra no calculo do ciclo |
| `notes` | `text` | nao | `null` | Observacao opcional |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |
| `updated_at` | `timestamptz` | sim | `now()` | Atualizacao |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`
- `category_id -> categories.id`

### Campos obrigatorios
- `user_id`
- `name`
- `amount`
- `due_day`
- `status`

### Campos opcionais
- `category_id`
- `payment_date`
- `notes`

### Valores padrao
- `status = 'pending'`
- `is_recurring_monthly = true`
- `auto_include_in_cycle = true`

### Indices recomendados
- `index` em `(user_id, status)`
- `index` em `(user_id, due_day)`

### Regras de negocio
- conta fixa com vencimento vencido e sem pagamento deve virar `overdue`;
- conta marcada como `paid` deve registrar `payment_date`;
- o valor das contas fixas entra no calculo do saldo disponivel do ciclo;
- no MVP, esta tabela pode representar a definicao da conta, nao necessariamente cada ocorrencia mensal separada.

---

## 5.6 `expenses`

### Objetivo
Registrar os gastos manuais do usuario.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `category_id` | `uuid` | sim | - | FK para `categories` |
| `description` | `varchar(160)` | sim | - | Descricao do gasto |
| `amount` | `numeric(12,2)` | sim | - | Valor do gasto |
| `spent_on` | `date` | sim | - | Data do gasto |
| `payment_method` | `payment_method` | sim | `'other'` | Forma de pagamento |
| `notes` | `text` | nao | `null` | Observacao opcional |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |
| `updated_at` | `timestamptz` | sim | `now()` | Atualizacao |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`
- `category_id -> categories.id`

### Campos obrigatorios
- `user_id`
- `category_id`
- `description`
- `amount`
- `spent_on`
- `payment_method`

### Campos opcionais
- `notes`

### Valores padrao
- `payment_method = 'other'`

### Indices recomendados
- `index` em `(user_id, spent_on)`
- `index` em `(user_id, category_id)`

### Regras de negocio
- toda despesa registrada reduz o saldo disponivel do ciclo;
- despesas devem ser listadas por data, com edicao e exclusao no MVP;
- se uma despesa for editada, o saldo e os alertas devem ser recalculados.

---

## 5.7 `financial_goals`

### Objetivo
Armazenar metas financeiras como reserva de emergencia, viagem ou compra de um item.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `category_id` | `uuid` | nao | `null` | FK opcional para categoria de meta |
| `name` | `varchar(140)` | sim | - | Nome da meta |
| `target_amount` | `numeric(12,2)` | sim | - | Valor alvo |
| `current_amount` | `numeric(12,2)` | sim | `0` | Valor ja guardado |
| `monthly_planned_amount` | `numeric(12,2)` | nao | `0` | Quanto planeja guardar por mes |
| `target_date` | `date` | nao | `null` | Prazo desejado |
| `priority` | `goal_priority` | sim | `'medium'` | Prioridade |
| `status` | `goal_status` | sim | `'active'` | Status da meta |
| `notes` | `text` | nao | `null` | Observacao opcional |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |
| `updated_at` | `timestamptz` | sim | `now()` | Atualizacao |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`
- `category_id -> categories.id`

### Campos obrigatorios
- `user_id`
- `name`
- `target_amount`
- `current_amount`
- `priority`
- `status`

### Campos opcionais
- `category_id`
- `monthly_planned_amount`
- `target_date`
- `notes`

### Valores padrao
- `current_amount = 0`
- `monthly_planned_amount = 0`
- `priority = 'medium'`
- `status = 'active'`

### Indices recomendados
- `index` em `(user_id, status)`
- `index` em `(user_id, priority)`

### Regras de negocio
- o progresso da meta e `current_amount / target_amount`;
- quando a meta atingir 100%, o status pode virar `completed`;
- metas ativas entram no calculo do planejamento do ciclo.

---

## 5.8 `goal_contributions`

### Objetivo
Registrar aportes feitos em metas financeiras.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `financial_goal_id` | `uuid` | sim | - | FK para `financial_goals` |
| `amount` | `numeric(12,2)` | sim | - | Valor aportado |
| `contributed_on` | `date` | sim | - | Data do aporte |
| `notes` | `text` | nao | `null` | Observacao opcional |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`
- `financial_goal_id -> financial_goals.id`

### Campos obrigatorios
- `user_id`
- `financial_goal_id`
- `amount`
- `contributed_on`

### Campos opcionais
- `notes`

### Indices recomendados
- `index` em `(user_id, contributed_on)`
- `index` em `financial_goal_id`

### Regras de negocio
- aporte registrado deve ser somado ao `current_amount` da meta;
- o sistema pode atualizar a meta de forma transacional quando um aporte for criado;
- aportes podem ser usados para historico e auditoria.

---

## 5.9 `monthly_budgets`

### Objetivo
Guardar o orcamento mensal por categoria para o ciclo financeiro atual.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `category_id` | `uuid` | sim | - | FK para `categories` |
| `cycle_start_date` | `date` | sim | - | Inicio do ciclo |
| `cycle_end_date` | `date` | sim | - | Fim do ciclo |
| `budget_amount` | `numeric(12,2)` | sim | `0` | Limite planejado |
| `spent_amount` | `numeric(12,2)` | sim | `0` | Total gasto na categoria no ciclo |
| `notes` | `text` | nao | `null` | Observacao opcional |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |
| `updated_at` | `timestamptz` | sim | `now()` | Atualizacao |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`
- `category_id -> categories.id`

### Campos obrigatorios
- `user_id`
- `category_id`
- `cycle_start_date`
- `cycle_end_date`
- `budget_amount`

### Campos opcionais
- `notes`

### Valores padrao
- `spent_amount = 0`

### Indices recomendados
- `index` em `(user_id, cycle_start_date, cycle_end_date)`
- `index` em `category_id`
- `unique` em `(user_id, category_id, cycle_start_date, cycle_end_date)`

### Regras de negocio
- o orcamento e por categoria e por ciclo;
- o total gasto pode ser calculado na consulta, mas armazenar `spent_amount` ajuda no dashboard;
- ao fechar o ciclo, os valores podem ser copiados para `cycle_summaries`.

---

## 5.10 `alerts`

### Objetivo
Registrar alertas simples gerados pelo sistema.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `type` | `alert_type` | sim | `'general'` | Tipo do alerta |
| `title` | `varchar(120)` | sim | - | Titulo curto |
| `message` | `text` | sim | - | Mensagem exibida |
| `is_read` | `boolean` | sim | `false` | Controle de leitura |
| `related_entity_type` | `varchar(40)` | nao | `null` | Ex.: expense, bill, goal |
| `related_entity_id` | `uuid` | nao | `null` | ID da entidade relacionada |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |
| `read_at` | `timestamptz` | nao | `null` | Leitura |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`

### Campos obrigatorios
- `user_id`
- `type`
- `title`
- `message`

### Campos opcionais
- `related_entity_type`
- `related_entity_id`
- `read_at`

### Valores padrao
- `is_read = false`
- `type = 'general'`

### Indices recomendados
- `index` em `(user_id, is_read)`
- `index` em `(user_id, type)`
- `index` em `created_at`

### Regras de negocio
- alertas podem ser gerados por limite diario excedido, saldo baixo, conta vencida ou meta concluida;
- o MVP pode apenas criar alertas no banco e exibi-los no dashboard;
- excluir alerta nao precisa ser prioridade no primeiro commit; marcar como lido e suficiente.

---

## 5.11 `cycle_summaries`

### Objetivo
Guardar o fechamento consolidado de cada ciclo financeiro.

### Campos

| Campo | Tipo sugerido | Obrigatorio | Padrao | Observacao |
|---|---|---:|---|---|
| `id` | `uuid` | sim | `gen_random_uuid()` | Chave primaria |
| `user_id` | `uuid` | sim | - | FK para `users` |
| `cycle_start_date` | `date` | sim | - | Inicio do ciclo |
| `cycle_end_date` | `date` | sim | - | Fim do ciclo |
| `income_total` | `numeric(12,2)` | sim | `0` | Total de receitas no ciclo |
| `fixed_bills_total` | `numeric(12,2)` | sim | `0` | Total de contas fixas |
| `expenses_total` | `numeric(12,2)` | sim | `0` | Total de despesas |
| `goal_planned_total` | `numeric(12,2)` | sim | `0` | Total planejado para metas |
| `goal_contributed_total` | `numeric(12,2)` | sim | `0` | Total efetivamente aportado em metas |
| `available_balance` | `numeric(12,2)` | sim | `0` | Saldo disponivel do ciclo |
| `safe_daily_limit` | `numeric(12,2)` | sim | `0` | Gasto diario seguro calculado |
| `days_in_cycle` | `smallint` | sim | - | Total de dias do ciclo |
| `days_remaining_at_close` | `smallint` | nao | `null` | Opcional para snapshot de encerramento |
| `closed_at` | `timestamptz` | nao | `null` | Momento do fechamento |
| `created_at` | `timestamptz` | sim | `now()` | Criacao |

### Chave primaria
- `id`

### Chaves estrangeiras
- `user_id -> users.id`

### Campos obrigatorios
- `user_id`
- `cycle_start_date`
- `cycle_end_date`
- `income_total`
- `fixed_bills_total`
- `expenses_total`
- `goal_planned_total`
- `goal_contributed_total`
- `available_balance`
- `safe_daily_limit`
- `days_in_cycle`

### Campos opcionais
- `days_remaining_at_close`
- `closed_at`

### Valores padrao
- todos os totais monetarios comecam em `0`

### Indices recomendados
- `index` em `(user_id, cycle_start_date, cycle_end_date)`
- `unique` em `(user_id, cycle_start_date, cycle_end_date)`

### Regras de negocio
- esta tabela guarda o snapshot final do ciclo;
- pode ser preenchida manualmente no MVP ou por job simples no fechamento do periodo;
- evita recalculo historico a cada acesso ao dashboard.

## 6. SQL inicial em PostgreSQL

O SQL abaixo e uma base inicial. Ele pode ser executado em uma migration ou adaptado para outro ORM.

```sql
CREATE EXTENSION IF NOT EXISTS pgcrypto;

DO $$ BEGIN
    CREATE TYPE category_type AS ENUM ('income', 'expense', 'bill', 'goal');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

DO $$ BEGIN
    CREATE TYPE fixed_bill_status AS ENUM ('pending', 'paid', 'overdue');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

DO $$ BEGIN
    CREATE TYPE goal_status AS ENUM ('active', 'completed', 'paused', 'canceled');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

DO $$ BEGIN
    CREATE TYPE goal_priority AS ENUM ('low', 'medium', 'high');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

DO $$ BEGIN
    CREATE TYPE alert_type AS ENUM (
        'daily_limit_exceeded',
        'low_balance',
        'overdue_bill',
        'goal_completed',
        'general'
    );
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

DO $$ BEGIN
    CREATE TYPE payment_method AS ENUM (
        'cash',
        'debit_card',
        'credit_card',
        'pix',
        'bank_transfer',
        'other'
    );
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

CREATE TABLE IF NOT EXISTS users (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    name varchar(120) NOT NULL,
    email varchar(255) NOT NULL,
    password_hash varchar(255) NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    deleted_at timestamptz
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_users_email
    ON users (email);

CREATE TABLE IF NOT EXISTS financial_settings (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL UNIQUE REFERENCES users(id) ON DELETE CASCADE,
    monthly_income_default numeric(12,2) NOT NULL DEFAULT 0,
    cycle_start_day smallint NOT NULL DEFAULT 1,
    currency_code char(3) NOT NULL DEFAULT 'BRL',
    month_closure_day smallint,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS categories (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    name varchar(80) NOT NULL,
    type category_type NOT NULL,
    color varchar(20),
    icon varchar(50),
    is_default boolean NOT NULL DEFAULT false,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    UNIQUE (user_id, name)
);

CREATE INDEX IF NOT EXISTS ix_categories_user_id
    ON categories (user_id);

CREATE INDEX IF NOT EXISTS ix_categories_user_type
    ON categories (user_id, type);

CREATE TABLE IF NOT EXISTS incomes (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    category_id uuid REFERENCES categories(id) ON DELETE SET NULL,
    description varchar(160) NOT NULL,
    amount numeric(12,2) NOT NULL,
    received_on date NOT NULL,
    is_recurring boolean NOT NULL DEFAULT false,
    notes text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_incomes_user_received_on
    ON incomes (user_id, received_on);

CREATE INDEX IF NOT EXISTS ix_incomes_category_id
    ON incomes (category_id);

CREATE TABLE IF NOT EXISTS fixed_bills (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    category_id uuid REFERENCES categories(id) ON DELETE SET NULL,
    name varchar(120) NOT NULL,
    amount numeric(12,2) NOT NULL,
    due_day smallint NOT NULL,
    status fixed_bill_status NOT NULL DEFAULT 'pending',
    payment_date date,
    is_recurring_monthly boolean NOT NULL DEFAULT true,
    auto_include_in_cycle boolean NOT NULL DEFAULT true,
    notes text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_fixed_bills_user_status
    ON fixed_bills (user_id, status);

CREATE INDEX IF NOT EXISTS ix_fixed_bills_user_due_day
    ON fixed_bills (user_id, due_day);

CREATE TABLE IF NOT EXISTS expenses (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    category_id uuid NOT NULL REFERENCES categories(id) ON DELETE RESTRICT,
    description varchar(160) NOT NULL,
    amount numeric(12,2) NOT NULL,
    spent_on date NOT NULL,
    payment_method payment_method NOT NULL DEFAULT 'other',
    notes text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_expenses_user_spent_on
    ON expenses (user_id, spent_on);

CREATE INDEX IF NOT EXISTS ix_expenses_user_category
    ON expenses (user_id, category_id);

CREATE TABLE IF NOT EXISTS financial_goals (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    category_id uuid REFERENCES categories(id) ON DELETE SET NULL,
    name varchar(140) NOT NULL,
    target_amount numeric(12,2) NOT NULL,
    current_amount numeric(12,2) NOT NULL DEFAULT 0,
    monthly_planned_amount numeric(12,2) NOT NULL DEFAULT 0,
    target_date date,
    priority goal_priority NOT NULL DEFAULT 'medium',
    status goal_status NOT NULL DEFAULT 'active',
    notes text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_financial_goals_user_status
    ON financial_goals (user_id, status);

CREATE INDEX IF NOT EXISTS ix_financial_goals_user_priority
    ON financial_goals (user_id, priority);

CREATE TABLE IF NOT EXISTS goal_contributions (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    financial_goal_id uuid NOT NULL REFERENCES financial_goals(id) ON DELETE CASCADE,
    amount numeric(12,2) NOT NULL,
    contributed_on date NOT NULL,
    notes text,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_goal_contributions_user_date
    ON goal_contributions (user_id, contributed_on);

CREATE INDEX IF NOT EXISTS ix_goal_contributions_goal_id
    ON goal_contributions (financial_goal_id);

CREATE TABLE IF NOT EXISTS monthly_budgets (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    category_id uuid NOT NULL REFERENCES categories(id) ON DELETE CASCADE,
    cycle_start_date date NOT NULL,
    cycle_end_date date NOT NULL,
    budget_amount numeric(12,2) NOT NULL DEFAULT 0,
    spent_amount numeric(12,2) NOT NULL DEFAULT 0,
    notes text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    UNIQUE (user_id, category_id, cycle_start_date, cycle_end_date)
);

CREATE INDEX IF NOT EXISTS ix_monthly_budgets_user_cycle
    ON monthly_budgets (user_id, cycle_start_date, cycle_end_date);

CREATE INDEX IF NOT EXISTS ix_monthly_budgets_category_id
    ON monthly_budgets (category_id);

CREATE TABLE IF NOT EXISTS alerts (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    type alert_type NOT NULL DEFAULT 'general',
    title varchar(120) NOT NULL,
    message text NOT NULL,
    is_read boolean NOT NULL DEFAULT false,
    related_entity_type varchar(40),
    related_entity_id uuid,
    created_at timestamptz NOT NULL DEFAULT now(),
    read_at timestamptz
);

CREATE INDEX IF NOT EXISTS ix_alerts_user_is_read
    ON alerts (user_id, is_read);

CREATE INDEX IF NOT EXISTS ix_alerts_user_type
    ON alerts (user_id, type);

CREATE INDEX IF NOT EXISTS ix_alerts_created_at
    ON alerts (created_at);

CREATE TABLE IF NOT EXISTS cycle_summaries (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    cycle_start_date date NOT NULL,
    cycle_end_date date NOT NULL,
    income_total numeric(12,2) NOT NULL DEFAULT 0,
    fixed_bills_total numeric(12,2) NOT NULL DEFAULT 0,
    expenses_total numeric(12,2) NOT NULL DEFAULT 0,
    goal_planned_total numeric(12,2) NOT NULL DEFAULT 0,
    goal_contributed_total numeric(12,2) NOT NULL DEFAULT 0,
    available_balance numeric(12,2) NOT NULL DEFAULT 0,
    safe_daily_limit numeric(12,2) NOT NULL DEFAULT 0,
    days_in_cycle smallint NOT NULL,
    days_remaining_at_close smallint,
    closed_at timestamptz,
    created_at timestamptz NOT NULL DEFAULT now(),
    UNIQUE (user_id, cycle_start_date, cycle_end_date)
);

CREATE INDEX IF NOT EXISTS ix_cycle_summaries_user_cycle
    ON cycle_summaries (user_id, cycle_start_date, cycle_end_date);
```

## 7. Seeds iniciais sugeridos para categorias padrao

As categorias padrao podem ser criadas no onboarding do usuario. O mais simples para o MVP e inserir as categorias logo apos a criacao da conta, usando o `user_id` do usuario autenticado.

### Categorias padrao sugeridas

| Nome | Tipo | Uso |
|---|---|---|
| alimentacao | `expense` | Restaurantes, lanches, delivery |
| transporte | `expense` | Onibus, app, combustivel |
| mercado | `expense` | Compras de supermercado |
| lazer | `expense` | Cinema, saidas, assinaturas de entretenimento |
| saude | `expense` | Medicamentos, consultas, exames |
| estudos | `expense` | Curso, faculdade, livros |
| moradia | `expense` | Aluguel, condominio, reparos |
| contas | `bill` | Agua, luz, internet, telefone |
| compras | `expense` | Roupas, itens diversos |
| assinaturas | `expense` | Apps e servicos recorrentes |
| outros | `expense` | Qualquer gasto nao classificado |

### Seed em SQL por usuario

```sql
INSERT INTO categories (user_id, name, type, is_default)
VALUES
  (:user_id, 'alimentacao', 'expense', true),
  (:user_id, 'transporte', 'expense', true),
  (:user_id, 'mercado', 'expense', true),
  (:user_id, 'lazer', 'expense', true),
  (:user_id, 'saude', 'expense', true),
  (:user_id, 'estudos', 'expense', true),
  (:user_id, 'moradia', 'expense', true),
  (:user_id, 'contas', 'bill', true),
  (:user_id, 'compras', 'expense', true),
  (:user_id, 'assinaturas', 'expense', true),
  (:user_id, 'outros', 'expense', true);
```

## 8. O que pode ser simplificado no primeiro commit

Para acelerar o MVP, algumas partes podem comecar mais simples:

- `monthly_budgets` pode ser somente armazenado como limite, sem calculos avancados por categoria.
- `cycle_summaries` pode ser preenchido por um job simples ou ate manualmente no inicio.
- `alerts` pode registrar apenas alertas de limite excedido, saldo baixo e conta atrasada.
- `monthly_income_default` pode ser apenas um atalho do onboarding, sem automatizacao adicional.
- `deleted_at` em `users` pode ficar para evolucao futura, se o primeiro commit quiser ainda mais simplicidade.
- `related_entity_type` e `related_entity_id` em `alerts` podem ser ignorados na primeira tela e usados apenas internamente.
- `color` e `icon` em `categories` podem ficar opcionais e sem interface no comeco.

## 9. O que deve ficar preparado para evolucao futura

Mesmo sem implementar agora, o modelo ja deixa caminho aberto para:

- recorrencia mais sofisticada de contas e receitas;
- fechamento automatico de ciclo;
- historico de ciclos fechados;
- orcamentos por categoria mais detalhados;
- comparacao entre ciclos;
- notificacoes e alertas mais inteligentes;
- importacao de arquivos CSV ou XLSX;
- leitura automatica de transacoes no futuro;
- Open Finance em fase posterior.

### Possiveis evolucoes sem quebrar o modelo atual

- adicionar uma tabela de `transactions` unificada no futuro;
- separar ocorrencias mensais de contas fixas em uma tabela propria;
- incluir `attachments` para documentos e comprovantes;
- incluir historico de alteracoes com auditoria;
- adicionar multi-moeda se o produto crescer para outros mercados.

## 10. Observacoes finais de arquitetura

### Decisoes boas para o MVP

- usar `uuid` como chave primaria em todas as tabelas;
- usar `ON DELETE CASCADE` para dados pertencentes ao usuario;
- usar `ON DELETE SET NULL` onde a categoria nao pode apagar o historico;
- manter valores monetarios como `numeric(12,2)`;
- manter datas como `date` e carimbos de operacao como `timestamptz`.

### Regra pratica de consulta

Para o dashboard, o backend deve buscar:

- configuracao financeira do usuario;
- receitas dentro do ciclo atual;
- contas fixas do ciclo;
- despesas dentro do ciclo;
- metas ativas;
- aporte total em metas;
- alertas nao lidos;
- ultimo `cycle_summary` fechado, se existir.

### Estrategia recomendada para calculo

O banco guarda os eventos e o backend calcula:

- saldo disponivel;
- dias restantes;
- limite diario seguro;
- progresso da meta;
- status das contas.

Isso deixa o sistema simples, facil de manter e pronto para crescer.

