<template>
  <!-- Główny szablon aplikacji -->
  <img alt="Vue logo" src="./assets/logo.png" />
  <!-- Wyświetlenie logo Vue -->
  <LoginView />
  <!-- Osadzenie komponentu logowania -->
</template>

<script>
// Importowanie komponentu LoginView
import LoginView from "./views/LoginView.vue";

export default {
  name: "App", // Nazwa komponentu głównego aplikacji
  components: {
    LoginView, // Rejestracja komponentu do użycia w szablonie
  },
};
</script>

<style>
/* Globalne style dla aplikacji */
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif; /* Główna czcionka */
  -webkit-font-smoothing: antialiased; /* Wygładzanie tekstu dla przeglądarek Webkit */
  -moz-osx-font-smoothing: grayscale; /* Wygładzanie tekstu dla macOS */
  text-align: center; /* Wyśrodkowanie tekstu */
  color: #2c3e50; /* Główny kolor tekstu */
  margin-top: 60px; /* Odstęp od góry strony */
}
</style>
