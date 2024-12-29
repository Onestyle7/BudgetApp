import { useState, useMemo, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Card } from "@/components/ui/card";
import { PieChart, Pie, Cell, Tooltip, Legend } from "recharts";
import axios from "axios";

axios.defaults.baseURL = "http://localhost:5050";

// Definicja typu Transaction
type Transaction = {
  id: number;
  name: string;
  amount: number;
  category: { id: number; name: string; color: string };
};

// Kategorie transakcji
const categories = [
  { id: 1, name: "Subscriptions", color: "#FF6384" },
  { id: 2, name: "Groceries", color: "#36A2EB" },
  { id: 3, name: "Rent", color: "#FFCE56" },
  { id: 4, name: "Utilities", color: "#4BC0C0" },
  { id: 5, name: "Transportation", color: "#9966FF" },
  { id: 6, name: "Health", color: "#FF9F40" },
  { id: 7, name: "Entertainment", color: "#FF6384" },
  { id: 8, name: "Other", color: "#C9CBCF" },
];

const Dashboard = () => {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [name, setName] = useState<string>("");
  const [amount, setAmount] = useState<string>("");
  const [category, setCategory] = useState<string>("1");

  const fetchTransactions = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem("token");
      const response = await axios.get("/api/Transaction/all", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      const mappedTransactions = response.data.map((t) => ({
        ...t,
        category: categories.find((c) => c.id === t.category) || {
          id: 0,
          name: "Nieznana",
          color: "#000000",
        },
      }));

      setTransactions(mappedTransactions);
    } catch (error) {
      console.error("Błąd podczas ładowania danych:", error);
      alert("Nie udało się pobrać danych z serwera!");
    } finally {
      setLoading(false);
    }
  };

  const addTransaction = async () => {
    if (!name.trim()) {
      alert("Nazwa transakcji nie może być pusta!");
      return;
    }
    if (!amount || parseFloat(amount) <= 0) {
      alert("Kwota musi być większa od 0!");
      return;
    }

    const categoryObj = categories.find((c) => c.id === parseInt(category));
    if (!categoryObj) {
      alert("Nieprawidłowa kategoria");
      return;
    }

    try {
      const token = localStorage.getItem("token");
      if (!token) {
        alert("Brak tokena. Zaloguj się ponownie.");
        return;
      }

      const newTransaction = {
        name,
        amount: parseFloat(amount),
        category: categoryObj.id,
        date: new Date().toISOString(), // Przesyłanie aktualnej daty
      };

      try {
        const response = await axios.post(
          "/api/Transaction/add",
          newTransaction,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        console.log("Sukces:", response.data);
      } catch (error) {
        if (axios.isAxiosError(error)) {
          console.error(
            "Błąd API:",
            error.response?.status,
            error.response?.data
          );
        } else {
          console.error("Inny błąd:", error);
        }
      }
      fetchTransactions();
      setName("");
      setAmount("");
      setCategory("1");
    } catch (error) {
      console.error("Błąd podczas dodawania transakcji:", error);
      alert("Nie udało się dodać transakcji!");
    }
  };

  useEffect(() => {
    fetchTransactions();
  }, []);

  const data = useMemo(() => {
    return categories.map((cat) => {
      const total = transactions
        .filter(
          (t) =>
            typeof t.category === "number"
              ? t.category === cat.id // Obsługa liczby
              : t.category.id === cat.id // Obsługa obiektu
        )
        .reduce((sum, t) => sum + t.amount, 0);
      return { name: cat.name, value: total, color: cat.color };
    });
  }, [transactions]);

  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>

      <Card className="p-6 mb-6">
        <h2 className="text-xl font-semibold mb-4">Dodaj transakcję</h2>
        <div className="flex gap-4 mb-4">
          <Input
            placeholder="Nazwa"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
          <Input
            type="number"
            placeholder="Kwota"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
          />
          <Select
            value={category}
            onValueChange={(value) => setCategory(value)}
          >
            <SelectTrigger>
              <SelectValue placeholder="Wybierz kategorię" />
            </SelectTrigger>
            <SelectContent>
              {categories.map((cat) => (
                <SelectItem key={cat.id} value={cat.id.toString()}>
                  {cat.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Button onClick={addTransaction}>Dodaj</Button>
        </div>
      </Card>

      <Card className="p-6 mb-6">
        <h2 className="text-xl font-semibold mb-4">Wykres wydatków</h2>
        <PieChart width={500} height={400}>
          <Pie
            data={data}
            cx={250}
            cy={200}
            labelLine={false}
            outerRadius={150}
            fill="#8884d8"
            dataKey="value"
          >
            {data.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={entry.color} />
            ))}
          </Pie>
          <Tooltip />
          <Legend layout="horizontal" verticalAlign="bottom" align="center" />
        </PieChart>
      </Card>

      <Card className="p-6">
        <h2 className="text-xl font-semibold mb-4">Historia transakcji</h2>
        {loading ? (
          <p>Ładowanie danych...</p>
        ) : (
          <table className="table-auto w-full">
            <thead>
              <tr>
                <th className="border px-4 py-2">Nazwa</th>
                <th className="border px-4 py-2">Kwota</th>
                <th className="border px-4 py-2">Kategoria</th>
              </tr>
            </thead>
            <tbody>
              {transactions.map((transaction) => (
                <tr key={transaction.id}>
                  <td className="border px-4 py-2">{transaction.name}</td>
                  <td className="border px-4 py-2">{transaction.amount} zł</td>
                  <td className="border px-4 py-2">
                    {transaction.category?.name ?? "Brak"}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </Card>
    </div>
  );
};

export default Dashboard;
