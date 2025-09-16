import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../providers/subscription_prodivers.dart';

class LoginPage extends ConsumerWidget {
  const LoginPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final authState = ref.watch(authNotifierProvider);

    return Scaffold(
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            if (authState.isLoading) const CircularProgressIndicator(),
            if (authState.errorMessage != null)
              Text('Erreur: ${authState.errorMessage}'),
            ElevatedButton(
              onPressed: () {
                ref
                    .read(authNotifierProvider.notifier)
                    .login("new@test.com", "Password123!");
              },
              child: const Text("Se connecter"),
            ),
            ElevatedButton(
              onPressed: () {
                ref
                    .read(authNotifierProvider.notifier)
                    .register("new@test.com", "Password123!");
              },
              child: const Text("S’inscrire"),
            ),
          ],
        ),
      ),
    );
  }
}
