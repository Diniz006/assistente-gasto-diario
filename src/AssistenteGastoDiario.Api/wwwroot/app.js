const state = {
    token: localStorage.getItem("agd.token"),
    user: JSON.parse(localStorage.getItem("agd.user") || "null"),
    dashboard: null,
    categoriesById: new Map(),
    incomesById: new Map(),
    expensesById: new Map(),
    fixedBillsById: new Map(),
    goalsById: new Map(),
    incomeCategories: [],
    expenseCategories: [],
    billCategories: [],
    goalCategories: []
};

const currency = new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL"
});

const $ = (selector) => document.querySelector(selector);
const authPanel = $("#auth-panel");
const appPanel = $("#app-panel");
const toast = $("#toast");

document.addEventListener("DOMContentLoaded", () => {
    bindAuthTabs();
    bindForms();
    renderSession();
});

function bindAuthTabs() {
    document.querySelectorAll("[data-auth-tab]").forEach((button) => {
        button.addEventListener("click", () => {
            const tab = button.dataset.authTab;
            document.querySelectorAll("[data-auth-tab]").forEach((item) => {
                item.classList.toggle("tab--active", item === button);
            });
            $("#login-form").classList.toggle("hidden", tab !== "login");
            $("#signup-form").classList.toggle("hidden", tab !== "signup");
        });
    });
}

function bindForms() {
    $("#login-form").addEventListener("submit", async (event) => {
        event.preventDefault();
        const data = Object.fromEntries(new FormData(event.currentTarget));
        await login(data.email, data.password);
    });

    $("#signup-form").addEventListener("submit", async (event) => {
        event.preventDefault();
        const data = Object.fromEntries(new FormData(event.currentTarget));
        await api("/api/users", {
            method: "POST",
            body: data,
            auth: false
        });
        await login(data.email, data.password);
    });

    $("#settings-form").addEventListener("submit", async (event) => {
        event.preventDefault();
        const data = Object.fromEntries(new FormData(event.currentTarget));
        await api("/api/me/financial-settings", {
            method: "PUT",
            body: {
                monthlyIncomeDefault: Number(data.monthlyIncomeDefault),
                cycleStartDay: Number(data.cycleStartDay),
                monthClosureDay: data.monthClosureDay ? Number(data.monthClosureDay) : null,
                currencyCode: "BRL"
            }
        });
        showToast("Configuracao salva. Agora o painel ja consegue calcular seu dia.");
        await loadDashboard();
    });

    $("#quick-expense-form").addEventListener("submit", async (event) => {
        event.preventDefault();
        const form = event.currentTarget;
        const data = Object.fromEntries(new FormData(form));
        $("#quick-status").textContent = "Lancando...";
        const response = await api("/api/me/quick-expenses", {
            method: "POST",
            body: {
                description: data.description,
                amount: Number(data.amount),
                categoryId: data.categoryId || null,
                paymentMethod: Number(data.paymentMethod)
            }
        });
        form.reset();
        $("#quick-status").textContent = "Despesa registrada";
        showToast(`${response.expense.description} entrou no ciclo. Limite atualizado.`);
        renderDashboard(response.dashboard);
        await loadExpensesForCurrentCycle();
        setTimeout(() => $("#quick-status").textContent = "", 1800);
    });

    $("#income-form").addEventListener("submit", async (event) => {
        event.preventDefault();
        const form = event.currentTarget;
        const data = Object.fromEntries(new FormData(form));
        $("#incomes-status").textContent = "Salvando...";
        await api("/api/me/incomes", {
            method: "POST",
            body: {
                categoryId: data.categoryId || null,
                description: data.description,
                amount: Number(data.amount),
                receivedOn: data.receivedOn || todayIso(),
                isRecurring: Boolean(data.isRecurring),
                notes: null
            }
        });
        form.reset();
        form.elements.receivedOn.value = todayIso();
        showToast(`${data.description} entrou nas rendas do ciclo.`);
        await loadDashboard();
    });

    $("#fixed-bill-form").addEventListener("submit", async (event) => {
        event.preventDefault();
        const form = event.currentTarget;
        const data = Object.fromEntries(new FormData(form));
        $("#bills-status").textContent = "Salvando...";
        await api("/api/me/fixed-bills", {
            method: "POST",
            body: {
                categoryId: data.categoryId || null,
                name: data.name,
                amount: Number(data.amount),
                dueDay: Number(data.dueDay),
                status: 1,
                isRecurringMonthly: true,
                autoIncludeInCycle: true
            }
        });
        form.reset();
        form.elements.dueDay.value = "5";
        showToast(`${data.name} entrou nas contas fixas. Limite recalculado.`);
        await loadDashboard();
        await loadFixedBills();
    });

    $("#financial-goal-form").addEventListener("submit", async (event) => {
        event.preventDefault();
        const form = event.currentTarget;
        const data = Object.fromEntries(new FormData(form));
        $("#goals-status").textContent = "Salvando...";
        await api("/api/me/financial-goals", {
            method: "POST",
            body: {
                categoryId: data.categoryId || null,
                name: data.name,
                targetAmount: Number(data.targetAmount),
                currentAmount: Number(data.currentAmount || 0),
                monthlyPlannedAmount: Number(data.monthlyPlannedAmount || 0),
                priority: 2,
                status: 1
            }
        });
        form.reset();
        form.elements.currentAmount.value = "0";
        form.elements.monthlyPlannedAmount.value = "0";
        showToast(`${data.name} entrou nas suas metas.`);
        await loadDashboard();
        await loadFinancialGoals();
    });

    $("#goal-list").addEventListener("submit", async (event) => {
        if (!event.target.matches("[data-goal-contribution-form]")) {
            return;
        }

        event.preventDefault();
        const form = event.target;
        const data = Object.fromEntries(new FormData(form));
        const goalId = form.dataset.goalId;
        await api(`/api/me/financial-goals/${goalId}/contributions`, {
            method: "POST",
            body: {
                amount: Number(data.amount),
                notes: data.notes || null
            }
        });
        showToast("Contribuicao registrada. Meta e limite recalculados.");
        await loadDashboard();
        await loadFinancialGoals();
    });

    $("#income-list").addEventListener("click", async (event) => {
        const button = event.target.closest("[data-income-action]");
        if (!button) {
            return;
        }

        const income = state.incomesById.get(button.dataset.incomeId);
        if (!income) {
            return;
        }

        if (button.dataset.incomeAction === "edit") {
            await editIncome(income);
        } else if (button.dataset.incomeAction === "delete") {
            await deleteIncome(income);
        }
    });

    $("#expense-list").addEventListener("click", async (event) => {
        const button = event.target.closest("[data-expense-action]");
        if (!button) {
            return;
        }

        const expense = state.expensesById.get(button.dataset.expenseId);
        if (!expense) {
            return;
        }

        if (button.dataset.expenseAction === "edit") {
            await editExpense(expense);
        } else if (button.dataset.expenseAction === "delete") {
            await deleteExpense(expense);
        }
    });

    $("#bill-list").addEventListener("click", async (event) => {
        const button = event.target.closest("[data-bill-action]");
        if (!button) {
            return;
        }

        const bill = state.fixedBillsById.get(button.dataset.billId);
        if (!bill) {
            return;
        }

        if (button.dataset.billAction === "edit") {
            await editFixedBill(bill);
        } else if (button.dataset.billAction === "delete") {
            await deleteFixedBill(bill);
        }
    });

    $("#goal-list").addEventListener("click", async (event) => {
        const button = event.target.closest("[data-goal-action]");
        if (!button) {
            return;
        }

        const goal = state.goalsById.get(button.dataset.goalId);
        if (!goal) {
            return;
        }

        if (button.dataset.goalAction === "edit") {
            await editFinancialGoal(goal);
        } else if (button.dataset.goalAction === "delete") {
            await deleteFinancialGoal(goal);
        }
    });

    $("#logout-button").addEventListener("click", () => {
        clearSession();
    });
}

async function login(email, password) {
    const auth = await api("/api/auth/login", {
        method: "POST",
        body: { email, password },
        auth: false
    });

    state.token = auth.accessToken;
    state.user = auth.user;
    localStorage.setItem("agd.token", state.token);
    localStorage.setItem("agd.user", JSON.stringify(state.user));
    showToast("Entrada liberada. Vamos olhar seu dinheiro com carinho.");
    renderSession();
}

async function renderSession() {
    const loggedIn = Boolean(state.token);
    authPanel.classList.toggle("hidden", loggedIn);
    appPanel.classList.toggle("hidden", !loggedIn);

    if (!loggedIn) {
        return;
    }

    $("#user-greeting").textContent = `Ola, ${state.user?.name || "vamos nessa"}`;
    $("#income-received-on").value = todayIso();
    try {
        await loadCategories();
        await loadDashboard();
        await loadFixedBills();
        await loadFinancialGoals();
    } catch (error) {
        if (error.status === 401) {
            clearSession("Sua sessao expirou. Entre novamente para continuar.");
            return;
        }

        throw error;
    }
}

function clearSession(message) {
    state.token = null;
    state.user = null;
    localStorage.removeItem("agd.token");
    localStorage.removeItem("agd.user");
    authPanel.classList.remove("hidden");
    appPanel.classList.add("hidden");

    if (message) {
        showToast(message);
    }
}

async function loadDashboard() {
    try {
        $("#setup-card").classList.add("hidden");
        const dashboard = await api("/api/me/dashboard");
        renderDashboard(dashboard);
        await loadIncomesForCurrentCycle();
        await loadExpensesForCurrentCycle();
    } catch (error) {
        if (error.status === 404) {
            $("#setup-card").classList.remove("hidden");
            renderDashboard(null);
            renderIncomes([]);
            renderExpenses([]);
            return;
        }
        throw error;
    }
}

async function loadCategories() {
    state.categoriesById.clear();
    state.incomeCategories = await api("/api/me/categories?type=1");
    state.expenseCategories = await api("/api/me/categories?type=2");
    state.billCategories = await api("/api/me/categories?type=3");
    state.goalCategories = await api("/api/me/categories?type=4");

    const incomeSelect = $("#income-category-select");
    incomeSelect.innerHTML = '<option value="">Sem categoria</option>';
    state.incomeCategories
        .filter((category) => category.isActive)
        .forEach((category) => {
            state.categoriesById.set(category.id, category);
            const option = document.createElement("option");
            option.value = category.id;
            option.textContent = category.name;
            incomeSelect.appendChild(option);
        });

    const select = $("#category-select");
    select.innerHTML = '<option value="">Automatica</option>';
    state.expenseCategories
        .filter((category) => category.isActive)
        .forEach((category) => {
            state.categoriesById.set(category.id, category);
            const option = document.createElement("option");
            option.value = category.id;
            option.textContent = category.name;
            select.appendChild(option);
        });

    const billSelect = $("#bill-category-select");
    billSelect.innerHTML = '<option value="">Sem categoria</option>';
    state.billCategories
        .filter((category) => category.isActive)
        .forEach((category) => {
            state.categoriesById.set(category.id, category);
            const option = document.createElement("option");
            option.value = category.id;
            option.textContent = category.name;
            billSelect.appendChild(option);
        });

    const goalSelect = $("#goal-category-select");
    goalSelect.innerHTML = '<option value="">Sem categoria</option>';
    state.goalCategories
        .filter((category) => category.isActive)
        .forEach((category) => {
            state.categoriesById.set(category.id, category);
            const option = document.createElement("option");
            option.value = category.id;
            option.textContent = category.name;
            goalSelect.appendChild(option);
        });
}

function renderDashboard(dashboard) {
    state.dashboard = dashboard;

    if (!dashboard) {
        $("#cycle-label").textContent = "Sem ciclo configurado";
        $("#safe-limit").textContent = currency.format(0);
        $("#safe-message").textContent = "Preencha a configuracao inicial para calcular seu limite diario.";
        $("#income-total").textContent = currency.format(0);
        $("#fixed-total").textContent = currency.format(0);
        $("#expenses-total").textContent = currency.format(0);
        $("#goals-total").textContent = currency.format(0);
        renderIncomes([]);
        return;
    }

    const safe = dashboard.safeDailyLimit;
    const cycle = dashboard.financialCycle;
    $("#cycle-label").textContent = `${formatDate(cycle.cycleStartDate)} ate ${formatDate(cycle.cycleEndDate)}`;
    $("#safe-limit").textContent = currency.format(safe.safeDailyLimit);
    $("#safe-message").textContent = safe.message;
    $("#income-total").textContent = currency.format(safe.incomeTotal);
    $("#fixed-total").textContent = currency.format(safe.fixedBillsTotal);
    $("#expenses-total").textContent = currency.format(safe.expensesTotal);
    $("#goals-total").textContent = currency.format(safe.goalPlannedTotal);
}

async function loadIncomesForCurrentCycle() {
    if (!state.dashboard?.financialCycle) {
        renderIncomes([]);
        return;
    }

    $("#incomes-status").textContent = "Atualizando...";
    const cycle = state.dashboard.financialCycle;
    const params = new URLSearchParams({
        startDate: cycle.cycleStartDate,
        endDate: cycle.cycleEndDate
    });
    const incomes = await api(`/api/me/incomes?${params.toString()}`);
    renderIncomes(incomes);
    $("#incomes-status").textContent = `${incomes.length} entrada${incomes.length === 1 ? "" : "s"}`;
}

async function loadExpensesForCurrentCycle() {
    if (!state.dashboard?.financialCycle) {
        renderExpenses([]);
        return;
    }

    $("#expenses-status").textContent = "Atualizando...";
    const cycle = state.dashboard.financialCycle;
    const params = new URLSearchParams({
        startDate: cycle.cycleStartDate,
        endDate: cycle.cycleEndDate
    });
    const expenses = await api(`/api/me/expenses?${params.toString()}`);
    renderExpenses(expenses);
    $("#expenses-status").textContent = `${expenses.length} lancamento${expenses.length === 1 ? "" : "s"}`;
}

function renderIncomes(incomes) {
    const list = $("#income-list");
    list.innerHTML = "";
    state.incomesById.clear();

    if (!incomes.length) {
        list.innerHTML = '<div class="empty-state">Quando voce cadastrar uma renda do ciclo, ela aparece aqui.</div>';
        $("#incomes-status").textContent = "";
        return;
    }

    const sortedIncomes = [...incomes].sort((left, right) => {
        const receivedOnComparison = right.receivedOn.localeCompare(left.receivedOn);
        return receivedOnComparison !== 0
            ? receivedOnComparison
            : left.description.localeCompare(right.description);
    });

    sortedIncomes.forEach((income) => {
        state.incomesById.set(income.id, income);
        const category = income.categoryId ? state.categoriesById.get(income.categoryId) : null;
        const item = document.createElement("article");
        item.className = "income-item";
        item.innerHTML = `
            <div>
                <strong>${escapeHtml(income.description)}</strong>
                <span>${formatDate(income.receivedOn)}${category ? ` &middot; ${escapeHtml(category.name)}` : ""}${income.isRecurring ? " &middot; recorrente" : ""}</span>
            </div>
            <div class="item-side">
                <b>${currency.format(income.amount)}</b>
                <div class="item-actions">
                    <button class="button button--tiny" type="button" data-income-action="edit" data-income-id="${income.id}">Editar</button>
                    <button class="button button--tiny button--danger" type="button" data-income-action="delete" data-income-id="${income.id}">Excluir</button>
                </div>
            </div>
        `;
        list.appendChild(item);
    });
}

function renderExpenses(expenses) {
    const list = $("#expense-list");
    list.innerHTML = "";
    state.expensesById.clear();

    if (!expenses.length) {
        list.innerHTML = '<div class="empty-state">Quando voce lancar despesas, elas aparecem aqui.</div>';
        $("#expenses-status").textContent = "";
        return;
    }

    const recentExpenses = [...expenses].sort((left, right) => {
        const spentOnComparison = right.spentOn.localeCompare(left.spentOn);
        return spentOnComparison !== 0
            ? spentOnComparison
            : right.createdAt.localeCompare(left.createdAt);
    });

    recentExpenses.slice(0, 8).forEach((expense) => {
        state.expensesById.set(expense.id, expense);
        const category = state.categoriesById.get(expense.categoryId);
        const item = document.createElement("article");
        item.className = "expense-item";
        item.innerHTML = `
            <div>
                <strong>${escapeHtml(expense.description)}</strong>
                <span>${formatDate(expense.spentOn)}${category ? ` &middot; ${escapeHtml(category.name)}` : ""}</span>
            </div>
            <div class="item-side">
                <b>${currency.format(expense.amount)}</b>
                <div class="item-actions">
                    <button class="button button--tiny" type="button" data-expense-action="edit" data-expense-id="${expense.id}">Editar</button>
                    <button class="button button--tiny button--danger" type="button" data-expense-action="delete" data-expense-id="${expense.id}">Excluir</button>
                </div>
            </div>
        `;
        list.appendChild(item);
    });
}

async function loadFixedBills() {
    $("#bills-status").textContent = "Atualizando...";
    const bills = await api("/api/me/fixed-bills");
    renderFixedBills(bills);
    $("#bills-status").textContent = `${bills.length} conta${bills.length === 1 ? "" : "s"}`;
}

function renderFixedBills(bills) {
    const list = $("#bill-list");
    list.innerHTML = "";
    state.fixedBillsById.clear();

    if (!bills.length) {
        list.innerHTML = '<div class="empty-state">Cadastre suas contas fixas para o limite diario ficar mais realista.</div>';
        $("#bills-status").textContent = "";
        return;
    }

    const sortedBills = [...bills].sort((left, right) => left.dueDay - right.dueDay || left.name.localeCompare(right.name));
    sortedBills.forEach((bill) => {
        state.fixedBillsById.set(bill.id, bill);
        const category = bill.categoryId ? state.categoriesById.get(bill.categoryId) : null;
        const item = document.createElement("article");
        item.className = "bill-item";
        item.innerHTML = `
            <div>
                <strong>${escapeHtml(bill.name)}</strong>
                <span>Vence dia ${bill.dueDay}${category ? ` &middot; ${escapeHtml(category.name)}` : ""}</span>
            </div>
            <div class="item-side">
                <b>${currency.format(bill.amount)}</b>
                <div class="item-actions">
                    <button class="button button--tiny" type="button" data-bill-action="edit" data-bill-id="${bill.id}">Editar</button>
                    <button class="button button--tiny button--danger" type="button" data-bill-action="delete" data-bill-id="${bill.id}">Excluir</button>
                </div>
            </div>
        `;
        list.appendChild(item);
    });
}

async function loadFinancialGoals() {
    $("#goals-status").textContent = "Atualizando...";
    const goals = await api("/api/me/financial-goals");
    renderFinancialGoals(goals);
    $("#goals-status").textContent = `${goals.length} meta${goals.length === 1 ? "" : "s"}`;
}

function renderFinancialGoals(goals) {
    const list = $("#goal-list");
    list.innerHTML = "";
    state.goalsById.clear();

    if (!goals.length) {
        list.innerHTML = '<div class="empty-state">Crie uma meta para acompanhar seu progresso e reservar dinheiro sem perder o limite do dia.</div>';
        $("#goals-status").textContent = "";
        return;
    }

    const sortedGoals = [...goals].sort((left, right) => left.status - right.status || right.priority - left.priority || left.name.localeCompare(right.name));
    sortedGoals.forEach((goal) => {
        state.goalsById.set(goal.id, goal);
        const category = goal.categoryId ? state.categoriesById.get(goal.categoryId) : null;
        const percent = Math.min(100, Math.round((goal.currentAmount / goal.targetAmount) * 100));
        const remaining = Math.max(0, goal.targetAmount - goal.currentAmount);
        const completed = goal.status === 2 || percent >= 100;
        const item = document.createElement("article");
        item.className = "goal-item";
        item.innerHTML = `
            <div class="goal-item__header">
                <div>
                    <strong>${escapeHtml(goal.name)}</strong>
                    <span>${category ? `${escapeHtml(category.name)} &middot; ` : ""}${completed ? "Concluida" : `${percent}% concluida`}</span>
                </div>
                <div class="item-side">
                    <b>${currency.format(goal.currentAmount)} / ${currency.format(goal.targetAmount)}</b>
                    <div class="item-actions">
                        <button class="button button--tiny" type="button" data-goal-action="edit" data-goal-id="${goal.id}">Editar</button>
                        <button class="button button--tiny button--danger" type="button" data-goal-action="delete" data-goal-id="${goal.id}">Excluir</button>
                    </div>
                </div>
            </div>
            <div class="progress" aria-label="Progresso da meta ${escapeHtml(goal.name)}">
                <span style="width: ${percent}%"></span>
            </div>
            <div class="goal-item__details">
                <span>Falta ${currency.format(remaining)}</span>
                <span>Plano mensal ${currency.format(goal.monthlyPlannedAmount)}</span>
            </div>
            <form class="goal-contribution-form" data-goal-contribution-form data-goal-id="${goal.id}">
                <label>
                    Contribuir
                    <input name="amount" type="number" min="0.01" step="0.01" placeholder="0,00" required ${completed ? "disabled" : ""}>
                </label>
                <label>
                    Nota
                    <input name="notes" type="text" placeholder="Opcional" maxlength="200" ${completed ? "disabled" : ""}>
                </label>
                <button class="button button--ghost" type="submit" ${completed ? "disabled" : ""}>Guardar</button>
            </form>
        `;
        list.appendChild(item);
    });
}

async function editIncome(income) {
    const description = prompt("Descricao da renda:", income.description);
    if (description === null) {
        return;
    }

    const amount = promptDecimal("Valor da renda:", income.amount, 0.01);
    if (amount === null) {
        return;
    }

    await api(`/api/me/incomes/${income.id}`, {
        method: "PUT",
        body: {
            categoryId: income.categoryId,
            description,
            amount,
            receivedOn: income.receivedOn,
            isRecurring: income.isRecurring,
            notes: income.notes
        }
    });
    showToast("Renda atualizada. Limite recalculado.");
    await loadDashboard();
}

async function deleteIncome(income) {
    if (!confirm(`Excluir a renda "${income.description}"?`)) {
        return;
    }

    await api(`/api/me/incomes/${income.id}`, { method: "DELETE" });
    showToast("Renda excluida. Limite recalculado.");
    await loadDashboard();
}

async function editExpense(expense) {
    const description = prompt("Descricao da despesa:", expense.description);
    if (description === null) {
        return;
    }

    const amount = promptDecimal("Valor da despesa:", expense.amount, 0.01);
    if (amount === null) {
        return;
    }

    await api(`/api/me/expenses/${expense.id}`, {
        method: "PUT",
        body: {
            categoryId: expense.categoryId,
            description,
            amount,
            spentOn: expense.spentOn,
            paymentMethod: expense.paymentMethod,
            notes: expense.notes
        }
    });
    showToast("Despesa atualizada. Limite recalculado.");
    await loadDashboard();
}

async function deleteExpense(expense) {
    if (!confirm(`Excluir a despesa "${expense.description}"?`)) {
        return;
    }

    await api(`/api/me/expenses/${expense.id}`, { method: "DELETE" });
    showToast("Despesa excluida. Limite recalculado.");
    await loadDashboard();
}

async function editFixedBill(bill) {
    const name = prompt("Nome da conta fixa:", bill.name);
    if (name === null) {
        return;
    }

    const amount = promptDecimal("Valor da conta:", bill.amount, 0.01);
    if (amount === null) {
        return;
    }

    const dueDay = promptInteger("Dia de vencimento:", bill.dueDay, 1, 31);
    if (dueDay === null) {
        return;
    }

    await api(`/api/me/fixed-bills/${bill.id}`, {
        method: "PUT",
        body: {
            categoryId: bill.categoryId,
            name,
            amount,
            dueDay,
            status: bill.status,
            paymentDate: bill.paymentDate,
            isRecurringMonthly: bill.isRecurringMonthly,
            autoIncludeInCycle: bill.autoIncludeInCycle,
            notes: bill.notes
        }
    });
    showToast("Conta fixa atualizada. Limite recalculado.");
    await loadDashboard();
    await loadFixedBills();
}

async function deleteFixedBill(bill) {
    if (!confirm(`Excluir a conta fixa "${bill.name}"?`)) {
        return;
    }

    await api(`/api/me/fixed-bills/${bill.id}`, { method: "DELETE" });
    showToast("Conta fixa excluida. Limite recalculado.");
    await loadDashboard();
    await loadFixedBills();
}

async function editFinancialGoal(goal) {
    const name = prompt("Nome da meta:", goal.name);
    if (name === null) {
        return;
    }

    const targetAmount = promptDecimal("Valor alvo:", goal.targetAmount, 0.01);
    if (targetAmount === null) {
        return;
    }

    const monthlyPlannedAmount = promptDecimal("Quanto guardar por mes:", goal.monthlyPlannedAmount);
    if (monthlyPlannedAmount === null) {
        return;
    }

    await api(`/api/me/financial-goals/${goal.id}`, {
        method: "PUT",
        body: {
            categoryId: goal.categoryId,
            name,
            targetAmount,
            currentAmount: goal.currentAmount,
            monthlyPlannedAmount,
            targetDate: goal.targetDate,
            priority: goal.priority,
            status: goal.status,
            notes: goal.notes
        }
    });
    showToast("Meta atualizada. Painel recalculado.");
    await loadDashboard();
    await loadFinancialGoals();
}

async function deleteFinancialGoal(goal) {
    if (!confirm(`Excluir a meta "${goal.name}"?`)) {
        return;
    }

    await api(`/api/me/financial-goals/${goal.id}`, { method: "DELETE" });
    showToast("Meta excluida. Painel recalculado.");
    await loadDashboard();
    await loadFinancialGoals();
}

function promptDecimal(message, currentValue, min = 0) {
    const value = prompt(message, String(currentValue).replace(".", ","));
    if (value === null) {
        return null;
    }

    const parsed = Number(value.replace(",", "."));
    if (!Number.isFinite(parsed) || parsed < min) {
        showToast("Valor invalido. Tente novamente.");
        return null;
    }

    return parsed;
}

function promptInteger(message, currentValue, min, max) {
    const value = prompt(message, String(currentValue));
    if (value === null) {
        return null;
    }

    const parsed = Number(value);
    if (!Number.isInteger(parsed) || parsed < min || parsed > max) {
        showToast(`Informe um numero entre ${min} e ${max}.`);
        return null;
    }

    return parsed;
}

async function api(path, options = {}) {
    const headers = {
        "Content-Type": "application/json",
        ...(options.headers || {})
    };

    if (options.auth !== false && state.token) {
        headers.Authorization = `Bearer ${state.token}`;
    }

    const response = await fetch(path, {
        method: options.method || "GET",
        headers,
        body: options.body ? JSON.stringify(options.body) : undefined
    });

    if (!response.ok) {
        let message = "Algo saiu do trilho. Tente novamente.";
        try {
            const payload = await response.json();
            message = payload.message || payload.title || message;
        } catch {
            // Keep the friendly fallback when the API has no JSON body.
        }

        const error = new Error(message);
        error.status = response.status;
        showToast(message);
        throw error;
    }

    if (response.status === 204) {
        return null;
    }

    return response.json();
}

function formatDate(value) {
    const [year, month, day] = value.split("-").map(Number);
    return new Intl.DateTimeFormat("pt-BR", {
        day: "2-digit",
        month: "2-digit"
    }).format(new Date(year, month - 1, day));
}

function todayIso() {
    const now = new Date();
    const offset = now.getTimezoneOffset() * 60000;
    return new Date(now.getTime() - offset).toISOString().slice(0, 10);
}

function showToast(message) {
    toast.textContent = message;
    toast.classList.remove("hidden");
    clearTimeout(showToast.timeout);
    showToast.timeout = setTimeout(() => toast.classList.add("hidden"), 3600);
}

function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}
