import 'package:flutter/material.dart';

import '../../../core/theme/app_tokens.dart';
import '../../../core/widgets/app_button.dart';
import '../../../core/widgets/app_fields.dart';
import '../../../core/widgets/app_panel.dart';
import '../../../core/widgets/section_title.dart';
import '../../dashboard/domain/finance_models.dart';

class CategoriesPanel extends StatelessWidget {
  const CategoriesPanel({required this.categories, super.key});

  final List<CategoryModel> categories;

  @override
  Widget build(BuildContext context) {
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(eyebrow: 'Organização', title: 'Categorias'),
          Wrap(
            spacing: 14,
            runSpacing: 14,
            crossAxisAlignment: WrapCrossAlignment.end,
            children: const [
              SizedBox(width: 250, child: AppTextField(label: 'Nome', hint: 'Ex: pets, estudos, delivery')),
              SizedBox(width: 170, child: AppTextField(label: 'Tipo', initialValue: 'Despesa')),
              SizedBox(width: 110, child: AppTextField(label: 'Cor', initialValue: '#1f6f4a')),
              AppButton(label: 'Criar categoria'),
            ],
          ),
          const SizedBox(height: 18),
          for (final group in CategoryType.values) _CategoryGroup(type: group, categories: categories.where((item) => item.type == group).toList()),
        ],
      ),
    );
  }
}

class _CategoryGroup extends StatelessWidget {
  const _CategoryGroup({required this.type, required this.categories});

  final CategoryType type;
  final List<CategoryModel> categories;

  @override
  Widget build(BuildContext context) {
    final title = switch (type) {
      CategoryType.income => 'Rendas',
      CategoryType.expense => 'Despesas',
      CategoryType.bill => 'Contas fixas',
      CategoryType.goal => 'Metas',
    };
    return Padding(
      padding: const EdgeInsets.only(bottom: 18),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(title, style: Theme.of(context).textTheme.titleLarge),
          const SizedBox(height: 10),
          if (categories.isEmpty)
            const EmptyState('Nenhuma categoria nesse tipo.')
          else
            ...categories.map((category) => _CategoryItem(category: category)),
        ],
      ),
    );
  }
}

class _CategoryItem extends StatelessWidget {
  const _CategoryItem({required this.category});

  final CategoryModel category;

  @override
  Widget build(BuildContext context) {
    return Opacity(
      opacity: category.isActive ? 1 : 0.52,
      child: Container(
        margin: const EdgeInsets.only(bottom: 10),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: BoxDecoration(
          color: Colors.white.withOpacity(0.58),
          border: Border.all(color: const Color(0x1a18221d)),
          borderRadius: BorderRadius.circular(22),
        ),
        child: Row(
          children: [
            Container(
              width: 18,
              height: 18,
              decoration: BoxDecoration(
                color: Color(category.color),
                shape: BoxShape.circle,
                border: Border.all(color: Colors.white.withOpacity(0.72), width: 2),
                boxShadow: const [BoxShadow(color: Color(0x2918221d), spreadRadius: 1)],
              ),
            ),
            const SizedBox(width: 14),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(category.name, style: const TextStyle(fontSize: 16, fontWeight: FontWeight.w700)),
                  const SizedBox(height: 4),
                  Text(category.isDefault ? 'Padrão' : 'Personalizada', style: const TextStyle(color: AppTokens.muted, fontFamily: 'Trebuchet MS')),
                ],
              ),
            ),
            const Wrap(
              spacing: 8,
              children: [
                AppButton(label: 'Editar', tone: AppButtonTone.tiny),
                AppButton(label: 'Ocultar', tone: AppButtonTone.danger),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
