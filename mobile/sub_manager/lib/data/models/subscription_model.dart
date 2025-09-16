import 'package:json_annotation/json_annotation.dart';

import '../../domain/entities/subscription.dart';

part 'subscription_model.g.dart';

@JsonSerializable()
class SubscriptionModel extends Subscription {
  const SubscriptionModel({
    required super.id,
    required super.name,
    required super.category,
    required super.price,
    required super.yearCost,
    required super.isActive,
    required super.createdAt,
    required super.daysBeforeNextPayment,
    required super.paymentDate,
    required super.userId,
  });

  factory SubscriptionModel.fromJson(Map<String, dynamic> json) =>
      _$SubscriptionModelFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionModelToJson(this);

  Subscription toEntity() => Subscription(
    id: id,
    name: name,
    category: category,
    price: price,
    yearCost: yearCost,
    isActive: isActive,
    createdAt: createdAt,
    daysBeforeNextPayment: daysBeforeNextPayment,
    paymentDate: paymentDate,
    userId: userId,
  );

  factory SubscriptionModel.fromEntity(Subscription subscription) => 
    SubscriptionModel(
      id: subscription.id,
      name: subscription.name,
      category: subscription.category,
      price: subscription.price,
      yearCost: subscription.yearCost,
      isActive: subscription.isActive,
      createdAt: subscription.createdAt,
      daysBeforeNextPayment: subscription.daysBeforeNextPayment,
      paymentDate: subscription.paymentDate,
      userId: subscription.userId,
    );
}