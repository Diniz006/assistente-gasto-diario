import 'package:flutter/material.dart';

import '../theme/app_tokens.dart';

class MutedLabel extends StatelessWidget {
  const MutedLabel(this.text, {super.key});

  final String text;

  @override
  Widget build(BuildContext context) {
    return Text(
      text.toUpperCase(),
      style: const TextStyle(
        color: AppTokens.muted,
        fontFamily: 'Trebuchet MS',
        fontSize: 12,
        fontWeight: FontWeight.w700,
        letterSpacing: 0.96,
      ),
    );
  }
}

class SectionTitle extends StatelessWidget {
  const SectionTitle({
    required this.eyebrow,
    required this.title,
    this.trailing,
    super.key,
  });

  final String eyebrow;
  final String title;
  final Widget? trailing;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: AppTokens.gapLg),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                MutedLabel(eyebrow),
                const SizedBox(height: 4),
                Text(title, style: Theme.of(context).textTheme.headlineMedium),
              ],
            ),
          ),
          if (trailing != null) trailing!,
        ],
      ),
    );
  }
}
