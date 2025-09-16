class Subscription {
  final int id;
  final String name;
  final String category;
  final double price;
  final double yearCost;
  final bool isActive;
  final DateTime createdAt;
  final int daysBeforeNextPayment;
  final DateTime paymentDate;
  final String userId;

  const Subscription({
    required this.id,
    required this.name,
    required this.category,
    required this.price,
    required this.yearCost,
    required this.isActive,
    required this.createdAt,
    required this.daysBeforeNextPayment,
    required this.paymentDate,
    required this.userId,
  });

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
    return other is Subscription && other.id == id;
  }

  @override
  int get hashCode => id.hashCode;

  @override
  String toString() => 'Subscription(id: $id, name: $name, price: $price)';
}