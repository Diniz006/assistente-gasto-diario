import 'dart:ui';

import 'package:flutter/material.dart';

import '../theme/app_tokens.dart';

class AppPanel extends StatelessWidget {
  const AppPanel({
    required this.child,
    this.padding = const EdgeInsets.all(26),
    this.radius = AppTokens.radiusPanel,
    this.margin,
    super.key,
  });

  final Widget child;
  final EdgeInsetsGeometry padding;
  final double radius;
  final EdgeInsetsGeometry? margin;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: margin,
      decoration: BoxDecoration(
        border: Border.all(color: AppTokens.line),
        borderRadius: BorderRadius.circular(radius),
        color: AppTokens.panel,
        boxShadow: AppTokens.shadow,
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(radius),
        child: BackdropFilter(
          filter: ImageFilter.blur(sigmaX: 18, sigmaY: 18),
          child: Padding(padding: padding, child: child),
        ),
      ),
    );
  }
}
