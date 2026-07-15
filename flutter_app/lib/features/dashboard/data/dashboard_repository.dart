import '../../../core/network/api_client.dart';
import '../../../core/storage/local_cache.dart';
import '../domain/finance_models.dart';
import 'mock_finance_data.dart';

abstract interface class DashboardRepository {
  Future<DashboardSnapshot> getCurrentDashboard();
}

class MockDashboardRepository implements DashboardRepository {
  @override
  Future<DashboardSnapshot> getCurrentDashboard() async {
    await Future<void>.delayed(const Duration(milliseconds: 350));
    return MockFinanceData.snapshot;
  }
}

class ApiDashboardRepository implements DashboardRepository {
  ApiDashboardRepository({
    required ApiClient apiClient,
    required LocalCache cache,
  })  : _apiClient = apiClient,
        _cache = cache;

  final ApiClient _apiClient;
  final LocalCache _cache;

  @override
  Future<DashboardSnapshot> getCurrentDashboard() async {
    final payload = await _apiClient.getJson('/api/me/dashboard');
    await _cache.saveJson('dashboard.current', payload);

    // Mapping definitivo entra na Etapa 2. Ate la, o app preserva a UI com dados simulados.
    return MockFinanceData.snapshot;
  }

  DashboardSnapshot? getCachedDashboard() {
    final cached = _cache.readJson('dashboard.current');
    if (cached == null) {
      return null;
    }

    // Mapping definitivo entra na Etapa 2.
    return MockFinanceData.snapshot.copyWith(offline: true);
  }
}
