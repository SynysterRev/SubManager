class Token {
  final String token;
  final DateTime expiration;
  final bool isPremium;
  final String? email;

  Token({
    required this.token,
    required this.expiration,
    required this.isPremium,
    this.email,
  });

  bool get isExpired => DateTime.now().isAfter(expiration);
}
