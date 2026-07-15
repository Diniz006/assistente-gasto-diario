import 'package:flutter/material.dart';

import '../theme/app_tokens.dart';

enum AppButtonTone { primary, ghost, tiny, danger }

class AppButton extends StatelessWidget {
  const AppButton({
    required this.label,
    this.onPressed,
    this.tone = AppButtonTone.primary,
    this.busy = false,
    super.key,
  });

  final String label;
  final VoidCallback? onPressed;
  final AppButtonTone tone;
  final bool busy;

  @override
  Widget build(BuildContext context) {
    final isTiny = tone == AppButtonTone.tiny || tone == AppButtonTone.danger;
    final textColor = switch (tone) {
      AppButtonTone.primary => AppTokens.cream,
      AppButtonTone.danger => const Color(0xff8b2f28),
      _ => AppTokens.brandStrong,
    };

    final background = switch (tone) {
      AppButtonTone.primary => null,
      AppButtonTone.danger => const Color(0x24d96b5f),
      _ => const Color(0x171f6f4a),
    };

    return DecoratedBox(
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(AppTokens.radiusPill),
        gradient: tone == AppButtonTone.primary
            ? const LinearGradient(
                colors: [AppTokens.brand, AppTokens.brandStrong],
              )
            : null,
        color: background,
        boxShadow: tone == AppButtonTone.primary
            ? const [
                BoxShadow(
                  color: Color(0x471f6f4a),
                  blurRadius: 28,
                  offset: Offset(0, 12),
                ),
              ]
            : null,
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(AppTokens.radiusPill),
          onTap: busy ? null : onPressed,
          child: AnimatedContainer(
            duration: const Duration(milliseconds: 180),
            constraints: BoxConstraints(minHeight: isTiny ? 36 : 48),
            padding: EdgeInsets.symmetric(
              horizontal: isTiny ? 11 : 18,
              vertical: isTiny ? 8 : 14,
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                if (busy) ...[
                  SizedBox(
                    width: 13,
                    height: 13,
                    child: CircularProgressIndicator(
                      strokeWidth: 2,
                      color: textColor,
                    ),
                  ),
                  const SizedBox(width: 9),
                ],
                Flexible(
                  child: Text(
                    label,
                    overflow: TextOverflow.ellipsis,
                    style: TextStyle(
                      fontFamily: 'Trebuchet MS',
                      color: textColor,
                      fontSize: isTiny ? 12 : 14,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
