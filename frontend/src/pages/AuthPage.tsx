import { useState } from "react";
import { useRouter } from "next/router";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { login, register } from "@/app/services/authService";

const AuthPage = () => {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isRegister, setIsRegister] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      if (isRegister) {
        await register(email, password);
        router.push("/login");
      } else {
        const data = await login(email, password);
        localStorage.setItem("token", data.token);
        router.push("/dashboard");
      }
    } catch (error) {
      setError("Nieprawidłowe dane lub błąd serwera.");
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
        {error && <p className="text-red-500 text-center mb-4">{error}</p>}
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
    </div>
  );
};

export default AuthPage;
