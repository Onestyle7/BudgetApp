<template>
  <div class="dashboard">
    <h1>Witaj na swoim dashboardzie!</h1>
    <p>To jest Twoja strona główna po zalogowaniu.</p>
    <button @click="handleLogout">Wyloguj</button>
  </div>
</template>

<script>
export default {
  name: "Dashboard",
  methods: {
    handleLogout() {
      // Usunięcie tokena z localStorage
      localStorage.removeItem("token");

      // Przekierowanie na stronę logowania
      this.$router.push("/login");
    },
  },
};
</script>

<style scoped>
.dashboard {
  text-align: center;
  margin-top: 2rem;
}

button {
  margin-top: 1rem;
  background-color: #42b983;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 5px;
  cursor: pointer;
  font-size: 1rem;
}

button:hover {
  background-color: #369870;
}
</style>
