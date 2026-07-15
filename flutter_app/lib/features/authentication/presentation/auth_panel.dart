import 'package:flutter/material.dart';

import '../../../core/theme/app_tokens.dart';
import '../../../core/widgets/app_button.dart';
import '../../../core/widgets/app_fields.dart';
import '../../../core/widgets/app_panel.dart';

class AuthPanel extends StatefulWidget {
  const AuthPanel({required this.onLoggedIn, super.key});

  final VoidCallback onLoggedIn;

  @override
  State<AuthPanel> createState() => _AuthPanelState();
}

class _AuthPanelState extends State<AuthPanel> {
  bool signup = false;
  bool busy = false;

  @override
  Widget build(BuildContext context) {
    return ConstrainedBox(
      constraints: const BoxConstraints(maxWidth: 480),
      child: AppPanel(
        padding: const EdgeInsets.all(18),
        child: Column(
          children: [
            Container(
              padding: const EdgeInsets.all(6),
              decoration: BoxDecoration(
                color: const Color(0x0f18221d),
                borderRadius: BorderRadius.circular(AppTokens.radiusPill),
              ),
              child: Row(
                children: [
                  Expanded(child: _TabButton(label: 'Entrar', active: !signup, onTap: () => setState(() => signup = false))),
                  Expanded(child: _TabButton(label: 'Criar conta', active: signup, onTap: () => setState(() => signup = true))),
                ],
              ),
            ),
            const SizedBox(height: AppTokens.gapLg),
            AnimatedSwitcher(
              duration: const Duration(milliseconds: 180),
              child: signup ? _SignupForm(onSubmit: _submit, busy: busy) : _LoginForm(onSubmit: _submit, busy: busy),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _submit() async {
    setState(() => busy = true);
    await Future<void>.delayed(const Duration(milliseconds: 280));
    setState(() => busy = false);
    widget.onLoggedIn();
  }
}

class _TabButton extends StatelessWidget {
  const _TabButton({required this.label, required this.active, required this.onTap});

  final String label;
  final bool active;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: active ? AppTokens.ink : Colors.transparent,
      borderRadius: BorderRadius.circular(AppTokens.radiusPill),
      child: InkWell(
        borderRadius: BorderRadius.circular(AppTokens.radiusPill),
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
          child: Text(
            label,
            textAlign: TextAlign.center,
            style: TextStyle(
              color: active ? AppTokens.cream : AppTokens.muted,
              fontFamily: 'Trebuchet MS',
              fontWeight: FontWeight.w700,
            ),
          ),
        ),
      ),
    );
  }
}

class _LoginForm extends StatelessWidget {
  const _LoginForm({required this.onSubmit, required this.busy});

  final VoidCallback onSubmit;
  final bool busy;

  @override
  Widget build(BuildContext context) {
    return Column(
      key: const ValueKey('login'),
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const AppTextField(label: 'Email', keyboardType: TextInputType.emailAddress),
        const SizedBox(height: AppTokens.gap),
        const AppTextField(label: 'Senha', obscureText: true),
        const SizedBox(height: AppTokens.gap),
        AppButton(label: 'Entrar no painel', busy: busy, onPressed: onSubmit),
      ],
    );
  }
}

class _SignupForm extends StatelessWidget {
  const _SignupForm({required this.onSubmit, required this.busy});

  final VoidCallback onSubmit;
  final bool busy;

  @override
  Widget build(BuildContext context) {
    return Column(
      key: const ValueKey('signup'),
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const AppTextField(label: 'Nome', maxLength: 120),
        const SizedBox(height: AppTokens.gap),
        const AppTextField(label: 'Email', keyboardType: TextInputType.emailAddress),
        const SizedBox(height: AppTokens.gap),
        const AppTextField(label: 'Senha', obscureText: true),
        const SizedBox(height: AppTokens.gap),
        AppButton(label: 'Criar e entrar', busy: busy, onPressed: onSubmit),
      ],
    );
  }
}
