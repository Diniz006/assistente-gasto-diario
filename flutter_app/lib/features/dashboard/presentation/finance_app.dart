import 'package:flutter/material.dart';

import '../../../core/theme/app_tokens.dart';
import '../../../core/widgets/app_background.dart';
import '../../../core/widgets/section_title.dart';
import '../../../core/widgets/state_view.dart';
import '../../authentication/presentation/auth_panel.dart';
import 'dashboard_controller.dart';
import 'dashboard_home.dart';

enum AppTab {
  today,
  quick,
  history,
  goals,
  more,
  incomes,
  bills,
  categories,
  settings,
  export
}

class FinanceApp extends StatefulWidget {
  const FinanceApp({required this.controller, super.key});

  final DashboardController controller;

  @override
  State<FinanceApp> createState() => _FinanceAppState();
}

class _FinanceAppState extends State<FinanceApp> {
  bool loggedIn = false;
  AppTab tab = AppTab.today;

  @override
  void initState() {
    super.initState();
    widget.controller.loadMock();
  }

  @override
  Widget build(BuildContext context) {
    return AppBackground(
      child: SafeArea(
        child: AnimatedBuilder(
          animation: widget.controller,
          builder: (context, _) {
            final snapshot = widget.controller.snapshot;
            return Scaffold(
              backgroundColor: Colors.transparent,
              body: Stack(
                children: [
                  Center(
                    child: ConstrainedBox(
                      constraints: const BoxConstraints(maxWidth: 1120),
                      child: SingleChildScrollView(
                        padding: EdgeInsets.fromLTRB(
                          _horizontalPadding(context),
                          28,
                          _horizontalPadding(context),
                          MediaQuery.sizeOf(context).width <= 860 ? 112 : 44,
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const _HeroHeader(),
                            const SizedBox(height: AppTokens.gapXl),
                            if (!loggedIn)
                              AuthPanel(
                                  onLoggedIn: () =>
                                      setState(() => loggedIn = true))
                            else if (snapshot != null)
                              DashboardHome(
                                controller: widget.controller,
                                snapshot: snapshot,
                                activeTab: tab,
                                onTabChanged: (value) =>
                                    setState(() => tab = value),
                                onLogout: () =>
                                    setState(() => loggedIn = false),
                              )
                            else
                              StateView(
                                status: widget.controller.status,
                                onRetry: widget.controller.loadMock,
                              ),
                          ],
                        ),
                      ),
                    ),
                  ),
                  if (loggedIn)
                    _BottomNav(
                      active: tab,
                      onChanged: (value) => setState(() => tab = value),
                    ),
                  if (snapshot?.offline ?? false) const _OfflineBanner(),
                  if (widget.controller.toast != null)
                    _Toast(
                      message: widget.controller.toast!,
                      onDone: widget.controller.clearToast,
                    ),
                ],
              ),
            );
          },
        ),
      ),
    );
  }

  double _horizontalPadding(BuildContext context) =>
      MediaQuery.sizeOf(context).width <= 860 ? 11 : 16;
}

class _HeroHeader extends StatelessWidget {
  const _HeroHeader();

  @override
  Widget build(BuildContext context) {
    final narrow = MediaQuery.sizeOf(context).width <= 560;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const MutedLabel('MVP financeiro pessoal'),
        const SizedBox(height: AppTokens.gapSm),
        Text(
          'Quanto posso gastar hoje?',
          style: Theme.of(context)
              .textTheme
              .displayLarge
              ?.copyWith(fontSize: narrow ? 46 : 92),
        ),
        const SizedBox(height: AppTokens.gapSm),
        ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 620),
          child: Text(
            'Entre, veja seu limite seguro do dia e lance uma despesa em poucos segundos.',
            style: Theme.of(context).textTheme.bodyLarge,
          ),
        ),
      ],
    );
  }
}

class _BottomNav extends StatelessWidget {
  const _BottomNav({required this.active, required this.onChanged});

  final AppTab active;
  final ValueChanged<AppTab> onChanged;

  @override
  Widget build(BuildContext context) {
    final items = [
      (AppTab.today, 'Hoje'),
      (AppTab.quick, 'Lançar'),
      (AppTab.history, 'Histórico'),
      (AppTab.goals, 'Metas'),
      (AppTab.more, 'Mais'),
    ];
    return Positioned(
      left: 12,
      right: 12,
      bottom: 10,
      child: Center(
        child: Container(
          constraints: const BoxConstraints(maxWidth: 680),
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: AppTokens.panel.withValues(alpha: 0.92),
            border: Border.all(color: const Color(0x1f18221d)),
            borderRadius: BorderRadius.circular(28),
            boxShadow: const [
              BoxShadow(
                  color: Color(0x2e18221d),
                  blurRadius: 40,
                  offset: Offset(0, 16)),
            ],
          ),
          child: Row(
            children: [
              for (final item in items)
                Expanded(
                  child: Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 3),
                    child: _NavItem(
                      label: item.$2,
                      active: active == item.$1 ||
                          (item.$1 == AppTab.more && _isMore(active)),
                      onTap: () => onChanged(item.$1),
                    ),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }

  bool _isMore(AppTab tab) => {
        AppTab.more,
        AppTab.incomes,
        AppTab.bills,
        AppTab.categories,
        AppTab.settings,
        AppTab.export,
      }.contains(tab);
}

class _NavItem extends StatelessWidget {
  const _NavItem(
      {required this.label, required this.active, required this.onTap});

  final String label;
  final bool active;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: active ? AppTokens.brand : Colors.transparent,
      borderRadius: BorderRadius.circular(20),
      child: InkWell(
        borderRadius: BorderRadius.circular(20),
        onTap: onTap,
        child: Container(
          alignment: Alignment.center,
          constraints: const BoxConstraints(minHeight: 48),
          padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 8),
          child: Text(
            label,
            overflow: TextOverflow.ellipsis,
            style: TextStyle(
              color: active ? AppTokens.cream : AppTokens.muted,
              fontFamily: 'Trebuchet MS',
              fontSize: 12,
              fontWeight: FontWeight.w800,
            ),
          ),
        ),
      ),
    );
  }
}

class _OfflineBanner extends StatelessWidget {
  const _OfflineBanner();

  @override
  Widget build(BuildContext context) {
    return Positioned(
      left: 12,
      right: 12,
      bottom: 88,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: BoxDecoration(
          color: const Color(0xf5fff6de),
          border: Border.all(color: const Color(0x389a681a)),
          borderRadius: BorderRadius.circular(22),
          boxShadow: const [
            BoxShadow(
                color: Color(0x292d2618), blurRadius: 42, offset: Offset(0, 16))
          ],
        ),
        child: const Text(
          'Você está offline. O app mostra os dados já carregados e volta a sincronizar quando a conexão retornar.',
          style: TextStyle(
            color: Color(0xff6f4815),
            fontFamily: 'Trebuchet MS',
            fontWeight: FontWeight.w800,
          ),
        ),
      ),
    );
  }
}

class _Toast extends StatefulWidget {
  const _Toast({required this.message, required this.onDone});

  final String message;
  final VoidCallback onDone;

  @override
  State<_Toast> createState() => _ToastState();
}

class _ToastState extends State<_Toast> {
  @override
  void initState() {
    super.initState();
    Future<void>.delayed(const Duration(milliseconds: 3600), widget.onDone);
  }

  @override
  Widget build(BuildContext context) {
    return Positioned(
      right: 22,
      bottom: 22,
      child: Container(
        constraints:
            BoxConstraints(maxWidth: MediaQuery.sizeOf(context).width - 44),
        padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 16),
        decoration: BoxDecoration(
          color: AppTokens.ink,
          borderRadius: BorderRadius.circular(22),
          boxShadow: AppTokens.shadow,
        ),
        child: Text(
          widget.message,
          style: const TextStyle(
            color: AppTokens.cream,
            fontFamily: 'Trebuchet MS',
            fontWeight: FontWeight.w700,
          ),
        ),
      ),
    );
  }
}
