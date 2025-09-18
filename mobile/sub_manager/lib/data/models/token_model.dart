import '../../domain/entities/token.dart';

class TokenModel {
  final String email;
  final String token;
  final DateTime expiration;
  final bool isPremium;

  TokenModel({
    required this.email,
    required this.token,
    required this.expiration,
    required this.isPremium,
  });

  factory TokenModel.fromJson(Map<String, dynamic> json) => TokenModel(
    email: json['email'] ?? '',
    token: json['token'] ?? '',
    expiration: DateTime.parse(json['expiration']),
    isPremium: json['isPremium'] ?? false,
  );

  Map<String, dynamic> toJson() => {
    'email': email,
    'token': token,
    'expiration': expiration.toIso8601String(),
    'isPremium': isPremium,
  };

  // Convert to entity
  Token toEntity() => Token(
    token: token,
    expiration: expiration,
    isPremium: isPremium,
    email: email,
  );

  // Create from entity
  factory TokenModel.fromEntity(Token entity) => TokenModel(
    token: entity.token,
    expiration: entity.expiration,
    isPremium: entity.isPremium,
    email: entity.email ?? '',
  );
}
