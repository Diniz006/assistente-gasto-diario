import 'package:flutter/material.dart';

import '../../../core/theme/app_tokens.dart';
import '../../../core/widgets/app_button.dart';
import '../../../core/widgets/app_fields.dart';
import '../../../core/widgets/app_panel.dart';
import '../../../core/widgets/section_title.dart';
import '../../dashboard/domain/finance_models.dart';
import '../../dashboard/presentation/dashboard_controller.dart';
import '../../shared/formatters.dart';

class GoalsPanel extends StatelessWidget {
  const GoalsPanel(
      {required this.goals,
      required this.categories,
      required this.controller,
      super.key});

  final List<GoalModel> goals;
  final List<CategoryModel> categories;
  final DashboardController controller;

  @override
  Widget build(BuildContext context) {
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(
              eyebrow: 'Planos e reservas', title: 'Metas financeiras'),
          const Wrap(
            spacing: 14,
            runSpacing: 14,
            crossAxisAlignment: WrapCrossAlignment.end,
            children: [
              SizedBox(
                  width: 220,
                  child: AppTextField(
                      label: 'Nome da meta',
                      hint: 'Ex: reserva, viagem, notebook')),
              SizedBox(
                  width: 140,
                  child: AppTextField(label: 'Valor alvo', hint: '0,00')),
              SizedBox(
                  width: 140,
                  child: AppTextField(label: 'Já guardado', initialValue: '0')),
              SizedBox(
                  width: 150,
                  child: AppTextField(
                      label: 'Guardar por mês', initialValue: '0')),
              SizedBox(
                  width: 170,
                  child: AppTextField(
                      label: 'Categoria', initialValue: 'Sem categoria')),
              AppButton(label: 'Criar meta'),
            ],
          ),
          const SizedBox(height: 18),
          if (goals.isEmpty)
            const EmptyState(
                'Crie uma meta para acompanhar seu progresso e reservar dinheiro sem perder o limite do dia.')
          else
            ...goals
                .map((goal) => _GoalItem(goal: goal, controller: controller)),
        ],
      ),
    );
  }
}

class _GoalItem extends StatelessWidget {
  const _GoalItem({required this.goal, required this.controller});

  final GoalModel goal;
  final DashboardController controller;

  @override
  Widget build(BuildContext context) {
    final remaining =
        (goal.targetAmount - goal.currentAmount).clamp(0, double.infinity);
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.58),
        border: Border.all(color: const Color(0x1a18221d)),
        borderRadius: BorderRadius.circular(22),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(goal.name,
                        style: const TextStyle(
                            fontSize: 16, fontWeight: FontWeight.w700)),
                    const SizedBox(height: 4),
                    Text('${goal.percent}% concluída',
                        style: const TextStyle(
                            color: AppTokens.muted,
                            fontFamily: 'Trebuchet MS')),
                  ],
                ),
              ),
              Text(
                '${Formatters.currency(goal.currentAmount)} / ${Formatters.currency(goal.targetAmount)}',
                style: const TextStyle(
                    color: AppTokens.brandStrong,
                    fontFamily: 'Trebuchet MS',
                    fontWeight: FontWeight.w800),
              ),
            ],
          ),
          const SizedBox(height: 12),
          ClipRRect(
            borderRadius: BorderRadius.circular(999),
            child: LinearProgressIndicator(
              minHeight: 12,
              value: goal.percent / 100,
              backgroundColor: const Color(0x1a18221d),
              valueColor: const AlwaysStoppedAnimation(AppTokens.sun),
            ),
          ),
          const SizedBox(height: 10),
          Row(
            children: [
              Expanded(
                  child: Text('Falta ${Formatters.currency(remaining)}',
                      style: const TextStyle(color: AppTokens.muted))),
              Text(
                  'Plano mensal ${Formatters.currency(goal.monthlyPlannedAmount)}',
                  style: const TextStyle(color: AppTokens.muted)),
            ],
          ),
          const SizedBox(height: 14),
          const Divider(color: Color(0x1a18221d)),
          Wrap(
            spacing: 12,
            runSpacing: 12,
            crossAxisAlignment: WrapCrossAlignment.end,
            children: [
              const SizedBox(
                  width: 150,
                  child: AppTextField(label: 'Contribuir', hint: '0,00')),
              const SizedBox(
                  width: 260,
                  child: AppTextField(label: 'Nota', hint: 'Opcional')),
              AppButton(
                label: 'Guardar',
                tone: AppButtonTone.ghost,
                onPressed: () => controller.showToast(
                    'Contribuição registrada. Meta e limite recalculados.'),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
