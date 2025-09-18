import 'package:dio/dio.dart';
import 'package:cookie_jar/cookie_jar.dart';
import 'package:dio_cookie_manager/dio_cookie_manager.dart';
import 'package:sub_manager/domain/entities/token.dart';
import '../storage/token_storage.dart';

class DioClient {
  final Dio _dio;
  final CookieJar _cookieJar;

  DioClient({Dio? dio})
    : _dio =
          dio ??
          Dio(
            BaseOptions(
              baseUrl: 'http://10.0.2.2:5267',
              connectTimeout: const Duration(seconds: 10),
              receiveTimeout: const Duration(seconds: 10),
              responseType: ResponseType.json,
            ),
          ),
      _cookieJar = CookieJar() {
    //  Interceptor handle cookies (refresh token)
    _dio.interceptors.add(CookieManager(_cookieJar));

    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final token = await TokenStorage.getToken();
          if (token != null) {
            if (token.isExpired) {
              final newToken = await refreshToken();
              options.headers['Authorization'] = 'Bearer ${newToken.token}';
              print("token expired");
            } else {
              options.headers['Authorization'] = 'Bearer ${token.token}';
              print("old token");
            }
          }
          return handler.next(options);
        },
        onError: (e, handler) async {
          if (e.response?.statusCode == 401) {
            try {
              final newToken = await refreshToken();
              final opts = e.requestOptions;
              opts.headers['Authorization'] = 'Bearer $newToken';
              final cloneReq = await _dio.request(
                opts.path,
                options: Options(method: opts.method, headers: opts.headers),
                data: opts.data,
                queryParameters: opts.queryParameters,
              );
              return handler.resolve(cloneReq);
            } catch (_) {
              return handler.next(e);
            }
          } else {
            return handler.next(e);
          }
        },
      ),
    );
  }

  Future<Token> refreshToken() async {
    final response = await _dio.post('/api/refresh-token');
    final newToken = response.data['token'] ?? response.data['accessToken'];
    if (newToken != null) {
      await TokenStorage.saveToken(newToken);
      return newToken;
    } else {
      throw Exception('Refresh token failed');
    }
  }

  Dio get dio => _dio;
}
