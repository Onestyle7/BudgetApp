import { createApp } from "vue";
import App from "./App.vue";
import router from "./router"; // Poprawny import routera

const app = createApp(App);
app.use(router); // Użycie routera jako plugin
app.mount("#app"); // Montowanie aplikacji
