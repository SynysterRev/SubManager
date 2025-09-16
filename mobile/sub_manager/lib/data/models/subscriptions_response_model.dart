import 'package:json_annotation/json_annotation.dart';
import 'subscription_model.dart';

part 'subscriptions_response_model.g.dart';

@JsonSerializable()
class SubscriptionsResponseModel {
  final List<SubscriptionModel> subscriptions;

  const SubscriptionsResponseModel({
    required this.subscriptions,
  });

  factory SubscriptionsResponseModel.fromJson(Map<String, dynamic> json) =>
      _$SubscriptionsResponseModelFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionsResponseModelToJson(this);
}