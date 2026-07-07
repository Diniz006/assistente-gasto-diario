const state = {
    token: localStorage.getItem("agd.token"),
    user: JSON.parse(localStorage.getItem("agd.user") || "null"),
    dashboard: null,
    categoriesById: new Map(),
    expenseCategories: [],
    billCategories: []
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

    $("#logout-button").addEventListener("click", () => {
        state.token = null;
        state.user = null;
        localStorage.removeItem("agd.token");
        localStorage.removeItem("agd.user");
        renderSession();
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
    await loadCategories();
    await loadDashboard();
    await loadFixedBills();
}

async function loadDashboard() {
    try {
        $("#setup-card").classList.add("hidden");
        const dashboard = await api("/api/me/dashboard");
        renderDashboard(dashboard);
        await loadExpensesForCurrentCycle();
    } catch (error) {
        if (error.status === 404) {
            $("#setup-card").classList.remove("hidden");
            renderDashboard(null);
            renderExpenses([]);
            return;
        }
        throw error;
    }
}

async function loadCategories() {
    state.categoriesById.clear();
    state.expenseCategories = await api("/api/me/categories?type=2");
    state.billCategories = await api("/api/me/categories?type=3");

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

function renderExpenses(expenses) {
    const list = $("#expense-list");
    list.innerHTML = "";

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
        const category = state.categoriesById.get(expense.categoryId);
        const item = document.createElement("article");
        item.className = "expense-item";
        item.innerHTML = `
            <div>
                <strong>${escapeHtml(expense.description)}</strong>
                <span>${formatDate(expense.spentOn)}${category ? ` &middot; ${escapeHtml(category.name)}` : ""}</span>
            </div>
            <b>${currency.format(expense.amount)}</b>
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

    if (!bills.length) {
        list.innerHTML = '<div class="empty-state">Cadastre suas contas fixas para o limite diario ficar mais realista.</div>';
        $("#bills-status").textContent = "";
        return;
    }

    const sortedBills = [...bills].sort((left, right) => left.dueDay - right.dueDay || left.name.localeCompare(right.name));
    sortedBills.forEach((bill) => {
        const category = bill.categoryId ? state.categoriesById.get(bill.categoryId) : null;
        const item = document.createElement("article");
        item.className = "bill-item";
        item.innerHTML = `
            <div>
                <strong>${escapeHtml(bill.name)}</strong>
                <span>Vence dia ${bill.dueDay}${category ? ` &middot; ${escapeHtml(category.name)}` : ""}</span>
            </div>
            <b>${currency.format(bill.amount)}</b>
        `;
        list.appendChild(item);
    });
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
