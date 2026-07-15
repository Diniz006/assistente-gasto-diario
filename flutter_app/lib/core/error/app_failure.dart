enum FailureKind {
  connection,
  unavailable,
  expiredSession,
  invalidPayload,
  unknown,
}

class AppFailure implements Exception {
  const AppFailure(this.kind, this.message);

  final FailureKind kind;
  final String message;

  factory AppFailure.connection() => const AppFailure(
        FailureKind.connection,
        'Não foi possível carregar seus dados.',
      );

  factory AppFailure.unavailable() => const AppFailure(
        FailureKind.unavailable,
        'O servidor está indisponível no momento.',
      );

  factory AppFailure.expiredSession() => const AppFailure(
        FailureKind.expiredSession,
        'Sua sessão expirou. Entre novamente para continuar.',
      );

  factory AppFailure.invalidPayload() => const AppFailure(
        FailureKind.invalidPayload,
        'Não foi possível carregar seus dados.',
      );
}
