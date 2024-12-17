import { createRouter, createWebHistory } from "vue-router";
import AuthForm from "../components/AuthForm.vue";
import DashboardView from "../views/DashboardView.vue";

const routes = [
  { path: "/", redirect: "/login" }, // Przekierowanie domyślne
  { path: "/login", name: "Login", component: AuthForm }, // Logowanie
  { path: "/dashboard", name: "Dashboard", component: DashboardView }, // Dashboard
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
