import 'package:flutter/material.dart';

import '../../../core/widgets/app_button.dart';
import '../../../core/widgets/app_fields.dart';
import '../../../core/widgets/app_panel.dart';
import '../../../core/widgets/section_title.dart';
import '../../dashboard/domain/finance_models.dart';
import '../../transactions/presentation/transactions_panels.dart';

class AccountsPanel extends StatelessWidget {
  const AccountsPanel(
      {required this.bills, required this.categories, super.key});

  final List<MoneyLine> bills;
  final List<CategoryModel> categories;

  @override
  Widget build(BuildContext context) {
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SectionTitle(
              eyebrow: 'Compromissos mensais', title: 'Contas fixas'),
          const Wrap(
            spacing: 14,
            runSpacing: 14,
            crossAxisAlignment: WrapCrossAlignment.end,
            children: [
              SizedBox(
                  width: 230,
                  child: AppTextField(
                      label: 'Nome da conta', hint: 'Ex: aluguel, internet')),
              SizedBox(
                  width: 140,
                  child: AppTextField(label: 'Valor', hint: '0,00')),
              SizedBox(
                  width: 130,
                  child: AppTextField(label: 'Vencimento', initialValue: '5')),
              SizedBox(
                  width: 180,
                  child: AppTextField(
                      label: 'Categoria', initialValue: 'Sem categoria')),
              AppButton(label: 'Adicionar conta'),
            ],
          ),
          const SizedBox(height: 18),
          if (bills.isEmpty)
            const EmptyState(
                'Cadastre suas contas fixas para o limite diário ficar mais realista.')
          else
            ...bills.map((item) => MoneyListItem(item: item, positive: true)),
        ],
      ),
    );
  }
}
