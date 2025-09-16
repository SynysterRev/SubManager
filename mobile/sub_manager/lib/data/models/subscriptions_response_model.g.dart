// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscriptions_response_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionsResponseModel _$SubscriptionsResponseModelFromJson(
  Map<String, dynamic> json,
) => SubscriptionsResponseModel(
  subscriptions:
      (json['subscriptions'] as List<dynamic>)
          .map((e) => SubscriptionModel.fromJson(e as Map<String, dynamic>))
          .toList(),
);

Map<String, dynamic> _$SubscriptionsResponseModelToJson(
  SubscriptionsResponseModel instance,
) => <String, dynamic>{'subscriptions': instance.subscriptions};
