using AssistenteGastoDiario.Application.DTOs.Dashboard;
using AssistenteGastoDiario.Application.DTOs.Categories;
using AssistenteGastoDiario.Application.DTOs.Expenses;
using AssistenteGastoDiario.Application.DTOs.FinancialCycles;
using AssistenteGastoDiario.Application.DTOs.SafeDailyLimits;
using AssistenteGastoDiario.Application.Services;
using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Domain.Enums;
using AssistenteGastoDiario.Infrastructure.Data;
using AssistenteGastoDiario.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var tests = new (string Name, Action Run)[]
{
    ("FinancialCycle: reference date inside current cycle", FinancialCycleInsideCurrentCycle),
    ("FinancialCycle: reference date before start day uses previous month", FinancialCycleBeforeStartDay),
    ("FinancialCycle: start day 31 adapts to shorter months", FinancialCycleStartDayAdaptsToShorterMonths),
    ("FinancialCycle: invalid start day throws", FinancialCycleInvalidStartDayThrows),
    ("SafeDailyLimit: calculates available balance and daily limit", SafeDailyLimitCalculatesAvailableBalance),
    ("SafeDailyLimit: returns zero when balance is unavailable", SafeDailyLimitReturnsZeroWhenUnavailable),
    ("SafeDailyLimit: rejects negative remaining days", SafeDailyLimitRejectsNegativeDays),
    ("Dashboard: combines cycle, safe limit and alerts", DashboardBuildsSummaryAndAlerts),
    ("CategoryService: creates, trims and lists categories", CategoryServiceCreatesAndListsCategories),
    ("CategoryService: rejects duplicated category names", CategoryServiceRejectsDuplicatedNames),
    ("ExpenseService: creates and lists expenses by period", ExpenseServiceCreatesAndListsByPeriod),
    ("ExpenseService: rejects inactive expense category", ExpenseServiceRejectsInactiveCategory)
};

var failures = new List<string>();

foreach (var test in tests)
{
    try
    {
        test.Run();
        Console.WriteLine($"PASS {test.Name}");
    }
    catch (Exception ex)
    {
        failures.Add($"{test.Name}: {ex.Message}");
        Console.WriteLine($"FAIL {test.Name}");
        Console.WriteLine($"     {ex.Message}");
    }
}

Console.WriteLine();
Console.WriteLine($"{tests.Length - failures.Count}/{tests.Length} tests passed.");

if (failures.Count > 0)
{
    Console.WriteLine("Failures:");
    foreach (var failure in failures)
    {
        Console.WriteLine($"- {failure}");
    }

    return 1;
}

return 0;

static void FinancialCycleInsideCurrentCycle()
{
    var service = new FinancialCycleService();

    var result = service.Calculate(new FinancialCycleCalculationRequest(
        new DateOnly(2026, 7, 14),
        5));

    AssertEqual(new DateOnly(2026, 7, 5), result.CycleStartDate);
    AssertEqual(new DateOnly(2026, 8, 4), result.CycleEndDate);
    AssertEqual(31, result.DaysInCycle);
    AssertEqual(22, result.DaysRemaining);
    AssertEqual(new DateOnly(2026, 8, 5), result.NextCycleStartDate);
}

static void FinancialCycleBeforeStartDay()
{
    var service = new FinancialCycleService();

    var result = service.Calculate(new FinancialCycleCalculationRequest(
        new DateOnly(2026, 7, 3),
        5));

    AssertEqual(new DateOnly(2026, 6, 5), result.CycleStartDate);
    AssertEqual(new DateOnly(2026, 7, 4), result.CycleEndDate);
    AssertEqual(30, result.DaysInCycle);
    AssertEqual(2, result.DaysRemaining);
    AssertEqual(new DateOnly(2026, 7, 5), result.NextCycleStartDate);
}

static void FinancialCycleStartDayAdaptsToShorterMonths()
{
    var service = new FinancialCycleService();

    var result = service.Calculate(new FinancialCycleCalculationRequest(
        new DateOnly(2026, 2, 15),
        31));

    AssertEqual(new DateOnly(2026, 1, 31), result.CycleStartDate);
    AssertEqual(new DateOnly(2026, 2, 27), result.CycleEndDate);
    AssertEqual(28, result.DaysInCycle);
    AssertEqual(13, result.DaysRemaining);
    AssertEqual(new DateOnly(2026, 2, 28), result.NextCycleStartDate);
}

static void FinancialCycleInvalidStartDayThrows()
{
    var service = new FinancialCycleService();

    AssertThrows<ArgumentOutOfRangeException>(() =>
        service.Calculate(new FinancialCycleCalculationRequest(
            new DateOnly(2026, 7, 14),
            0)));
}

static void SafeDailyLimitCalculatesAvailableBalance()
{
    var service = new SafeDailyLimitService();

    var result = service.Calculate(new SafeDailyLimitCalculationRequest(
        IncomeTotal: 3000m,
        FixedBillsTotal: 1000m,
        GoalPlannedTotal: 500m,
        ExpensesTotal: 400m,
        DaysRemaining: 11));

    AssertEqual(1100m, result.AvailableBalance);
    AssertEqual(100m, result.SafeDailyLimit);
    AssertEqual(11, result.DaysRemaining);
}

static void SafeDailyLimitReturnsZeroWhenUnavailable()
{
    var service = new SafeDailyLimitService();

    var result = service.Calculate(new SafeDailyLimitCalculationRequest(
        IncomeTotal: 1000m,
        FixedBillsTotal: 800m,
        GoalPlannedTotal: 300m,
        ExpensesTotal: 100m,
        DaysRemaining: 10));

    AssertEqual(-200m, result.AvailableBalance);
    AssertEqual(0m, result.SafeDailyLimit);
}

static void SafeDailyLimitRejectsNegativeDays()
{
    var service = new SafeDailyLimitService();

    AssertThrows<ArgumentOutOfRangeException>(() =>
        service.Calculate(new SafeDailyLimitCalculationRequest(
            IncomeTotal: 1000m,
            FixedBillsTotal: 0m,
            GoalPlannedTotal: 0m,
            ExpensesTotal: 0m,
            DaysRemaining: -1)));
}

static void DashboardBuildsSummaryAndAlerts()
{
    var service = new DashboardService(
        new FinancialCycleService(),
        new SafeDailyLimitService());

    var result = service.Build(new DashboardSummaryRequest(
        ReferenceDate: new DateOnly(2026, 7, 29),
        CycleStartDay: 1,
        MonthClosureDay: null,
        IncomeTotal: 1000m,
        FixedBillsTotal: 800m,
        GoalPlannedTotal: 300m,
        ExpensesTotal: 100m,
        GoalContributedTotal: 50m,
        OpenAlertsCount: 2));

    AssertEqual(new DateOnly(2026, 7, 1), result.FinancialCycle.CycleStartDate);
    AssertEqual(new DateOnly(2026, 7, 31), result.FinancialCycle.CycleEndDate);
    AssertEqual(3, result.FinancialCycle.DaysRemaining);
    AssertEqual(-200m, result.SafeDailyLimit.AvailableBalance);
    AssertEqual(0m, result.SafeDailyLimit.SafeDailyLimit);
    AssertEqual(50m, result.GoalContributedTotal);
    AssertEqual(2, result.OpenAlertsCount);
    AssertEqual(3, result.Alerts.Count);
    AssertTrue(result.Alerts.Any(alert => alert.Type == AlertType.LowBalance), "Expected a low balance alert.");
}

static void CategoryServiceCreatesAndListsCategories()
{
    using var dbContext = CreateDbContext();
    var userId = SeedUser(dbContext);
    var service = new CategoryService(dbContext);

    var created = service.CreateAsync(
        userId,
        new CreateCategoryRequest("  Mercado  ", CategoryType.Expense, "#1f6f4a", "cart"))
        .GetAwaiter()
        .GetResult();

    var categories = service.ListAsync(userId, CategoryType.Expense)
        .GetAwaiter()
        .GetResult();

    AssertEqual("Mercado", created.Name);
    AssertEqual(CategoryType.Expense, created.Type);
    AssertEqual("#1f6f4a", created.Color);
    AssertEqual(1, categories.Count);
    AssertEqual(created.Id, categories.Single().Id);
}

static void CategoryServiceRejectsDuplicatedNames()
{
    using var dbContext = CreateDbContext();
    var userId = SeedUser(dbContext);
    var service = new CategoryService(dbContext);

    service.CreateAsync(userId, new CreateCategoryRequest("Mercado", CategoryType.Expense))
        .GetAwaiter()
        .GetResult();

    AssertThrows<InvalidOperationException>(() =>
        service.CreateAsync(userId, new CreateCategoryRequest("Mercado", CategoryType.Expense))
            .GetAwaiter()
            .GetResult());
}

static void ExpenseServiceCreatesAndListsByPeriod()
{
    using var dbContext = CreateDbContext();
    var userId = SeedUser(dbContext);
    var categoryId = SeedCategory(dbContext, userId, CategoryType.Expense, "Mercado");
    var service = new ExpenseService(dbContext);

    var created = service.CreateAsync(
        userId,
        new CreateExpenseRequest(
            categoryId,
            "  Compra do mes  ",
            250.75m,
            new DateOnly(2026, 7, 14),
            PaymentMethod.Pix,
            "arroz e feijao"))
        .GetAwaiter()
        .GetResult();

    service.CreateAsync(
        userId,
        new CreateExpenseRequest(
            categoryId,
            "Fora do periodo",
            80m,
            new DateOnly(2026, 8, 2),
            PaymentMethod.DebitCard))
        .GetAwaiter()
        .GetResult();

    var expenses = service.ListByPeriodAsync(
            userId,
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 31))
        .GetAwaiter()
        .GetResult();

    AssertEqual("Compra do mes", created.Description);
    AssertEqual(250.75m, created.Amount);
    AssertEqual(PaymentMethod.Pix, created.PaymentMethod);
    AssertEqual("arroz e feijao", created.Notes);
    AssertEqual(1, expenses.Count);
    AssertEqual(created.Id, expenses.Single().Id);
}

static void ExpenseServiceRejectsInactiveCategory()
{
    using var dbContext = CreateDbContext();
    var userId = SeedUser(dbContext);
    var categoryId = SeedCategory(dbContext, userId, CategoryType.Expense, "Antiga", isActive: false);
    var service = new ExpenseService(dbContext);

    AssertThrows<InvalidOperationException>(() =>
        service.CreateAsync(
                userId,
                new CreateExpenseRequest(
                    categoryId,
                    "Compra",
                    10m,
                    new DateOnly(2026, 7, 14),
                    PaymentMethod.Cash))
            .GetAwaiter()
            .GetResult());
}

static AppDbContext CreateDbContext()
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase($"agd-tests-{Guid.NewGuid()}")
        .Options;

    return new AppDbContext(options);
}

static Guid SeedUser(AppDbContext dbContext)
{
    var user = new User
    {
        Name = "Teste",
        Email = $"{Guid.NewGuid():N}@example.com",
        PasswordHash = "hash"
    };

    dbContext.Users.Add(user);
    dbContext.SaveChanges();

    return user.Id;
}

static Guid SeedCategory(
    AppDbContext dbContext,
    Guid userId,
    CategoryType type,
    string name,
    bool isActive = true)
{
    var category = new Category
    {
        UserId = userId,
        Name = name,
        Type = type,
        IsActive = isActive,
        IsDefault = false
    };

    dbContext.Categories.Add(category);
    dbContext.SaveChanges();

    return category.Id;
}

static void AssertEqual<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected {expected}, got {actual}.");
    }
}

static void AssertTrue(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

static void AssertThrows<TException>(Action action)
    where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Expected {typeof(TException).Name}, got {ex.GetType().Name}.");
    }

    throw new InvalidOperationException($"Expected {typeof(TException).Name}, but no exception was thrown.");
}
