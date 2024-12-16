const BASE_URL = "http://localhost:5050/api/auth";

export const AuthService = {
  async register(data) {
    const response = await fetch(`${BASE_URL}/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    if (!response.ok) throw await response.json();
    return response.json();
  },

  async login(data) {
    const response = await fetch(`${BASE_URL}/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    if (!response.ok) throw await response.json();
    return response.json();
  },
};
