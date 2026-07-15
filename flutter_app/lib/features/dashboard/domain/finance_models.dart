class CategoryModel {
  const CategoryModel({
    required this.id,
    required this.name,
    required this.type,
    required this.color,
    this.isActive = true,
    this.isDefault = false,
  });

  final String id;
  final String name;
  final CategoryType type;
  final int color;
  final bool isActive;
  final bool isDefault;
}

enum CategoryType { income, expense, bill, goal }

class MoneyLine {
  const MoneyLine({
    required this.title,
    required this.subtitle,
    required this.amount,
    this.categoryId,
    this.trailing,
  });

  final String title;
  final String subtitle;
  final double amount;
  final String? categoryId;
  final String? trailing;
}

class GoalModel {
  const GoalModel({
    required this.name,
    required this.currentAmount,
    required this.targetAmount,
    required this.monthlyPlannedAmount,
    this.categoryId,
    this.completed = false,
  });

  final String name;
  final double currentAmount;
  final double targetAmount;
  final double monthlyPlannedAmount;
  final String? categoryId;
  final bool completed;

  int get percent => targetAmount <= 0
      ? 0
      : ((currentAmount / targetAmount) * 100).clamp(0, 100).round();
}

class DashboardSnapshot {
  const DashboardSnapshot({
    required this.userName,
    required this.cycleLabel,
    required this.safeLimit,
    required this.safeMessage,
    required this.incomeTotal,
    required this.fixedBillsTotal,
    required this.expensesTotal,
    required this.goalsTotal,
    required this.availableBalance,
    required this.incomes,
    required this.expenses,
    required this.bills,
    required this.goals,
    required this.categories,
    required this.offline,
  });

  final String userName;
  final String cycleLabel;
  final double safeLimit;
  final String safeMessage;
  final double incomeTotal;
  final double fixedBillsTotal;
  final double expensesTotal;
  final double goalsTotal;
  final double availableBalance;
  final List<MoneyLine> incomes;
  final List<MoneyLine> expenses;
  final List<MoneyLine> bills;
  final List<GoalModel> goals;
  final List<CategoryModel> categories;
  final bool offline;

  DashboardSnapshot copyWith({bool? offline}) {
    return DashboardSnapshot(
      userName: userName,
      cycleLabel: cycleLabel,
      safeLimit: safeLimit,
      safeMessage: safeMessage,
      incomeTotal: incomeTotal,
      fixedBillsTotal: fixedBillsTotal,
      expensesTotal: expensesTotal,
      goalsTotal: goalsTotal,
      availableBalance: availableBalance,
      incomes: incomes,
      expenses: expenses,
      bills: bills,
      goals: goals,
      categories: categories,
      offline: offline ?? this.offline,
    );
  }
}

enum ScreenLoadStatus {
  loading,
  success,
  empty,
  connectionError,
  serverUnavailable,
  expiredSession,
  offline,
}
