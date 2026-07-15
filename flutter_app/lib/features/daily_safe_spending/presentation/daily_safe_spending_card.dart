import 'package:flutter/material.dart';

import '../../../core/theme/app_tokens.dart';
import '../../../core/widgets/app_panel.dart';
import '../../../core/widgets/section_title.dart';
import '../../dashboard/domain/finance_models.dart';
import '../../shared/formatters.dart';

class DailySafeSpendingCard extends StatelessWidget {
  const DailySafeSpendingCard({required this.snapshot, super.key});

  final DashboardSnapshot snapshot;

  @override
  Widget build(BuildContext context) {
    final narrow = MediaQuery.sizeOf(context).width <= 560;
    return AppPanel(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: double.infinity,
            alignment: Alignment.centerLeft,
            child: DecoratedBox(
              decoration: BoxDecoration(
                color: const Color(0x1f1f6f4a),
                borderRadius: BorderRadius.circular(AppTokens.radiusPill),
              ),
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                child: Text(
                  snapshot.cycleLabel,
                  style: const TextStyle(
                    color: AppTokens.brandStrong,
                    fontFamily: 'Trebuchet MS',
                    fontSize: 12,
                    fontWeight: FontWeight.w800,
                  ),
                ),
              ),
            ),
          ),
          const SizedBox(height: AppTokens.gapLg),
          const MutedLabel('Você pode gastar hoje'),
          const SizedBox(height: 4),
          Text(
            Formatters.currency(snapshot.safeLimit),
            style: Theme.of(context).textTheme.displayLarge?.copyWith(
                  color: AppTokens.brandStrong,
                  fontSize: narrow ? 52 : 86,
                ),
          ),
          const SizedBox(height: 12),
          Text(snapshot.safeMessage, style: Theme.of(context).textTheme.bodyLarge),
        ],
      ),
    );
  }
}
