import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../domain/entities/subscription.dart';
import '../../../providers/subscription_prodivers.dart';
import 'subscription_card.dart';

class SubscriptionsList extends ConsumerWidget {
  final Function(Subscription, bool)? onToggleSubscription;
  final Function(Subscription, String)? onMenuAction;

  const SubscriptionsList({
    super.key,
    this.onToggleSubscription,
    this.onMenuAction,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final subscriptionsAsync = ref.watch(subscriptionsProvider);

    return subscriptionsAsync.when(
      data: (subscriptions) {
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
                    style: const TextStyle(color: Colors.white),
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
                onMenuAction:
                    (action) => onMenuAction?.call(subscription, action),
              ),
            ),
          ],
        );
      },
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, stack) => Center(child: Text('Erreur: $err')),
    );
  }
}
