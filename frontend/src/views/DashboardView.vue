<template>
  <div class="dashboard">
    <h1>Witaj w swoim dashboardzie!</h1>

    <!-- Sekcja Podsumowanie -->
    <div class="summary">
      <div class="summary-box">
        <h2>Saldo</h2>
        <p>{{ totalBalance }} zł</p>
      </div>
      <div class="summary-box">
        <h2>Przychody</h2>
        <p>{{ totalIncome }} zł</p>
      </div>
      <div class="summary-box">
        <h2>Wydatki</h2>
        <p>{{ totalExpenses }} zł</p>
      </div>
    </div>

    <!-- Sekcja Ostatnie transakcje -->
    <div class="recent-transactions">
      <h2>Ostatnie transakcje</h2>
      <table>
        <thead>
          <tr>
            <th>Data</th>
            <th>Kategoria</th>
            <th>Kwota</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="transaction in recentTransactions" :key="transaction.date">
            <td>{{ transaction.date }}</td>
            <td>{{ transaction.category }}</td>
            <td>{{ transaction.amount }} zł</td>
          </tr>
        </tbody>
      </table>
    </div>

    <button @click="handleLogout">Wyloguj</button>
  </div>
</template>

<script>
import "@/assets/styles/DashboardStyle.css";
export default {
  name: "DashboardView",
  data() {
    return {
      totalBalance: 5000,
      totalIncome: 8000,
      totalExpenses: 3000,
      recentTransactions: [
        { date: "2024-12-15", category: "Jedzenie", amount: -50 },
        { date: "2024-12-14", category: "Transport", amount: -100 },
        { date: "2024-12-12", category: "Wynagrodzenie", amount: 5000 },
      ],
    };
  },
  methods: {
    handleLogout() {
      localStorage.removeItem("token");
      this.$router.push("/login");
    },
  },
};
</script>

<style scoped>
.dashboard {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  font-family: Arial, sans-serif;
}

h1 {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 2rem;
}

.summary {
  display: flex;
  justify-content: space-between;
  margin-bottom: 2rem;
}

.summary-box {
  flex: 1;
  margin: 0 1rem;
  padding: 1rem;
  background-color: #f9f9f9;
  border-radius: 8px;
  text-align: center;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

table {
  width: 100%;
  border-collapse: collapse;
}

table th,
table td {
  border: 1px solid #ddd;
  padding: 0.75rem;
  text-align: center;
}

table th {
  background-color: #f4f4f4;
  font-weight: bold;
}

button {
  width: 100%;
  margin-top: 2rem;
  padding: 0.75rem;
  background-color: #42b983;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-color: #369870;
}
</style>
