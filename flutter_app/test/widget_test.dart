import 'package:assistente_gasto_diario_flutter/features/dashboard/presentation/dashboard_controller.dart';
import 'package:assistente_gasto_diario_flutter/main.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('shows the authentication entry screen', (tester) async {
    final controller = DashboardController();

    await tester.pumpWidget(AssistenteGastoDiarioApp(controller: controller));
    await tester.pump(const Duration(milliseconds: 800));

    expect(find.text('Quanto posso gastar hoje?'), findsOneWidget);
    expect(find.text('Entrar no painel'), findsOneWidget);
    expect(find.text('Criar conta'), findsOneWidget);
  });
}
