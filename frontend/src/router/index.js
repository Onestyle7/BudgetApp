// Importowanie funkcji do tworzenia routera Vue
import { createRouter, createWebHistory } from "vue-router";
// Importowanie widoków stron
import DashboardView from "@/views/DashboardView.vue";
import LoginView from "@/views/LoginView.vue";

// Definicja tras dla aplikacji
const routes = [
  {
    path: "/", // Ścieżka dla strony głównej (Dashboard)
    name: "Dashboard", // Nazwa trasy
    component: DashboardView, // Powiązany komponent widoku
  },
  {
    path: "/login", // Ścieżka dla strony logowania
    name: "Login",
    component: LoginView,
  },
  {
    path: "/register", // Ścieżka dla strony rejestracji
    name: "Register",
    // Lazy loading: komponent zostanie załadowany tylko po wejściu na tę trasę
    component: () => import("@/views/RegisterView.vue"),
  },
];

// Tworzenie routera z użyciem historii nawigacji (HTML5 history mode)
const router = createRouter({
  history: createWebHistory(),
  routes, // Przekazanie zdefiniowanych tras
});

// Eksport routera do użycia w aplikacji
export default router;
