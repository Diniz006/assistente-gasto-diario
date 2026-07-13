const state = {
    token: localStorage.getItem("agd.token"),
    user: JSON.parse(localStorage.getItem("agd.user") || "null"),
    dashboard: null,
    cycleSummary: null,
    categoriesById: new Map(),
    categories: [],
    incomesById: new Map(),
    expensesById: new Map(),
    fixedBillsById: new Map(),
    goalsById: new Map(),
    incomeCount: 0,
    expenseCount: 0,
    fixedBillCount: 0,
    goalCount: 0,
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
const editModal = $("#edit-modal");
const editModalForm = $("#edit-modal-form");
let editModalResolver = null;

document.addEventListener("DOMContentLoaded", () => {
    bindAuthTabs();
    bindForms();
    bindEditModal();
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

function bindEditModal() {
    $("#edit-modal-close").addEventListener("click", () => closeEditModal(null));
    editModal.addEventListener("click", (event) => {
        if (event.target === editModal) {
            closeEditModal(null);
        }
    });
    editModalForm.addEventListener("submit", (event) => {
        event.preventDefault();
        const values = Object.fromEntries(new FormData(editModalForm));
        closeEditModal(values);
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
        await loadCycleSummary();
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

    $("#category-form").addEventListener("submit", async (event) => {
        event.preventDefault();
        const form = event.currentTarget;
        const data = Object.fromEntries(new FormData(form));
        $("#categories-status").textContent = "Salvando...";
        await api("/api/me/categories", {
            method: "POST",
            body: {
                name: data.name,
                type: Number(data.type),
                color: data.color || null,
                icon: null
            }
        });
        form.reset();
        form.elements.color.value = "#1f6f4a";
        showToast(`${data.name} entrou nas suas categorias.`);
        await loadCategories();
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

    $("#category-list").addEventListener("click", async (event) => {
        const button = event.target.closest("[data-category-action]");
        if (!button) {
            return;
        }

        const category = state.categoriesById.get(button.dataset.categoryId);
        if (!category) {
            return;
        }

        if (button.dataset.categoryAction === "edit") {
            await editCategory(category);
        } else if (button.dataset.categoryAction === "deactivate") {
            await deactivateCategory(category);
        }
    });

    $("#onboarding-list").addEventListener("click", (event) => {
        const button = event.target.closest("[data-scroll-target]");
        if (!button) {
            return;
        }

        const target = $(button.dataset.scrollTarget);
        target?.scrollIntoView({ behavior: "smooth", block: "start" });
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
        await loadCycleSummary();
        await loadIncomesForCurrentCycle();
        await loadExpensesForCurrentCycle();
    } catch (error) {
        if (error.status === 404) {
            $("#setup-card").classList.remove("hidden");
            renderDashboard(null);
            renderInsights(null);
            renderIncomes([]);
            renderExpenses([]);
            renderOnboarding();
            return;
        }
        throw error;
    }
}

async function loadCategories() {
    state.categoriesById.clear();
    const categoryGroups = await Promise.all([
        api("/api/me/categories?type=1"),
        api("/api/me/categories?type=2"),
        api("/api/me/categories?type=3"),
        api("/api/me/categories?type=4")
    ]);
    state.incomeCategories = categoryGroups[0];
    state.expenseCategories = categoryGroups[1];
    state.billCategories = categoryGroups[2];
    state.goalCategories = categoryGroups[3];
    state.categories = categoryGroups.flat();
    state.categories.forEach((category) => state.categoriesById.set(category.id, category));

    const incomeSelect = $("#income-category-select");
    incomeSelect.innerHTML = '<option value="">Sem categoria</option>';
    state.incomeCategories
        .filter((category) => category.isActive)
        .forEach((category) => {
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
            const option = document.createElement("option");
            option.value = category.id;
            option.textContent = category.name;
            goalSelect.appendChild(option);
        });
    renderCategories();
    if (state.cycleSummary) {
        renderInsights(state.cycleSummary);
    }
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
        renderInsights(null);
        renderIncomes([]);
        renderOnboarding();
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
    renderOnboarding();
}

async function loadCycleSummary() {
    $("#insights-status").textContent = "Atualizando...";
    const summary = await api("/api/me/cycle-summary");
    state.cycleSummary = summary;
    renderInsights(summary);
}

function renderInsights(summary) {
    const flowChart = $("#flow-chart");
    const categoryChart = $("#expense-category-chart");

    if (!summary) {
        $("#insights-status").textContent = "";
        flowChart.innerHTML = '<div class="empty-state">Configure o ciclo para ver os graficos.</div>';
        categoryChart.innerHTML = '<div class="empty-state">Lance despesas para ver o grafico por categoria.</div>';
        return;
    }

    const fixedBillsTotal = Number(summary.fixedBillsTotal || 0);
    const expensesTotal = Number(summary.expensesTotal || 0);
    const goalContributedTotal = Number(summary.goalContributedTotal || 0);
    const goalPlannedTotal = Number(summary.goalPlannedTotal || 0);
    const investedTotal = Math.max(goalContributedTotal, goalPlannedTotal);
    const outgoingTotal = fixedBillsTotal + expensesTotal;
    const flowItems = [
        { label: "Entrou", value: Number(summary.incomeTotal || 0), kind: "income" },
        { label: "Ficou guardado", value: investedTotal, kind: "invested" },
        { label: "Saiu", value: outgoingTotal, kind: "outgoing" }
    ];
    const maxFlow = Math.max(...flowItems.map((item) => item.value), 1);

    $("#insights-status").textContent = `${summary.expenseCount} gasto${summary.expenseCount === 1 ? "" : "s"} no ciclo`;
    flowChart.innerHTML = flowItems.map((item) => {
        const percent = Math.max(3, Math.round((item.value / maxFlow) * 100));
        return `
            <div class="flow-row flow-row--${item.kind}">
                <div class="flow-row__label">
                    <strong>${item.label}</strong>
                    <span>${currency.format(item.value)}</span>
                </div>
                <div class="flow-bar">
                    <span style="width: ${percent}%"></span>
                </div>
            </div>
        `;
    }).join("");

    const categories = [...(summary.expensesByCategory || [])].sort((left, right) => right.amount - left.amount);
    if (!categories.length) {
        categoryChart.innerHTML = '<div class="empty-state">Lance despesas para ver o grafico por categoria.</div>';
        return;
    }

    const maxCategory = Math.max(...categories.map((category) => Number(category.amount)), 1);
    categoryChart.innerHTML = categories.map((category) => {
        const knownCategory = state.categoriesById.get(category.categoryId);
        const color = knownCategory?.color || "#d96b5f";
        const amount = Number(category.amount);
        const percent = Math.max(4, Math.round((amount / maxCategory) * 100));
        return `
            <div class="category-chart-row">
                <div class="category-chart-row__heading">
                    <span><i style="background: ${escapeHtml(color)}"></i>${escapeHtml(category.categoryName)}</span>
                    <strong>${currency.format(amount)}</strong>
                </div>
                <div class="category-chart-bar">
                    <span style="width: ${percent}%; background: ${escapeHtml(color)}"></span>
                </div>
                <small>${category.expenseCount} lancamento${category.expenseCount === 1 ? "" : "s"}</small>
            </div>
        `;
    }).join("");
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
    renderOnboarding();
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
    renderOnboarding();
}

function renderIncomes(incomes) {
    const list = $("#income-list");
    list.innerHTML = "";
    state.incomesById.clear();
    state.incomeCount = incomes.length;

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
    state.expenseCount = expenses.length;

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

function renderOnboarding() {
    const list = $("#onboarding-list");
    if (!list) {
        return;
    }

    const safe = state.dashboard?.safeDailyLimit;
    const steps = [
        {
            title: "Configurar ciclo",
            description: "Informe renda mensal e dia inicial para o app entender seu mes.",
            done: Boolean(state.dashboard),
            target: "#setup-card",
            action: "Configurar"
        },
        {
            title: "Registrar renda",
            description: "Adicione salario ou entrada extra para deixar o limite mais real.",
            done: state.incomeCount > 0 || Number(safe?.incomeTotal || 0) > 0,
            target: ".incomes-panel",
            action: "Adicionar renda"
        },
        {
            title: "Cadastrar conta fixa",
            description: "Inclua aluguel, internet, academia e tudo que volta todo mes.",
            done: state.fixedBillCount > 0,
            target: ".bills-panel",
            action: "Adicionar conta"
        },
        {
            title: "Criar primeira meta",
            description: "Separe dinheiro para reserva, viagem ou qualquer plano importante.",
            done: state.goalCount > 0,
            target: ".goals-panel",
            action: "Criar meta"
        },
        {
            title: "Lancar primeira despesa",
            description: "Registre um gasto real para ver o limite do dia se mexendo.",
            done: state.expenseCount > 0,
            target: ".quick-panel",
            action: "Lancar despesa"
        }
    ];

    const completed = steps.filter((step) => step.done).length;
    const nextStep = steps.find((step) => !step.done);
    $("#onboarding-status").textContent = `${completed} de ${steps.length}`;
    $("#onboarding-progress-bar").style.width = `${Math.round((completed / steps.length) * 100)}%`;

    list.innerHTML = steps.map((step, index) => `
        <article class="onboarding-step ${step.done ? "onboarding-step--done" : ""} ${step === nextStep ? "onboarding-step--next" : ""}">
            <span class="onboarding-step__number">${step.done ? "OK" : index + 1}</span>
            <div>
                <strong>${escapeHtml(step.title)}</strong>
                <p>${escapeHtml(step.description)}</p>
            </div>
            <button class="button button--tiny" type="button" data-scroll-target="${step.target}" ${step.done ? "disabled" : ""}>
                ${step.done ? "Feito" : escapeHtml(step.action)}
            </button>
        </article>
    `).join("");
}

function renderCategories() {
    const list = $("#category-list");
    if (!list) {
        return;
    }

    const activeCategories = state.categories.filter((category) => category.isActive);
    $("#categories-status").textContent = `${activeCategories.length} ativa${activeCategories.length === 1 ? "" : "s"}`;

    if (!state.categories.length) {
        list.innerHTML = '<div class="empty-state">Suas categorias aparecem aqui.</div>';
        return;
    }

    const groups = [
        { type: 1, title: "Rendas" },
        { type: 2, title: "Despesas" },
        { type: 3, title: "Contas fixas" },
        { type: 4, title: "Metas" }
    ];

    list.innerHTML = groups.map((group) => {
        const categories = state.categories
            .filter((category) => category.type === group.type)
            .sort((left, right) => Number(right.isActive) - Number(left.isActive) || left.name.localeCompare(right.name));

        return `
            <section class="category-group">
                <h4>${group.title}</h4>
                <div class="category-items">
                    ${categories.length ? categories.map((category) => renderCategoryItem(category)).join("") : '<div class="empty-state">Nenhuma categoria nesse tipo.</div>'}
                </div>
            </section>
        `;
    }).join("");
}

function renderCategoryItem(category) {
    return `
        <article class="category-item ${category.isActive ? "" : "category-item--inactive"}">
            <span class="category-dot" style="background: ${escapeHtml(category.color || "#1f6f4a")}"></span>
            <div>
                <strong>${escapeHtml(category.name)}</strong>
                <span>${category.isDefault ? "Padrao" : "Personalizada"}${category.isActive ? "" : " &middot; inativa"}</span>
            </div>
            <div class="item-actions">
                <button class="button button--tiny" type="button" data-category-action="edit" data-category-id="${category.id}">Editar</button>
                <button class="button button--tiny button--danger" type="button" data-category-action="deactivate" data-category-id="${category.id}" ${category.isActive ? "" : "disabled"}>Ocultar</button>
            </div>
        </article>
    `;
}

async function loadFixedBills() {
    $("#bills-status").textContent = "Atualizando...";
    const bills = await api("/api/me/fixed-bills");
    renderFixedBills(bills);
    $("#bills-status").textContent = `${bills.length} conta${bills.length === 1 ? "" : "s"}`;
    renderOnboarding();
}

function renderFixedBills(bills) {
    const list = $("#bill-list");
    list.innerHTML = "";
    state.fixedBillsById.clear();
    state.fixedBillCount = bills.length;

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
    renderOnboarding();
}

function renderFinancialGoals(goals) {
    const list = $("#goal-list");
    list.innerHTML = "";
    state.goalsById.clear();
    state.goalCount = goals.length;

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
    const values = await openEditModal("Editar renda", [
        { name: "description", label: "Descricao", value: income.description, maxlength: 160 },
        { name: "amount", label: "Valor", type: "number", value: income.amount, min: "0.01", step: "0.01" }
    ]);
    if (!values) {
        return;
    }

    const amount = parsePositiveNumber(values.amount);
    if (amount === null) {
        return;
    }
    await api(`/api/me/incomes/${income.id}`, {
        method: "PUT",
        body: {
            categoryId: income.categoryId,
            description: values.description,
            amount,
            receivedOn: income.receivedOn,
            isRecurring: income.isRecurring,
            notes: income.notes
        }
    });
    showToast("Renda atualizada. Limite recalculado.");
    await loadDashboard();
}

async function editCategory(category) {
    const values = await openEditModal("Editar categoria", [
        { name: "name", label: "Nome", value: category.name, maxlength: 80 },
        { name: "color", label: "Cor", type: "color", value: category.color || "#1f6f4a" }
    ]);
    if (!values) {
        return;
    }

    await api(`/api/me/categories/${category.id}`, {
        method: "PUT",
        body: {
            name: values.name,
            type: category.type,
            color: values.color || null,
            icon: category.icon,
            isActive: category.isActive
        }
    });
    showToast("Categoria atualizada.");
    await loadCategories();
}

async function deactivateCategory(category) {
    if (!confirm(`Ocultar a categoria "${category.name}"?`)) {
        return;
    }

    await api(`/api/me/categories/${category.id}`, { method: "DELETE" });
    showToast("Categoria ocultada dos formularios.");
    await loadCategories();
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
    const values = await openEditModal("Editar despesa", [
        { name: "description", label: "Descricao", value: expense.description, maxlength: 160 },
        { name: "amount", label: "Valor", type: "number", value: expense.amount, min: "0.01", step: "0.01" }
    ]);
    if (!values) {
        return;
    }

    const amount = parsePositiveNumber(values.amount);
    if (amount === null) {
        return;
    }
    await api(`/api/me/expenses/${expense.id}`, {
        method: "PUT",
        body: {
            categoryId: expense.categoryId,
            description: values.description,
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
    const values = await openEditModal("Editar conta fixa", [
        { name: "name", label: "Nome", value: bill.name, maxlength: 120 },
        { name: "amount", label: "Valor", type: "number", value: bill.amount, min: "0.01", step: "0.01" },
        { name: "dueDay", label: "Vencimento", type: "number", value: bill.dueDay, min: "1", max: "31", step: "1" }
    ]);
    if (!values) {
        return;
    }

    const amount = parsePositiveNumber(values.amount);
    const dueDay = parseIntegerInRange(values.dueDay, 1, 31);
    if (amount === null || dueDay === null) {
        return;
    }
    await api(`/api/me/fixed-bills/${bill.id}`, {
        method: "PUT",
        body: {
            categoryId: bill.categoryId,
            name: values.name,
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
    const values = await openEditModal("Editar meta", [
        { name: "name", label: "Nome", value: goal.name, maxlength: 140 },
        { name: "targetAmount", label: "Valor alvo", type: "number", value: goal.targetAmount, min: "0.01", step: "0.01" },
        { name: "monthlyPlannedAmount", label: "Guardar por mes", type: "number", value: goal.monthlyPlannedAmount, min: "0", step: "0.01" }
    ]);
    if (!values) {
        return;
    }

    const targetAmount = parsePositiveNumber(values.targetAmount);
    const monthlyPlannedAmount = parseNonNegativeNumber(values.monthlyPlannedAmount);
    if (targetAmount === null || monthlyPlannedAmount === null) {
        return;
    }
    await api(`/api/me/financial-goals/${goal.id}`, {
        method: "PUT",
        body: {
            categoryId: goal.categoryId,
            name: values.name,
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

function openEditModal(title, fields) {
    $("#edit-modal-title").textContent = title;
    editModalForm.innerHTML = `
        <div class="modal-form__fields">
            ${fields.map((field) => `
                <label>
                    ${escapeHtml(field.label)}
                    <input
                        name="${escapeHtml(field.name)}"
                        type="${escapeHtml(field.type || "text")}"
                        value="${escapeHtml(field.value ?? "")}"
                        ${field.min ? `min="${escapeHtml(field.min)}"` : ""}
                        ${field.max ? `max="${escapeHtml(field.max)}"` : ""}
                        ${field.step ? `step="${escapeHtml(field.step)}"` : ""}
                        ${field.maxlength ? `maxlength="${escapeHtml(field.maxlength)}"` : ""}
                        required>
                </label>
            `).join("")}
        </div>
        <div class="modal-actions">
            <button class="button button--ghost" type="button" data-modal-cancel>Cancelar</button>
            <button class="button button--primary" type="submit">Salvar alteracoes</button>
        </div>
    `;
    editModalForm.querySelector("[data-modal-cancel]").addEventListener("click", () => closeEditModal(null));
    editModal.classList.remove("hidden");
    editModalForm.querySelector("input")?.focus();

    return new Promise((resolve) => {
        editModalResolver = resolve;
    });
}

function closeEditModal(value) {
    editModal.classList.add("hidden");
    const resolver = editModalResolver;
    editModalResolver = null;
    if (resolver) {
        resolver(value);
    }
}

function parsePositiveNumber(value) {
    const parsed = Number(String(value).replace(",", "."));
    if (!Number.isFinite(parsed) || parsed <= 0) {
        showToast("Informe um valor maior que zero.");
        return null;
    }

    return parsed;
}

function parseNonNegativeNumber(value) {
    const parsed = Number(String(value).replace(",", "."));
    if (!Number.isFinite(parsed) || parsed < 0) {
        showToast("Informe um valor valido.");
        return null;
    }

    return parsed;
}

function parseIntegerInRange(value, min, max) {
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
