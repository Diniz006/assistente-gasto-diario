import 'package:flutter/material.dart';

import 'core/theme/app_theme.dart';
import 'features/dashboard/presentation/dashboard_controller.dart';
import 'features/dashboard/presentation/finance_app.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final controller = DashboardController();
  await controller.loadMock();

  runApp(AssistenteGastoDiarioApp(controller: controller));
}

class AssistenteGastoDiarioApp extends StatelessWidget {
  const AssistenteGastoDiarioApp({required this.controller, super.key});

  final DashboardController controller;

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Assistente de Gasto Diario',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.light(),
      home: FinanceApp(controller: controller),
    );
  }
}
