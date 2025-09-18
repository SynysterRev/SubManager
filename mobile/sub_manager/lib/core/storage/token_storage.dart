import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../../domain/entities/token.dart';

class TokenStorage {
  static const _storage = FlutterSecureStorage();
  static const _keyToken = 'jwt_token';
  static const _keyExpiration = 'jwt_expiration';
  static const _keyIsPremium = 'is_premium';
  static const _keyEmail = 'email';

  static Future<void> saveToken(Token token) async {
    await _storage.write(key: _keyToken, value: token.token);
    await _storage.write(
      key: _keyExpiration,
      value: token.expiration.toIso8601String(),
    );
    await _storage.write(key: _keyIsPremium, value: token.isPremium.toString());
    if (token.email != null) {
      await _storage.write(key: _keyEmail, value: token.email!);
    }
  }

  static Future<Token?> getToken() async {
    final token = await _storage.read(key: _keyToken);
    final expirationStr = await _storage.read(key: _keyExpiration);
    if (token == null || expirationStr == null) return null;

    final isPremiumStr = await _storage.read(key: _keyIsPremium);
    final email = await _storage.read(key: _keyEmail);

    return Token(
      token: token,
      expiration: DateTime.parse(expirationStr),
      isPremium: isPremiumStr == 'true',
      email: email,
    );
  }

  static Future<void> deleteToken() async {
    await _storage.delete(key: _keyToken);
    await _storage.delete(key: _keyExpiration);
    await _storage.delete(key: _keyIsPremium);
    await _storage.delete(key: _keyEmail);
  }
}
