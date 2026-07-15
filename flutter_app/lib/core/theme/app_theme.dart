import 'package:flutter/material.dart';

import 'app_tokens.dart';

abstract final class AppTheme {
  static ThemeData light() {
    final base = ThemeData(
      useMaterial3: true,
      colorScheme: ColorScheme.fromSeed(
        seedColor: AppTokens.brand,
        brightness: Brightness.light,
      ),
    );

    return base.copyWith(
      scaffoldBackgroundColor: Colors.transparent,
      textTheme: base.textTheme.copyWith(
        displayLarge: const TextStyle(
          fontFamily: 'Georgia',
          fontSize: 72,
          height: 0.9,
          letterSpacing: -5.0,
          color: AppTokens.ink,
          fontWeight: FontWeight.w700,
        ),
        headlineLarge: const TextStyle(
          fontFamily: 'Georgia',
          fontSize: 42,
          height: 0.95,
          letterSpacing: -2.1,
          color: AppTokens.ink,
          fontWeight: FontWeight.w700,
        ),
        headlineMedium: const TextStyle(
          fontFamily: 'Georgia',
          fontSize: 28,
          height: 1.0,
          letterSpacing: -1.1,
          color: AppTokens.ink,
          fontWeight: FontWeight.w700,
        ),
        titleLarge: const TextStyle(
          fontFamily: 'Georgia',
          fontSize: 21,
          height: 1.1,
          color: AppTokens.ink,
          fontWeight: FontWeight.w700,
        ),
        bodyLarge: const TextStyle(
          fontFamily: 'Georgia',
          fontSize: 17,
          height: 1.35,
          color: Color(0xff405247),
        ),
        bodyMedium: const TextStyle(
          fontFamily: 'Trebuchet MS',
          fontSize: 14,
          height: 1.25,
          color: AppTokens.ink,
        ),
        labelLarge: const TextStyle(
          fontFamily: 'Trebuchet MS',
          fontSize: 14,
          fontWeight: FontWeight.w800,
          color: AppTokens.brandStrong,
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: AppTokens.field,
        contentPadding:
            const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppTokens.radiusInput),
          borderSide: const BorderSide(color: Color(0x2918221d)),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppTokens.radiusInput),
          borderSide: const BorderSide(color: Color(0x2918221d)),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppTokens.radiusInput),
          borderSide: const BorderSide(color: AppTokens.brand),
        ),
      ),
    );
  }
}
