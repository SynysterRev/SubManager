import 'package:dartz/dartz.dart';

import '../../core/errors/failures.dart';
import '../entities/subscription.dart';
import '../repositories/subscription_repository.dart';

class GetUserSubscriptions {
  final SubscriptionRepository repository;

  GetUserSubscriptions(this.repository);

  Future<Either<Failure, List<Subscription>>> call() async {
    return await repository.getSubscriptions();
  }
}
