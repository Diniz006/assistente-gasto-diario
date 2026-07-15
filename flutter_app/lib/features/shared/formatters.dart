import 'package:intl/intl.dart';

abstract final class Formatters {
  static final money = NumberFormat.currency(locale: 'pt_BR', symbol: 'R\$');

  static String currency(num value) => money.format(value);
}
