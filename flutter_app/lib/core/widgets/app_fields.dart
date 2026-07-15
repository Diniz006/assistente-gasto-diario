import 'package:flutter/material.dart';

import '../theme/app_tokens.dart';

class AppTextField extends StatelessWidget {
  const AppTextField({
    required this.label,
    this.hint,
    this.initialValue,
    this.keyboardType,
    this.obscureText = false,
    this.maxLength,
    this.onChanged,
    super.key,
  });

  final String label;
  final String? hint;
  final String? initialValue;
  final TextInputType? keyboardType;
  final bool obscureText;
  final int? maxLength;
  final ValueChanged<String>? onChanged;

  @override
  Widget build(BuildContext context) {
    return _FieldShell(
      label: label,
      child: TextFormField(
        initialValue: initialValue,
        keyboardType: keyboardType,
        obscureText: obscureText,
        maxLength: maxLength,
        onChanged: onChanged,
        decoration: InputDecoration(
          hintText: hint,
          counterText: '',
        ),
      ),
    );
  }
}

class AppSelectField<T> extends StatelessWidget {
  const AppSelectField({
    required this.label,
    required this.value,
    required this.items,
    required this.onChanged,
    super.key,
  });

  final String label;
  final T value;
  final List<DropdownMenuItem<T>> items;
  final ValueChanged<T?> onChanged;

  @override
  Widget build(BuildContext context) {
    return _FieldShell(
      label: label,
      child: DropdownButtonFormField<T>(
        value: value,
        items: items,
        onChanged: onChanged,
        decoration: const InputDecoration(),
      ),
    );
  }
}

class _FieldShell extends StatelessWidget {
  const _FieldShell({required this.label, required this.child});

  final String label;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          label,
          style: const TextStyle(
            color: Color(0xff38463e),
            fontFamily: 'Trebuchet MS',
            fontSize: 14,
            fontWeight: FontWeight.w700,
          ),
        ),
        const SizedBox(height: 7),
        child,
      ],
    );
  }
}

class EmptyState extends StatelessWidget {
  const EmptyState(this.message, {super.key});

  final String message;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.38),
        borderRadius: BorderRadius.circular(AppTokens.radiusCard),
        border: Border.all(color: const Color(0x2e18221d), style: BorderStyle.solid),
      ),
      child: Text(
        message,
        style: const TextStyle(
          color: AppTokens.muted,
          fontFamily: 'Trebuchet MS',
          fontSize: 14,
        ),
      ),
    );
  }
}
