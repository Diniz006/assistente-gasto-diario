import 'dart:convert';

import 'package:shared_preferences/shared_preferences.dart';

class LocalCache {
  LocalCache(this._preferences);

  final SharedPreferences _preferences;

  static Future<LocalCache> create() async {
    final preferences = await SharedPreferences.getInstance();
    return LocalCache(preferences);
  }

  Future<void> saveJson(String key, Map<String, dynamic> value) async {
    await _preferences.setString(key, jsonEncode(value));
  }

  Map<String, dynamic>? readJson(String key) {
    final raw = _preferences.getString(key);
    if (raw == null) {
      return null;
    }
    final decoded = jsonDecode(raw);
    return decoded is Map<String, dynamic> ? decoded : null;
  }

  Future<void> saveString(String key, String value) =>
      _preferences.setString(key, value);

  String? readString(String key) => _preferences.getString(key);

  Future<void> clearSession() async {
    await _preferences.remove('auth.token');
    await _preferences.remove('auth.user');
  }
}
