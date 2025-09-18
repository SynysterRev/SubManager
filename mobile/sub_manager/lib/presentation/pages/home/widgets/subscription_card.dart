import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:sub_manager/domain/entities/subscription.dart';

class SubscriptionCard extends StatelessWidget {
  final Subscription subscription;
  final Function(bool)? onToggle;
  final Function(String)? onMenuAction;

  const SubscriptionCard({
    super.key,
    required this.subscription,
    this.onToggle,
    this.onMenuAction,
  });

  @override
  Widget build(BuildContext context) {
    Locale userLocale = Localizations.localeOf(context);
    String userFormattedDate = DateFormat.yMd(userLocale.toString()).format(subscription.paymentDate);

    return Card(
      margin: const EdgeInsets.symmetric(vertical: 8),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            // Top Row for Logo, Name, Price, and Menu
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    CircleAvatar(
                      backgroundColor: Colors.purple.withValues(
                        alpha: 0.15,
                      ),
                      radius: 20,
                      // child: Image.asset(
                      //   subscription.logo,
                      //   color: subscription.logoColor,
                      //   width: 24,
                      //   height: 24,
                      // ),
                    ),
                    const SizedBox(width: 12),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          subscription.name,
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 16,
                          ),
                        ),
                        Text(
                          subscription.category,
                          style: TextStyle(
                            color: Colors.grey[600],
                            fontSize: 14,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
                Row(
                  children: [
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Text(
                          '\$${subscription.price.toStringAsFixed(2)}',
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 16,
                          ),
                        ),
                        Text(
                          '/month',
                          style: TextStyle(
                            color: Colors.grey[600],
                            fontSize: 12,
                          ),
                        ),
                      ],
                    ),
                    IconButton(
                      icon: const Icon(Icons.more_vert),
                      onPressed: () {
                        // Implement menu action
                      },
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: 16),
            // Middle Row for Days Remaining and Day of billing cycle
            Column(
              mainAxisAlignment: MainAxisAlignment.center,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 8,
                    vertical: 4,
                  ),
                  decoration: BoxDecoration(
                    color: Colors.green.withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    'Due in ${subscription.daysBeforeNextPayment} days',
                    style: TextStyle(
                      color: Colors.green,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    Icon(
                      Icons.calendar_today,
                      size: 14.0,
                      color: Colors.grey[600],
                    ),
                    SizedBox(width: 5.0),
                    Text(
                      'Monthly on $userFormattedDate',
                      style: TextStyle(color: Colors.grey[600]),
                    ),
                    SizedBox(width: 15.0),
                    Text(
                      '\$${subscription.yearCost.toStringAsFixed(0)}/yr',
                      style: TextStyle(color: Colors.grey[600]),
                    ),
                  ],
                ),
                Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    // Switch button
                    Switch(
                      value: subscription.isActive,
                      onChanged: onToggle,
                      activeColor: Colors.blue, // Change to your accent color
                    ),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
