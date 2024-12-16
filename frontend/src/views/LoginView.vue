<template>
  <!-- Sekcja formularza logowania -->
  <div class="login">
    <h2>Logowanie</h2>
    <form @submit.prevent="handleLogin">
      <!-- Zapobieganie domyślnej akcji formularza -->
      <input v-model="email" type="email" placeholder="Email" />
      <input v-model="password" type="password" placeholder="Hasło" />
      <button type="submit">Zaloguj</button>
    </form>
  </div>
</template>

<script>
// Importowanie funkcji login z serwisu API
import { login } from "@/services/authService";

export default {
  // Dane komponentu - inicjalizacja pól dla formularza logowania
  data() {
    return {
      email: "", // Zmienna do przechowywania emaila
      password: "", // Zmienna do przechowywania hasła
    };
  },
  methods: {
    // Metoda do obsługi logowania
    async handleLogin() {
      try {
        // Wywołanie funkcji logowania z danymi użytkownika
        const response = await login({
          email: this.email,
          password: this.password,
        });
        console.log(response.data); // Logowanie odpowiedzi z backendu
      } catch (error) {
        console.error("Błąd logowania", error); // Logowanie błędu
      }
    },
  },
};
</script>
