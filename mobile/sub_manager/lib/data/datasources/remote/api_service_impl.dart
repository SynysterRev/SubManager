import 'package:dio/dio.dart';

import '../../../core/errors/exceptions.dart';
import '../../../core/storage/token_storage.dart';
import '../../models/paginated_response_model.dart';
import '../../models/subscriptions_response_model.dart';
import 'api_service.dart';

class ApiServiceImpl implements ApiService {
  final Dio dio;

  ApiServiceImpl(this.dio);

  @override
  Future<PaginatedResponseModel<SubscriptionsResponseModel>> getSubscriptions({
    int page = 1,
  }) async {
    try {
      final response = await dio.get(
        '/api/me/subscriptions',
        queryParameters: {'pageIndex': page},
      );

      return PaginatedResponseModel.fromJson(
        response.data,
        (json) =>
            SubscriptionsResponseModel.fromJson(json as Map<String, dynamic>),
      );
    } catch (e) {
      throw ServerException('Erreur lors de la récupération des abonnements');
    }
  }

  @override
  Future<void> login(String email, String password) async {
    final response = await dio.post(
      '/api/login',
      data: {'email': email, 'password': password},
    );

    if (response.statusCode == 200) {
      final token = response.data['token'];
      await TokenStorage.saveToken(token);
      print('✅ Token sauvegardé');
    } else {
      throw Exception('Échec du login');
    }
  }

  @override
  Future<void> register(String email, String password) async {
    final response = await dio.post(
      '/api/register',
      data: {'email': email, 'password': password, 'confirmPassword': password},
      options: Options(headers: {'Content-Type': 'application/json'}),
    );

    if (response.statusCode == 200) {
      final data = response.data; // Map<String, dynamic>
      final token = data['token'] as String;
      final expiration = DateTime.parse(data['expiration'] as String);
      final isPremium = data['isPremium'] as bool;

      // Sauvegarde du token et des infos utilisateur
      await TokenStorage.saveToken(token);
      await TokenStorage.saveIsPremium(isPremium);
      print('✅ Token sauvegardé, expire le $expiration');
    } else {
      throw Exception('Échec de l’inscription');
    }
  }

  @override
  Future<String> refreshToken() async {
    try {
      final response = await dio.post(
        '/api/refresh-token',
        options: Options(
          // Important : si ton API utilise les cookies pour le refresh token
          // il faut envoyer les cookies, donc pas de "Authorization" ici
          contentType: 'application/json',
        ),
      );

      final newToken = response.data['token'] ?? response.data['accessToken'];
      if (newToken != null) {
        await TokenStorage.saveToken(newToken);
        return newToken;
      } else {
        throw Exception('Refresh token failed');
      }
    } catch (e) {
      throw Exception('Refresh token failed: $e');
    }
  }
}
