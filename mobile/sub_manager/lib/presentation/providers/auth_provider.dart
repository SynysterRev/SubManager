import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../data/repositories/auth_repository_impl.dart';
import '../../domain/repositories/auth_repository.dart';
import '../../domain/usecases/login.dart';
import '../../domain/usecases/register.dart';
import 'subscription_prodivers.dart';

class AuthState {
  final bool isLoading;
  final String? errorMessage;
  final bool isAuthenticated;

  AuthState({
    this.isLoading = false,
    this.errorMessage,
    this.isAuthenticated = false,
  });

  AuthState copyWith({
    bool? isLoading,
    String? errorMessage,
    bool? isAuthenticated,
  }) {
    return AuthState(
      isLoading: isLoading ?? this.isLoading,
      errorMessage: errorMessage,
      isAuthenticated: isAuthenticated ?? this.isAuthenticated,
    );
  }
}

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

class AuthNotifier extends StateNotifier<AuthState> {
  final Login loginUseCase;
  final Register registerUseCase;

  AuthNotifier({required this.loginUseCase, required this.registerUseCase})
    : super(AuthState());

  Future<void> login(String email, String password) async {
    state = state.copyWith(isLoading: true, errorMessage: null);

    final result = await loginUseCase(email, password);

    result.fold(
      (failure) =>
          state = state.copyWith(
            isLoading: false,
            errorMessage: failure.message,
          ),
      (_) => state = state.copyWith(isLoading: false, isAuthenticated: true),
    );
  }

  Future<void> register(String email, String password) async {
    state = state.copyWith(isLoading: true, errorMessage: null);

    final result = await registerUseCase(email, password);

    result.fold(
      (failure) =>
          state = state.copyWith(
            isLoading: false,
            errorMessage: failure.message,
          ),
      (_) => state = state.copyWith(isLoading: false, isAuthenticated: true),
    );
  }
}
