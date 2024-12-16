// Importowanie biblioteki Axios do wykonywania żądań HTTP
import axios from "axios";

// Adres URL API backendu
const API_URL = "http://localhost:5000/api/";

// Funkcja do logowania użytkownika
// Przyjmuje obiekt credentials zawierający dane logowania (email, password)
// Wysyła żądanie POST na endpoint "auth/login"
export const login = async (credentials) => {
  return axios.post(`${API_URL}auth/login`, credentials);
};

// Funkcja do rejestracji nowego użytkownika
// Przyjmuje obiekt data zawierający dane rejestracyjne
// Wysyła żądanie POST na endpoint "auth/register"
export const register = async (data) => {
  return axios.post(`${API_URL}auth/register`, data);
};
