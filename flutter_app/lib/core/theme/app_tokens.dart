import 'package:flutter/material.dart';

abstract final class AppTokens {
  static const bg = Color(0xfff5efe3);
  static const ink = Color(0xff18221d);
  static const muted = Color(0xff66736b);
  static const panel = Color(0xe0fffcf4);
  static const line = Color(0x1f18221d);
  static const brand = Color(0xff1f6f4a);
  static const brandStrong = Color(0xff154d36);
  static const sun = Color(0xfff0b84b);
  static const rose = Color(0xffd96b5f);
  static const cream = Color(0xfffffaf0);
  static const field = Color(0xb8ffffff);

  static const radiusPanel = 32.0;
  static const radiusCard = 22.0;
  static const radiusInput = 18.0;
  static const radiusPill = 999.0;

  static const gapXs = 6.0;
  static const gapSm = 10.0;
  static const gap = 14.0;
  static const gapLg = 18.0;
  static const gapXl = 24.0;
  static const gap2Xl = 28.0;

  static const shadow = [
    BoxShadow(
      color: Color(0x2e2d2618),
      blurRadius: 80,
      offset: Offset(0, 24),
    ),
  ];

  static const softShadow = [
    BoxShadow(
      color: Color(0x1a18221d),
      blurRadius: 32,
      offset: Offset(0, 12),
    ),
  ];
}
