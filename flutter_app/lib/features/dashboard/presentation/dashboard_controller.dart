import 'package:flutter/foundation.dart';

import '../data/mock_finance_data.dart';
import '../data/dashboard_repository.dart';
import '../domain/finance_models.dart';

class DashboardController extends ChangeNotifier {
  DashboardController({DashboardRepository? repository})
      : _repository = repository ?? MockDashboardRepository();

  final DashboardRepository _repository;

  ScreenLoadStatus status = ScreenLoadStatus.loading;
  DashboardSnapshot? snapshot;
  String? toast;

  Future<void> loadMock({ScreenLoadStatus forcedStatus = ScreenLoadStatus.success}) async {
    status = ScreenLoadStatus.loading;
    notifyListeners();
    await Future<void>.delayed(const Duration(milliseconds: 350));

    status = forcedStatus;
    if (forcedStatus == ScreenLoadStatus.success || forcedStatus == ScreenLoadStatus.offline) {
      snapshot = forcedStatus == ScreenLoadStatus.success
          ? await _repository.getCurrentDashboard()
          : MockFinanceData.snapshot.copyWith(offline: true);
    }
    notifyListeners();
  }

  void showToast(String message) {
    toast = message;
    notifyListeners();
  }

  void clearToast() {
    toast = null;
    notifyListeners();
  }
}
