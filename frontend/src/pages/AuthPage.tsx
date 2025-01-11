import { useState, useEffect } from "react";
import { useRouter } from "next/router";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { login, register } from "@/app/services/authService";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import axios from "axios";
// Inicjalizacja ToastContainer
import { ToastContainer } from "react-toastify";

const AuthPage = () => {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isRegister, setIsRegister] = useState(false);

  useEffect(() => {
    toast.dismiss(); // Wyczyść wcześniejsze komunikaty
  }, []);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    // Walidacja emaila
    if (!email || !email.includes("@")) {
      toast.error("Wprowadź poprawny adres email.");
      return;
    }

    // Walidacja hasła
    if (!password || password.length < 6) {
      toast.error("Hasło musi mieć co najmniej 6 znaków.");
      return;
    }
    try {
      if (isRegister) {
        await register(email, password);
        toast.success("Rejestracja zakończona sukcesem! Zaloguj się.");
        router.push("/login");
      } else {
        const data = await login(email, password);
        localStorage.setItem("token", data.token);
        toast.success("Logowanie udane!");
        router.push("/dashboard");
      }
    } catch (error) {
      console.error("Szczegóły błędu logowania:", error);
      if (error.response) {
        const status = error.response.status;
        const message = error.response.data.message || "Wystąpił błąd.";

        if (status === 401) {
          toast.error("Nieprawidłowe dane logowania. Spróbuj ponownie.");
        } else if (status === 400) {
          toast.error(message);
        } else if (status === 500) {
          toast.error("Błąd serwera. Skontaktuj się z administratorem.");
        } else {
          toast.error(`Błąd: ${message}`);
        }
      } else {
        toast.error("Nie udało się połączyć z serwerem. Spróbuj później.");
      }
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500">
      <div className="max-w-md w-full bg-white shadow-lg rounded-lg p-8">
        <h1 className="text-4xl font-bold text-center mb-6 text-gray-800">
          BudgetApp
        </h1>
        <h2 className="text-2xl font-semibold text-center mb-4">
          {isRegister ? "Rejestracja" : "Logowanie"}
        </h2>
        <form onSubmit={handleSubmit} className="space-y-6">
          <Input
            type="email"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
          <Input
            type="password"
            placeholder="Hasło"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <Button type="submit" className="w-full">
            {isRegister ? "Zarejestruj się" : "Zaloguj się"}
          </Button>
        </form>
        <div className="text-center mt-6">
          <button
            className="text-sm text-indigo-600 hover:underline"
            onClick={() => setIsRegister(!isRegister)}
          >
            {isRegister
              ? "Masz już konto? Zaloguj się"
              : "Nie masz konta? Zarejestruj się"}
          </button>
        </div>
      </div>
      <ToastContainer />
    </div>
  );
};

export default AuthPage;
