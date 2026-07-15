import 'package:flutter/material.dart';

import '../../../core/theme/app_tokens.dart';
import '../../../core/widgets/app_button.dart';
import '../../../core/widgets/app_fields.dart';
import '../../../core/widgets/app_panel.dart';
import '../../../core/widgets/section_title.dart';
import '../../dashboard/domain/finance_models.dart';
import '../../dashboard/presentation/dashboard_controller.dart';
import '../../shared/formatters.dart';

class QuickExpensePanel extends StatelessWidget {
  const QuickExpensePanel(
      {required this.controller, required this.categories, super.key});

  final DashboardController controller;
  final List<CategoryModel> categories;

  @override
  Widget build(BuildContext context) {
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(
              eyebrow: 'Lançamento rápido', title: 'Registrar despesa'),
          Wrap(
            spacing: 14,
            runSpacing: 14,
            crossAxisAlignment: WrapCrossAlignment.end,
            children: [
              const SizedBox(
                  width: 230,
                  child: AppTextField(
                      label: 'O que foi?',
                      hint: 'Ex: cafe, mercado, uber',
                      maxLength: 160)),
              const SizedBox(
                  width: 140,
                  child: AppTextField(
                      label: 'Valor',
                      hint: '0,00',
                      keyboardType: TextInputType.number)),
              const SizedBox(
                  width: 150,
                  child:
                      AppTextField(label: 'Data', initialValue: '2026-07-15')),
              SizedBox(
                width: 180,
                child: AppSelectField<String>(
                  label: 'Categoria',
                  value: '',
                  items: [
                    const DropdownMenuItem(
                        value: '', child: Text('Automática')),
                    ...categories
                        .where((item) => item.type == CategoryType.expense)
                        .map((item) => DropdownMenuItem(
                            value: item.id, child: Text(item.name))),
                  ],
                  onChanged: (_) {},
                ),
              ),
              const SizedBox(
                  width: 150,
                  child: AppTextField(label: 'Pagamento', initialValue: 'Pix')),
              const SizedBox(
                  width: 220,
                  child: AppTextField(
                      label: 'Observação', hint: 'Opcional', maxLength: 300)),
              AppButton(
                label: 'Lançar despesa',
                onPressed: () => controller
                    .showToast('Despesa registrada. Limite atualizado.'),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class IncomesPanel extends StatelessWidget {
  const IncomesPanel(
      {required this.incomes, required this.categories, super.key});

  final List<MoneyLine> incomes;
  final List<CategoryModel> categories;

  @override
  Widget build(BuildContext context) {
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(
              eyebrow: 'Dinheiro que entrou', title: 'Rendas do ciclo'),
          const Wrap(
            spacing: 14,
            runSpacing: 14,
            crossAxisAlignment: WrapCrossAlignment.end,
            children: [
              SizedBox(
                  width: 230,
                  child: AppTextField(
                      label: 'Descrição', hint: 'Ex: salário, freelance')),
              SizedBox(
                  width: 140,
                  child: AppTextField(label: 'Valor', hint: '0,00')),
              SizedBox(
                  width: 160,
                  child: AppTextField(
                      label: 'Recebido em', initialValue: '2026-07-15')),
              SizedBox(
                  width: 180,
                  child: AppTextField(
                      label: 'Categoria', initialValue: 'Sem categoria')),
              AppButton(label: 'Adicionar renda'),
            ],
          ),
          const SizedBox(height: 18),
          if (incomes.isEmpty)
            const EmptyState(
                'Quando você cadastrar uma renda do ciclo, ela aparece aqui.')
          else
            ...incomes.map((item) => MoneyListItem(item: item, positive: true)),
        ],
      ),
    );
  }
}

class ExpensesPanel extends StatelessWidget {
  const ExpensesPanel(
      {required this.expenses, required this.categories, super.key});

  final List<MoneyLine> expenses;
  final List<CategoryModel> categories;

  @override
  Widget build(BuildContext context) {
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(
              eyebrow: 'Histórico do ciclo', title: 'Despesas e filtros'),
          const Wrap(
            spacing: 14,
            runSpacing: 14,
            crossAxisAlignment: WrapCrossAlignment.end,
            children: [
              SizedBox(
                  width: 230,
                  child: AppTextField(
                      label: 'Buscar', hint: 'Ex: mercado, uber, cafe')),
              SizedBox(
                  width: 140,
                  child: AppTextField(
                      label: 'Início', initialValue: '2026-07-01')),
              SizedBox(
                  width: 140,
                  child:
                      AppTextField(label: 'Fim', initialValue: '2026-07-31')),
              SizedBox(
                  width: 160,
                  child:
                      AppTextField(label: 'Categoria', initialValue: 'Todas')),
              SizedBox(
                  width: 150,
                  child:
                      AppTextField(label: 'Pagamento', initialValue: 'Todos')),
              AppButton(label: 'Filtrar', tone: AppButtonTone.ghost),
              AppButton(label: 'Limpar', tone: AppButtonTone.tiny),
            ],
          ),
          const SizedBox(height: 18),
          if (expenses.isEmpty)
            const EmptyState('Quando você lançar despesas, elas aparecem aqui.')
          else
            ...expenses.map((item) => MoneyListItem(item: item)),
        ],
      ),
    );
  }
}

class MoneyListItem extends StatelessWidget {
  const MoneyListItem({required this.item, this.positive = false, super.key});

  final MoneyLine item;
  final bool positive;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.58),
        border: Border.all(color: const Color(0x1a18221d)),
        borderRadius: BorderRadius.circular(22),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(item.title,
                    style: const TextStyle(
                        fontSize: 16, fontWeight: FontWeight.w700)),
                const SizedBox(height: 4),
                Text(item.subtitle,
                    style: const TextStyle(
                        color: AppTokens.muted, fontFamily: 'Trebuchet MS')),
              ],
            ),
          ),
          const SizedBox(width: 12),
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Text(
                Formatters.currency(item.amount),
                style: TextStyle(
                  color: positive ? AppTokens.brandStrong : AppTokens.rose,
                  fontFamily: 'Trebuchet MS',
                  fontWeight: FontWeight.w800,
                ),
              ),
              const SizedBox(height: 8),
              const Wrap(
                spacing: 8,
                children: [
                  AppButton(label: 'Editar', tone: AppButtonTone.tiny),
                  AppButton(label: 'Excluir', tone: AppButtonTone.danger),
                ],
              ),
            ],
          ),
        ],
      ),
    );
  }
}
