<template>
  <div>
    <!-- Główny komponent AuthForm -->
    <AuthForm v-if="showAuth" />

    <!-- Komponenty renderowane przez router -->
    <router-view v-else />
  </div>
</template>

<script>
import AuthForm from "./components/AuthForm.vue";

export default {
  name: "App",
  components: { AuthForm },
  data() {
    return {
      showAuth: true, // Na start pokazuje formularz autoryzacji
    };
  },
};
</script>
