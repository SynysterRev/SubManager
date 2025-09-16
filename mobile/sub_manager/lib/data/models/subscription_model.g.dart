// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionModel _$SubscriptionModelFromJson(Map<String, dynamic> json) =>
    SubscriptionModel(
      id: (json['id'] as num).toInt(),
      name: json['name'] as String,
      category: json['category'] as String,
      price: (json['price'] as num).toDouble(),
      yearCost: (json['yearCost'] as num).toDouble(),
      isActive: json['isActive'] as bool,
      createdAt: DateTime.parse(json['createdAt'] as String),
      daysBeforeNextPayment: (json['daysBeforeNextPayment'] as num).toInt(),
      paymentDate: DateTime.parse(json['paymentDate'] as String),
      userId: json['userId'] as String,
    );

Map<String, dynamic> _$SubscriptionModelToJson(SubscriptionModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'category': instance.category,
      'price': instance.price,
      'yearCost': instance.yearCost,
      'isActive': instance.isActive,
      'createdAt': instance.createdAt.toIso8601String(),
      'daysBeforeNextPayment': instance.daysBeforeNextPayment,
      'paymentDate': instance.paymentDate.toIso8601String(),
      'userId': instance.userId,
    };
