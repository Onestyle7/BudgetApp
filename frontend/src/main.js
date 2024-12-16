import { createApp } from "vue";
import App from "./App.vue";
import router from "./router"; // Import routera

const app = createApp(App);
app.use(router); // Wstrzyknięcie routera
app.mount("#app");
