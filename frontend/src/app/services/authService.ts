import axios from "axios";

const API_URL = "http://localhost:5050/api/auth";

export interface LoginResponse {
  token: string;
}

export const login = async (
  email: string,
  password: string
): Promise<LoginResponse> => {
  try {
    const response = await axios.post<LoginResponse>(`${API_URL}/login`, {
      email,
      password,
    });
    return response.data;
  } catch (error) {
    console.error("Błąd logowania:", error);
    throw new Error("Invalid credentials");
  }
};
export const register = async (email: string, password: string) => {
  const response = await fetch("/api/register", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });
  if (!response.ok) {
    throw new Error("Nie udało się zarejestrować.");
  }
  return await response.json();
};
