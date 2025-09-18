import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../providers/subscription_prodivers.dart';
import '../home/home_page.dart';

class LoginPage extends ConsumerWidget {
  const LoginPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final authState = ref.watch(authNotifierProvider);
    ref.listen(authNotifierProvider, (previous, next) {
      if (next.isAuthenticated) {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (context) => HomePage()),
        );
      }

      if (next.errorMessage != null) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(next.errorMessage!)));
      }
    });

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
