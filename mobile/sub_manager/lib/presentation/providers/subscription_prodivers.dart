import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:dio/dio.dart';
import '../../core/network/dio_client.dart';
import '../../data/datasources/remote/api_service.dart';
import '../../data/datasources/remote/api_service_impl.dart';
import '../../data/repositories/auth_repository_impl.dart';
import '../../data/repositories/subscription_repository_impl.dart';
import '../../domain/repositories/auth_repository.dart';
import '../../domain/repositories/subscription_repository.dart';
import '../../domain/entities/subscription.dart';
import '../../domain/usecases/login.dart';
import '../../domain/usecases/register.dart';
import 'auth_provider.dart';

// Provider pour Dio
final dioProvider = Provider<Dio>((ref) {
  final dioClient = DioClient();
  return dioClient.dio;
});

// Provider pour ApiService
final apiServiceProvider = Provider<ApiService>((ref) {
  final dio = ref.watch(dioProvider);
  return ApiServiceImpl(dio);
});

// Provider pour Repository
final subscriptionRepositoryProvider = Provider<SubscriptionRepository>((ref) {
  final apiService = ref.watch(apiServiceProvider);
  return SubscriptionRepositoryImpl(apiService: apiService);
});

// Provider pour récupérer les subscriptions
final subscriptionsProvider = FutureProvider<List<Subscription>>((ref) async {
  final repository = ref.watch(subscriptionRepositoryProvider);
  final result = await repository.getSubscriptions();

  return result.fold(
    (failure) => throw Exception(failure.message),
    (subscriptions) => subscriptions,
  );
});

final authRepositoryProvider = Provider<AuthRepository>((ref) {
  final apiService = ref.watch(apiServiceProvider);
  return AuthRepositoryImpl(apiService);
});

final loginUseCaseProvider = Provider<Login>((ref) {
  final repository = ref.watch(authRepositoryProvider);
  return Login(repository);
});

final registerUseCaseProvider = Provider<Register>((ref) {
  final repository = ref.watch(authRepositoryProvider);
  return Register(repository);
});

final authNotifierProvider = StateNotifierProvider<AuthNotifier, AuthState>((
  ref,
) {
  final login = ref.watch(loginUseCaseProvider);
  final register = ref.watch(registerUseCaseProvider);
  return AuthNotifier(loginUseCase: login, registerUseCase: register);
});
