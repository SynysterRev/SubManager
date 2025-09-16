import 'package:dartz/dartz.dart';
import '../../core/errors/failures.dart';
import '../../core/errors/exceptions.dart';
import '../../domain/entities/subscription.dart';
import '../../domain/repositories/subscription_repository.dart';
import '../datasources/remote/api_service.dart';

class SubscriptionRepositoryImpl implements SubscriptionRepository {
  final ApiService apiService;

  SubscriptionRepositoryImpl({required this.apiService});

  @override
  Future<Either<Failure, List<Subscription>>> getSubscriptions({
    int page = 1,
  }) async {
    try {
      final paginatedResponse = await apiService.getSubscriptions(page: page);

      // Convertir les models en entities
      final subscriptions =
          paginatedResponse.items?.subscriptions
              .map((model) => model.toEntity())
              .toList() ??
          [];

      return Right(subscriptions);
    } on ServerException catch (e) {
      return Left(ServerFailure(e.message));
    } catch (e) {
      return Left(ServerFailure('Erreur inconnue: ${e.toString()}'));
    }
  }
}
