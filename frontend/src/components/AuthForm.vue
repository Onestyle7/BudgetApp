<template>
  <div class="auth-container">
    <!-- Header aplikacji -->
    <div class="auth-header">
      <h1>BudgetApp</h1>
    </div>

    <div class="auth-card">
      <h2>{{ isLogin ? "Logowanie" : "Rejestracja" }}</h2>

      <!-- Formularz logowania -->
      <form v-if="isLogin" @submit.prevent="handleLogin">
        <div class="input-group">
          <label>Email:</label>
          <input
            type="email"
            v-model="loginData.email"
            placeholder="Wprowadź email"
            required
          />
        </div>
        <div class="input-group">
          <label>Hasło:</label>
          <input
            type="password"
            v-model="loginData.password"
            placeholder="Wprowadź hasło"
            required
          />
        </div>
        <button type="submit">Zaloguj</button>
      </form>

      <!-- Formularz rejestracji -->
      <form v-else @submit.prevent="handleRegister">
        <div class="input-group">
          <label>Imię:</label>
          <input
            type="text"
            v-model="registerData.firstName"
            placeholder="Wprowadź imię"
            required
          />
        </div>
        <div class="input-group">
          <label>Nazwisko:</label>
          <input
            type="text"
            v-model="registerData.lastName"
            placeholder="Wprowadź nazwisko"
            required
          />
        </div>
        <div class="input-group">
          <label>Email:</label>
          <input
            type="email"
            v-model="registerData.email"
            placeholder="Wprowadź email"
            required
          />
        </div>
        <div class="input-group">
          <label>Hasło:</label>
          <input
            type="password"
            v-model="registerData.password"
            placeholder="Wprowadź hasło"
            required
          />
        </div>
        <button type="submit">Zarejestruj</button>
      </form>

      <!-- Przełączanie między logowaniem a rejestracją -->
      <p>
        <span v-if="isLogin"
          >Nie masz konta?
          <a href="#" @click="toggleForm">Zarejestruj się</a></span
        >
        <span v-else
          >Masz już konto? <a href="#" @click="toggleForm">Zaloguj się</a></span
        >
      </p>
    </div>
  </div>
</template>

<script>
import { ref } from "vue"; // Importowanie ref z Composition API
import { useRouter } from "vue-router"; // Importowanie routera

const BASE_URL = "http://localhost:5050/api/auth";

export default {
  setup() {
    const router = useRouter(); // Pobranie instancji routera

    const isLogin = ref(true); // Przełączanie formularza
    const loginData = ref({ email: "", password: "" });
    const registerData = ref({
      firstName: "",
      lastName: "",
      email: "",
      password: "",
    });

    // Obsługa logowania
    const handleLogin = async () => {
      try {
        const response = await fetch(`${BASE_URL}/login`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(loginData.value),
        });

        if (!response.ok) {
          const errorData = await response.json();
          alert(`Błąd logowania: ${errorData.message || "Nieprawidłowe dane"}`);
          return;
        }

        const data = await response.json();
        localStorage.setItem("token", data.token);
        alert("Zalogowano pomyślnie!");
        router.push("/dashboard");
      } catch (error) {
        console.error("Błąd logowania:", error);
        alert("Wystąpił błąd podczas logowania.");
      }
    };

    // Obsługa rejestracji
    const handleRegister = async () => {
      try {
        const response = await fetch(`${BASE_URL}/register`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(registerData.value),
        });

        if (!response.ok) {
          const errorData = await response.json();
          alert(`Błąd rejestracji: ${errorData.message || "Spróbuj ponownie"}`);
          return;
        }

        alert("Rejestracja zakończona sukcesem!");
        isLogin.value = true; // Przełączenie na logowanie
      } catch (error) {
        console.error("Błąd rejestracji:", error);
        alert("Wystąpił błąd podczas rejestracji.");
      }
    };

    const toggleForm = () => {
      isLogin.value = !isLogin.value;
    };

    return {
      isLogin,
      loginData,
      registerData,
      handleLogin,
      handleRegister,
      toggleForm,
    };
  },
};
</script>

<style scoped>
.auth-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f5f5f5;
  font-family: Arial, sans-serif;
}

.auth-header {
  text-align: center;
  margin-bottom: 2rem;
}

.auth-header h1 {
  font-size: 2rem;
  color: #42b983;
  font-weight: bold;
}

.auth-card {
  background: #fff;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
  text-align: center;
}

.input-group {
  margin-bottom: 1rem;
  text-align: left;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
}

input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}

button {
  background-color: #42b983;
  color: #fff;
  border: none;
  padding: 0.75rem;
  width: 100%;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
}

button:hover {
  background-color: #369870;
}

p {
  margin-top: 1rem;
}

a {
  color: #42b983;
  text-decoration: none;
  font-weight: bold;
}

a:hover {
  text-decoration: underline;
}
</style>
