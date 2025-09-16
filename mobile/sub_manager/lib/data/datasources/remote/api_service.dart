import '../../models/paginated_response_model.dart';
import '../../models/subscriptions_response_model.dart';

abstract class ApiService {
  Future<PaginatedResponseModel<SubscriptionsResponseModel>> getSubscriptions({
    int page = 1,
  });

  Future<void> login(String email, String password);
  Future<void> register(String email, String password);
  Future<String> refreshToken();
}
