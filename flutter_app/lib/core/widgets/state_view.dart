import 'package:flutter/material.dart';

import '../../features/dashboard/domain/finance_models.dart';
import '../theme/app_tokens.dart';
import 'app_button.dart';
import 'app_panel.dart';
import 'section_title.dart';

class StateView extends StatelessWidget {
  const StateView({
    required this.status,
    required this.onRetry,
    super.key,
  });

  final ScreenLoadStatus status;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    if (status == ScreenLoadStatus.loading) {
      return const AppPanel(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            CircularProgressIndicator(color: AppTokens.brand),
            SizedBox(height: 14),
            Text('Carregando seus dados...'),
          ],
        ),
      );
    }

    final message = switch (status) {
      ScreenLoadStatus.empty => 'Ainda não há dados para mostrar.',
      ScreenLoadStatus.connectionError => 'Não foi possível carregar seus dados.',
      ScreenLoadStatus.serverUnavailable => 'O servidor está indisponível no momento.',
      ScreenLoadStatus.expiredSession => 'Sua sessão expirou. Entre novamente para continuar.',
      ScreenLoadStatus.offline => 'Você está offline. Mostrando os últimos dados salvos.',
      ScreenLoadStatus.success || ScreenLoadStatus.loading => 'Não foi possível carregar seus dados.',
    };

    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const SectionTitle(eyebrow: 'Estado da tela', title: 'Algo precisa de atenção'),
          Text(
            message,
            style: Theme.of(context).textTheme.bodyLarge,
          ),
          const SizedBox(height: 18),
          AppButton(label: 'Tentar novamente', tone: AppButtonTone.ghost, onPressed: onRetry),
        ],
      ),
    );
  }
}
