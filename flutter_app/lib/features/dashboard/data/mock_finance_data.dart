import '../domain/finance_models.dart';

abstract final class MockFinanceData {
  static const categories = [
    CategoryModel(id: 'salario', name: 'Salario', type: CategoryType.income, color: 0xff2f855a, isDefault: true),
    CategoryModel(id: 'freela', name: 'Outras rendas', type: CategoryType.income, color: 0xff38a169, isDefault: true),
    CategoryModel(id: 'mercado', name: 'Mercado', type: CategoryType.expense, color: 0xffdd6b20, isDefault: true),
    CategoryModel(id: 'transporte', name: 'Transporte', type: CategoryType.expense, color: 0xff3182ce, isDefault: true),
    CategoryModel(id: 'lazer', name: 'Lazer', type: CategoryType.expense, color: 0xff805ad5, isDefault: true),
    CategoryModel(id: 'moradia', name: 'Moradia', type: CategoryType.bill, color: 0xffc53030, isDefault: true),
    CategoryModel(id: 'servicos', name: 'Servicos', type: CategoryType.bill, color: 0xffd69e2e, isDefault: true),
    CategoryModel(id: 'reserva', name: 'Reserva de emergencia', type: CategoryType.goal, color: 0xff319795, isDefault: true),
  ];

  static const snapshot = DashboardSnapshot(
    userName: 'Dini',
    cycleLabel: 'Ciclo 01/07 - 31/07',
    safeLimit: 86.35,
    safeMessage: 'Hoje você pode gastar até R$ 86,35 sem comprometer suas contas e metas.',
    incomeTotal: 4200,
    fixedBillsTotal: 1510,
    expensesTotal: 962.50,
    goalsTotal: 650,
    availableBalance: 1077.50,
    incomes: [
      MoneyLine(title: 'Salario', subtitle: '14/07 · Salario', amount: 3800, categoryId: 'salario'),
      MoneyLine(title: 'Freelance', subtitle: '10/07 · Outras rendas', amount: 400, categoryId: 'freela'),
    ],
    expenses: [
      MoneyLine(title: 'Mercado da semana', subtitle: '14/07 · Mercado · Pix', amount: 248.90, categoryId: 'mercado'),
      MoneyLine(title: 'Uber', subtitle: '13/07 · Transporte · Debito', amount: 34.60, categoryId: 'transporte'),
      MoneyLine(title: 'Cafe e lanche', subtitle: '12/07 · Lazer · Pix', amount: 42.00, categoryId: 'lazer'),
    ],
    bills: [
      MoneyLine(title: 'Aluguel', subtitle: 'Vence dia 5 · Moradia', amount: 1100, categoryId: 'moradia'),
      MoneyLine(title: 'Internet', subtitle: 'Vence dia 12 · Servicos', amount: 110, categoryId: 'servicos'),
      MoneyLine(title: 'Energia', subtitle: 'Vence dia 18 · Servicos', amount: 300, categoryId: 'servicos'),
    ],
    goals: [
      GoalModel(
        name: 'Reserva de emergencia',
        currentAmount: 2800,
        targetAmount: 8000,
        monthlyPlannedAmount: 500,
        categoryId: 'reserva',
      ),
      GoalModel(
        name: 'Notebook',
        currentAmount: 900,
        targetAmount: 4500,
        monthlyPlannedAmount: 150,
        categoryId: 'reserva',
      ),
    ],
    categories: categories,
    offline: false,
  );
}
