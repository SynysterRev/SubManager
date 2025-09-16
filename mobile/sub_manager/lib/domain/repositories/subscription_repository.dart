import 'package:dartz/dartz.dart';

import '../../core/errors/failures.dart';
import '../entities/subscription.dart';

abstract class SubscriptionRepository {
  Future<Either<Failure, List<Subscription>>> getSubscriptions({int page = 1});
}
