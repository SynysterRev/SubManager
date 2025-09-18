import 'package:flutter/material.dart';

import '../../../domain/entities/subscription.dart';
import 'widgets/filter_chip.dart';
import 'widgets/stat_card.dart';
import 'widgets/subscriptions_list.dart';

class HomePage extends StatelessWidget {
  HomePage({super.key});

  final subscriptions = [
    Subscription(
      id: 1,
      name: 'Netflix',
      category: 'Entertainment',
      price: 15.99,
      yearCost: 192,
      daysBeforeNextPayment: 27,
      paymentDate: DateTime.now(),
      isActive: true,
      userId: "azeq",
      createdAt: DateTime.now()
    ),
    Subscription(
      id: 2,
      name: 'Spotify',
      category: 'Entertainment',
      price: 15.99,
      yearCost: 192,
      daysBeforeNextPayment: 27,
      paymentDate: DateTime.now(),
      isActive: true,
      userId: "azeq",
      createdAt: DateTime.now()
    ),
  ];

  @override
  Widget build(BuildContext context) {    
    return Scaffold(
      appBar: AppBar(
        title: const Text("SubTracker"),
        actions: [
          IconButton(onPressed: () {}, icon: const Icon(Icons.search)),
          IconButton(onPressed: () {}, icon: const Icon(Icons.settings)),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // --- Total section ---
            Row(
              children: [
                Expanded(
                  child: StatCard(
                    title: "Monthly Total",
                    value: "\$101.96",
                    subtitle: "Per Year \$1224",
                    color: Colors.green.shade100,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: StatCard(
                    title: "Active Subscriptions",
                    value: "5",
                    subtitle: "6 Total",
                    color: Colors.purple.shade100,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 20),

            // --- Filter ---
            Row(
              children: [
                FilterChipWidget(label: "All", selected: true),
                const SizedBox(width: 8),
                FilterChipWidget(label: "Entertainment"),
                const SizedBox(width: 8),
                FilterChipWidget(label: "Music & Audio"),
              ],
            ),
            const SizedBox(height: 20),

            // --- Sub list ---
            SubscriptionsList(
              subscriptions: subscriptions,
              onToggleSubscription: (subscription, isActive) {
                print('${subscription.name} toggled: $isActive');
              },
              onMenuAction: (subscription, action) {
                print('Action $action on ${subscription.name}');
              },
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {},
        child: const Icon(Icons.add),
      ),
    );
  }
}
