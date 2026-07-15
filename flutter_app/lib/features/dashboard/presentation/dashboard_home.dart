import 'package:flutter/material.dart';

import '../../../core/theme/app_tokens.dart';
import '../../../core/widgets/app_button.dart';
import '../../../core/widgets/app_fields.dart';
import '../../../core/widgets/app_panel.dart';
import '../../../core/widgets/section_title.dart';
import '../../accounts/presentation/accounts_panel.dart';
import '../../categories/presentation/categories_panel.dart';
import '../../daily_safe_spending/presentation/daily_safe_spending_card.dart';
import '../../goals/presentation/goals_panel.dart';
import '../../shared/formatters.dart';
import '../../transactions/presentation/transactions_panels.dart';
import '../domain/finance_models.dart';
import 'dashboard_controller.dart';
import 'finance_app.dart';

class DashboardHome extends StatelessWidget {
  const DashboardHome({
    required this.controller,
    required this.snapshot,
    required this.activeTab,
    required this.onTabChanged,
    required this.onLogout,
    super.key,
  });

  final DashboardController controller;
  final DashboardSnapshot snapshot;
  final AppTab activeTab;
  final ValueChanged<AppTab> onTabChanged;
  final VoidCallback onLogout;

  @override
  Widget build(BuildContext context) {
    final content = switch (activeTab) {
      AppTab.today => _TodayTab(controller: controller, snapshot: snapshot),
      AppTab.quick => QuickExpensePanel(controller: controller, categories: snapshot.categories),
      AppTab.history => ExpensesPanel(expenses: snapshot.expenses, categories: snapshot.categories),
      AppTab.goals => GoalsPanel(goals: snapshot.goals, categories: snapshot.categories, controller: controller),
      AppTab.more => _MoreTab(snapshot: snapshot, onTabChanged: onTabChanged),
      AppTab.incomes => IncomesPanel(incomes: snapshot.incomes, categories: snapshot.categories),
      AppTab.bills => AccountsPanel(bills: snapshot.bills, categories: snapshot.categories),
      AppTab.categories => CategoriesPanel(categories: snapshot.categories),
      AppTab.settings => const _SettingsPanel(),
      AppTab.export => const _ExportPanel(),
    };

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _AppHeader(userName: snapshot.userName, onLogout: onLogout),
        const SizedBox(height: AppTokens.gapLg),
        _DesktopNav(active: activeTab, onChanged: onTabChanged),
        const SizedBox(height: AppTokens.gapLg),
        content,
      ],
    );
  }
}

class _AppHeader extends StatelessWidget {
  const _AppHeader({required this.userName, required this.onLogout});

  final String userName;
  final VoidCallback onLogout;

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const MutedLabel('Painel de hoje'),
              const SizedBox(height: 4),
              Text('Olá, $userName', style: Theme.of(context).textTheme.headlineLarge),
            ],
          ),
        ),
        AppButton(label: 'Sair', tone: AppButtonTone.ghost, onPressed: onLogout),
      ],
    );
  }
}

class _DesktopNav extends StatelessWidget {
  const _DesktopNav({required this.active, required this.onChanged});

  final AppTab active;
  final ValueChanged<AppTab> onChanged;

  @override
  Widget build(BuildContext context) {
    if (MediaQuery.sizeOf(context).width <= 860) {
      return const SizedBox.shrink();
    }
    final items = [
      (AppTab.today, 'Hoje'),
      (AppTab.quick, 'Lançar'),
      (AppTab.history, 'Histórico'),
      (AppTab.goals, 'Metas'),
      (AppTab.more, 'Mais'),
    ];
    return Align(
      alignment: Alignment.centerLeft,
      child: Container(
        padding: const EdgeInsets.all(6),
        decoration: BoxDecoration(
          color: AppTokens.panel.withOpacity(0.72),
          border: Border.all(color: const Color(0x1a18221d)),
          borderRadius: BorderRadius.circular(AppTokens.radiusPill),
          boxShadow: AppTokens.softShadow,
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            for (final item in items)
              Padding(
                padding: const EdgeInsets.only(right: 8),
                child: AppButton(
                  label: item.$2,
                  tone: active == item.$1 ? AppButtonTone.primary : AppButtonTone.ghost,
                  onPressed: () => onChanged(item.$1),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

class _TodayTab extends StatelessWidget {
  const _TodayTab({required this.controller, required this.snapshot});

  final DashboardController controller;
  final DashboardSnapshot snapshot;

  @override
  Widget build(BuildContext context) {
    final narrow = MediaQuery.sizeOf(context).width <= 860;
    final topCards = narrow
        ? Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              DailySafeSpendingCard(snapshot: snapshot),
              const SizedBox(height: AppTokens.gapLg),
              _SummaryCard(snapshot: snapshot),
            ],
          )
        : Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(flex: 14, child: DailySafeSpendingCard(snapshot: snapshot)),
              const SizedBox(width: AppTokens.gapLg),
              Expanded(flex: 8, child: _SummaryCard(snapshot: snapshot)),
            ],
          );

    return Column(
      children: [
        topCards,
        const SizedBox(height: AppTokens.gapLg),
        _InsightsPanel(snapshot: snapshot),
        const SizedBox(height: AppTokens.gapLg),
        _MonthlyPanel(),
      ],
    );
  }
}

class _SummaryCard extends StatelessWidget {
  const _SummaryCard({required this.snapshot});

  final DashboardSnapshot snapshot;

  @override
  Widget build(BuildContext context) {
    final rows = [
      ('Renda', snapshot.incomeTotal),
      ('Contas', snapshot.fixedBillsTotal),
      ('Despesas', snapshot.expensesTotal),
      ('Metas', snapshot.goalsTotal),
    ];
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Resumo do ciclo', style: Theme.of(context).textTheme.headlineMedium),
          const SizedBox(height: 22),
          for (final row in rows)
            Padding(
              padding: const EdgeInsets.only(bottom: 14),
              child: Row(
                children: [
                  Expanded(
                    child: Text(row.$1, style: const TextStyle(color: AppTokens.muted, fontFamily: 'Trebuchet MS')),
                  ),
                  Text(
                    Formatters.currency(row.$2),
                    style: const TextStyle(fontFamily: 'Trebuchet MS', fontWeight: FontWeight.w800),
                  ),
                ],
              ),
            ),
        ],
      ),
    );
  }
}

class _InsightsPanel extends StatelessWidget {
  const _InsightsPanel({required this.snapshot});

  final DashboardSnapshot snapshot;

  @override
  Widget build(BuildContext context) {
    final narrow = MediaQuery.sizeOf(context).width <= 860;
    final charts = narrow
        ? Column(
            children: [
              _ChartCard(
                eyebrow: 'Fluxo principal',
                title: 'Entrou, guardou, saiu',
                child: _FlowBars(snapshot: snapshot),
              ),
              const SizedBox(height: 16),
              _ChartCard(
                eyebrow: 'Despesas',
                title: 'Gastos por categoria',
                child: _CategoryBars(snapshot: snapshot),
              ),
            ],
          )
        : Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                flex: 9,
                child: _ChartCard(
                  eyebrow: 'Fluxo principal',
                  title: 'Entrou, guardou, saiu',
                  child: _FlowBars(snapshot: snapshot),
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                flex: 11,
                child: _ChartCard(
                  eyebrow: 'Despesas',
                  title: 'Gastos por categoria',
                  child: _CategoryBars(snapshot: snapshot),
                ),
              ),
            ],
          );

    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(eyebrow: 'Leitura do ciclo', title: 'Para onde o dinheiro está indo?'),
          charts,
        ],
      ),
    );
  }
}

class _ChartCard extends StatelessWidget {
  const _ChartCard({required this.eyebrow, required this.title, required this.child});

  final String eyebrow;
  final String title;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.5),
        border: Border.all(color: const Color(0x1a18221d)),
        borderRadius: BorderRadius.circular(26),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          MutedLabel(eyebrow),
          const SizedBox(height: 3),
          Text(title, style: Theme.of(context).textTheme.titleLarge),
          const SizedBox(height: 16),
          child,
        ],
      ),
    );
  }
}

class _FlowBars extends StatelessWidget {
  const _FlowBars({required this.snapshot});

  final DashboardSnapshot snapshot;

  @override
  Widget build(BuildContext context) {
    final rows = [
      ('Renda', snapshot.incomeTotal, const [Color(0xff2f855a), AppTokens.brand]),
      ('Guardado', snapshot.goalsTotal, const [AppTokens.sun, Color(0xffd59a2f)]),
      ('Saídas', snapshot.fixedBillsTotal + snapshot.expensesTotal, const [AppTokens.rose, Color(0xffa9443c)]),
    ];
    final maxValue = rows.map((row) => row.$2).reduce((a, b) => a > b ? a : b);
    return Column(
      children: [
        for (final row in rows)
          _ProgressLine(label: row.$1, amount: row.$2, percent: row.$2 / maxValue, colors: row.$3),
      ],
    );
  }
}

class _CategoryBars extends StatelessWidget {
  const _CategoryBars({required this.snapshot});

  final DashboardSnapshot snapshot;

  @override
  Widget build(BuildContext context) {
    final expenseCategories = snapshot.expenses.map((expense) {
      final category = snapshot.categories.firstWhere((item) => item.id == expense.categoryId);
      return (category, expense.amount);
    }).toList();
    final maxValue = expenseCategories.map((row) => row.$2).reduce((a, b) => a > b ? a : b);
    return Column(
      children: [
        for (final row in expenseCategories)
          _ProgressLine(
            label: row.$1.name,
            amount: row.$2,
            percent: row.$2 / maxValue,
            colors: [Color(row.$1.color), Color(row.$1.color).withOpacity(0.72)],
            dotColor: Color(row.$1.color),
          ),
      ],
    );
  }
}

class _ProgressLine extends StatelessWidget {
  const _ProgressLine({
    required this.label,
    required this.amount,
    required this.percent,
    required this.colors,
    this.dotColor,
  });

  final String label;
  final double amount;
  final double percent;
  final List<Color> colors;
  final Color? dotColor;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 14),
      child: Column(
        children: [
          Row(
            children: [
              if (dotColor != null)
                Container(
                  width: 12,
                  height: 12,
                  margin: const EdgeInsets.only(right: 8),
                  decoration: BoxDecoration(color: dotColor, shape: BoxShape.circle),
                ),
              Expanded(child: Text(label, style: const TextStyle(fontFamily: 'Trebuchet MS'))),
              Text(Formatters.currency(amount), style: const TextStyle(color: AppTokens.muted, fontFamily: 'Trebuchet MS')),
            ],
          ),
          const SizedBox(height: 8),
          ClipRRect(
            borderRadius: BorderRadius.circular(AppTokens.radiusPill),
            child: Container(
              height: 16,
              color: const Color(0x1a18221d),
              alignment: Alignment.centerLeft,
              child: FractionallySizedBox(
                widthFactor: percent.clamp(0.03, 1),
                child: Container(decoration: BoxDecoration(gradient: LinearGradient(colors: colors))),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _MonthlyPanel extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(eyebrow: 'Resumo do mês', title: 'Compare sua evolução'),
          Wrap(
            runSpacing: 12,
            spacing: 12,
            children: const [
              _MonthlyCard(label: 'Entradas', value: 'R$ 4.200,00', tone: AppTokens.brandStrong),
              _MonthlyCard(label: 'Guardado', value: 'R$ 650,00', tone: Color(0xff9a681a)),
              _MonthlyCard(label: 'Saídas', value: 'R$ 2.472,50', tone: Color(0xff9d3b33)),
              _MonthlyCard(label: 'Saldo', value: 'R$ 1.077,50', tone: AppTokens.brandStrong),
            ],
          ),
          const SizedBox(height: 18),
          const EmptyState('A tendência dos últimos meses será conectada à API na Etapa 2.'),
        ],
      ),
    );
  }
}

class _MonthlyCard extends StatelessWidget {
  const _MonthlyCard({required this.label, required this.value, required this.tone});

  final String label;
  final String value;
  final Color tone;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 240,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.52),
        border: Border.all(color: const Color(0x1a18221d)),
        borderRadius: BorderRadius.circular(24),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(label, style: const TextStyle(color: AppTokens.muted, fontFamily: 'Trebuchet MS')),
          const SizedBox(height: 8),
          Text(value, style: TextStyle(color: tone, fontFamily: 'Trebuchet MS', fontSize: 22, fontWeight: FontWeight.w800)),
        ],
      ),
    );
  }
}

class _MoreTab extends StatelessWidget {
  const _MoreTab({required this.snapshot, required this.onTabChanged});

  final DashboardSnapshot snapshot;
  final ValueChanged<AppTab> onTabChanged;

  @override
  Widget build(BuildContext context) {
    final items = [
      ('Rendas', '${snapshot.incomes.length}', AppTab.incomes),
      ('Contas fixas', '${snapshot.bills.length}', AppTab.bills),
      ('Categorias', '${snapshot.categories.where((item) => item.isActive).length}', AppTab.categories),
      ('Ajustes', 'Ciclo', AppTab.settings),
      ('Exportar', 'CSV', AppTab.export),
    ];
    return AppPanel(
      padding: const EdgeInsets.symmetric(horizontal: 30, vertical: 28),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(eyebrow: 'Mais opções', title: 'Organize seu dinheiro'),
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: [
              for (final item in items)
                SizedBox(
                  width: MediaQuery.sizeOf(context).width <= 860 ? double.infinity : 190,
                  child: _MoreButton(label: item.$1, badge: item.$2, onTap: () => onTabChanged(item.$3)),
                ),
            ],
          ),
        ],
      ),
    );
  }
}

class _MoreButton extends StatelessWidget {
  const _MoreButton({required this.label, required this.badge, required this.onTap});

  final String label;
  final String badge;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: const Color(0x141f6f4a),
      borderRadius: BorderRadius.circular(18),
      child: InkWell(
        borderRadius: BorderRadius.circular(18),
        onTap: onTap,
        child: Container(
          constraints: const BoxConstraints(minHeight: 68),
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
          decoration: BoxDecoration(
            border: Border.all(color: const Color(0x1a18221d)),
            borderRadius: BorderRadius.circular(18),
          ),
          child: Row(
            children: [
              Expanded(
                child: Text(label, style: const TextStyle(fontFamily: 'Trebuchet MS', fontWeight: FontWeight.w800)),
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 9, vertical: 5),
                decoration: BoxDecoration(color: Colors.white.withOpacity(0.72), borderRadius: BorderRadius.circular(999)),
                child: Text(
                  badge,
                  style: const TextStyle(
                    color: AppTokens.brandStrong,
                    fontFamily: 'Trebuchet MS',
                    fontSize: 12,
                    fontWeight: FontWeight.w800,
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _SettingsPanel extends StatelessWidget {
  const _SettingsPanel();

  @override
  Widget build(BuildContext context) {
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(eyebrow: 'Configurações financeiras', title: 'Ajuste renda e ciclo'),
          Wrap(
            spacing: 14,
            runSpacing: 14,
            crossAxisAlignment: WrapCrossAlignment.end,
            children: const [
              SizedBox(width: 260, child: AppTextField(label: 'Renda mensal padrão', initialValue: '4200')),
              SizedBox(width: 200, child: AppTextField(label: 'Dia inicial do ciclo', initialValue: '1')),
              SizedBox(width: 200, child: AppTextField(label: 'Dia de fechamento', initialValue: '31')),
            ],
          ),
          const SizedBox(height: 14),
          Align(alignment: Alignment.centerLeft, child: AppButton(label: 'Salvar ajustes', onPressed: null)),
        ],
      ),
    );
  }
}

class _ExportPanel extends StatelessWidget {
  const _ExportPanel();

  @override
  Widget build(BuildContext context) {
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const SectionTitle(eyebrow: 'Exportação', title: 'Baixe seus dados'),
          Wrap(
            spacing: 10,
            runSpacing: 10,
            children: const [
              AppButton(label: 'Despesas filtradas', tone: AppButtonTone.ghost),
              AppButton(label: 'Rendas do ciclo', tone: AppButtonTone.ghost),
              AppButton(label: 'Resumo mensal', tone: AppButtonTone.ghost),
            ],
          ),
        ],
      ),
    );
  }
}
