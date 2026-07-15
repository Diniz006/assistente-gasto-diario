import '../../../core/network/api_client.dart';
import '../../../core/storage/local_cache.dart';

class AuthRepository {
  AuthRepository({
    required ApiClient apiClient,
    required LocalCache cache,
  })  : _apiClient = apiClient,
        _cache = cache;

  final ApiClient _apiClient;
  final LocalCache _cache;

  Future<void> login(String email, String password) async {
    final payload = await _apiClient.postJson('/api/auth/login', {
      'email': email,
      'password': password,
    });
    final token = payload['accessToken'];
    final user = payload['user'];
    if (token is String) {
      _apiClient.token = token;
      await _cache.saveString('auth.token', token);
    }
    if (user is Map<String, dynamic>) {
      await _cache.saveJson('auth.user', user);
    }
  }

  Future<void> register(String name, String email, String password) async {
    await _apiClient.postJson('/api/auth/register', {
      'name': name,
      'email': email,
      'password': password,
    });
    await login(email, password);
  }

  Future<void> logout() async {
    _apiClient.token = null;
    await _cache.clearSession();
  }
}
