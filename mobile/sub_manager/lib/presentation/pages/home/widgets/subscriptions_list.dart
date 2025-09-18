import 'package:flutter/material.dart';

import '../../../../domain/entities/subscription.dart';
import 'subscription_card.dart';

class SubscriptionsList extends StatelessWidget {
  final List<Subscription> subscriptions;
  final String title;
  final Function(Subscription, bool)? onToggleSubscription;
  final Function(Subscription, String)? onMenuAction;

  const SubscriptionsList({
    super.key,
    required this.subscriptions,
    this.title = 'Your Subscriptions',
    this.onToggleSubscription,
    this.onMenuAction,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              "Your Subscriptions",
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            CircleAvatar(
              backgroundColor: Colors.red[600],
              radius: 14,
              child: Text(
                subscriptions.length.toString(),
                style: TextStyle(color: Colors.white),
              ),
            ),
          ],
        ),

        const SizedBox(height: 10),
        // Sub lists
        ...subscriptions.map(
          (subscription) => SubscriptionCard(
            subscription: subscription,
            onToggle:
                (isActive) =>
                    onToggleSubscription?.call(subscription, isActive),
            onMenuAction: (action) => onMenuAction?.call(subscription, action),
          ),
        ),
      ],
    );
  }
}
