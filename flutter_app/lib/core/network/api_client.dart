import 'dart:convert';

import 'package:http/http.dart' as http;

import '../error/app_failure.dart';

class ApiClient {
  ApiClient({
    required this.baseUrl,
    http.Client? httpClient,
  }) : _httpClient = httpClient ?? http.Client();

  final Uri baseUrl;
  final http.Client _httpClient;
  String? _token;

  set token(String? value) => _token = value;

  Future<Map<String, dynamic>> getJson(String path) async {
    final response = await _send(() => _httpClient.get(_uri(path), headers: _headers()));
    return _decodeObject(response);
  }

  Future<Map<String, dynamic>> postJson(String path, Map<String, dynamic> body) async {
    final response = await _send(
      () => _httpClient.post(
        _uri(path),
        headers: _headers(),
        body: jsonEncode(body),
      ),
    );
    return _decodeObject(response);
  }

  Uri _uri(String path) => baseUrl.resolve(path);

  Map<String, String> _headers() => {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        if (_token != null) 'Authorization': 'Bearer $_token',
      };

  Future<http.Response> _send(Future<http.Response> Function() request) async {
    try {
      final response = await request();
      if (response.statusCode == 401) {
        throw AppFailure.expiredSession();
      }
      if (response.statusCode >= 500) {
        throw AppFailure.unavailable();
      }
      if (response.statusCode < 200 || response.statusCode >= 300) {
        throw AppFailure.invalidPayload();
      }
      final contentType = response.headers['content-type'] ?? '';
      if (!contentType.toLowerCase().contains('application/json')) {
        throw AppFailure.invalidPayload();
      }
      return response;
    } on AppFailure {
      rethrow;
    } catch (_) {
      throw AppFailure.connection();
    }
  }

  Map<String, dynamic> _decodeObject(http.Response response) {
    try {
      final decoded = jsonDecode(response.body);
      if (decoded is Map<String, dynamic>) {
        return decoded;
      }
      throw AppFailure.invalidPayload();
    } catch (_) {
      throw AppFailure.invalidPayload();
    }
  }
}
