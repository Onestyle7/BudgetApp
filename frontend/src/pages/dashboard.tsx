import { useState, useMemo, useEffect } from "react";
import axios from "axios";
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

// Zakładam, że Twój backend jest na http://localhost:5050
axios.defaults.baseURL = "http://localhost:5050";

// Definicja typu Transaction (opcjonalnie, jeśli używasz TypeScript)
type Transaction = {
  id: number;
  name: string;
  amount: number;
  date: string;
  // category po zmapowaniu to będzie obiekt z frontu
  category: {
    id: number;
    name: string;
    color: string;
  };
  userName?: string;
};

// Kategorie
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
  const [loading, setLoading] = useState(false);

  // Formularz
  const [name, setName] = useState("");
  const [amount, setAmount] = useState("");
  const [category, setCategory] = useState("1");

  // -------------------
  // 1. Pobieranie transakcji
  // -------------------
  const fetchTransactions = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem("token");

      const response = await axios.get("/api/Transaction/all", {
        headers: { Authorization: `Bearer ${token}` },
      });

      // API zwraca np.
      // [
      //   {
      //     "id": 17,
      //     "amount": 29,
      //     "date": "2025-01-05T17:25:54.227",
      //     "category": "Subscriptions",
      //     "userName": "TestName testSurname",
      //     "name": "Netflix"
      //   },
      //   ...
      // ]

      const mapped = response.data.map((t: any) => {
        // Szukamy kategorii po polu `name`:
        const foundCat = categories.find((c) => c.name === t.category);

        return {
          ...t,
          // Nadpisujemy field `category` obiektem z frontu
          category: foundCat || {
            id: 0,
            name: "Nieznana",
            color: "#000000",
          },
        };
      });

      setTransactions(mapped);
    } catch (error) {
      console.error("Błąd podczas ładowania danych:", error);
      alert("Nie udało się pobrać danych z serwera!");
    } finally {
      setLoading(false);
    }
  };

  // -------------------
  // 2. Dodawanie transakcji
  // -------------------
  const addTransaction = async () => {
    if (!name.trim()) {
      alert("Nazwa transakcji nie może być pusta!");
      return;
    }
    if (!amount || parseFloat(amount) <= 0) {
      alert("Kwota musi być większa od 0!");
      return;
    }

    const catObj = categories.find((c) => c.id === parseInt(category));
    if (!catObj) {
      alert("Nieprawidłowa kategoria!");
      return;
    }

    const token = localStorage.getItem("token");
    if (!token) {
      alert("Brak tokena. Zaloguj się ponownie.");
      return;
    }

    try {
      // Wysyłamy do backendu w formacie, który on obsługuje
      // (Skoro zwraca "category": "Subscriptions", to zapewne
      //  oczekuje też stringa np. "Subscriptions" ?)
      // Jeżeli jednak oczekuje integera (enuma), to musisz zmienić
      //  tu na catObj.id. Zależnie od tego, jak jest w kodzie .NET.
      const newTransaction = {
        name,
        amount: parseFloat(amount),
        date: new Date().toISOString(),
        // Jeśli Twój .NET przyjmuje "Subscriptions", "Rent" itp.:
        category: catObj.name,
      };

      await axios.post("/api/Transaction/add", newTransaction, {
        headers: { Authorization: `Bearer ${token}` },
      });

      // Odświeżamy listę
      fetchTransactions();

      // Reset
      setName("");
      setAmount("");
      setCategory("1");
    } catch (error) {
      console.error("Błąd podczas dodawania transakcji:", error);
      alert("Nie udało się dodać transakcji!");
    }
  };
  const deleteTransaction = async (id: number) => {
    try {
      const token = localStorage.getItem("token");
      await axios.delete(`/api/Transaction/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchTransactions();
    } catch (error) {
      console.error("Błąd podczas usuwania transakcji:", error);
      alert("Nie udało się usunąć transakcji!");
    }
  };
  // -------------------
  // 3. useEffect
  // -------------------
  useEffect(() => {
    fetchTransactions();
  }, []);

  // -------------------
  // 4. Wykres
  // -------------------
  const data = useMemo(() => {
    return categories.map((cat) => {
      const total = transactions
        .filter((t) => t.category?.name === cat.name)
        .reduce((sum, t) => sum + t.amount, 0);
      return { name: cat.name, value: total, color: cat.color };
    });
  }, [transactions]);

  // -------------------
  // 5. Render
  // -------------------
  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>

      <Card className="p-6 mb-6">
        <h2 className="text-xl font-semibold mb-4">Dodaj transakcję</h2>
        <div className="flex gap-4 mb-4">
          <Input
            placeholder="Nazwa transakcji"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
          <Input
            type="number"
            placeholder="Kwota"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
          />
          <Select value={category} onValueChange={(val) => setCategory(val)}>
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
                <th className="border px-4 py-2">Akcje</th>
              </tr>
            </thead>
            <tbody>
              {transactions.map((tx) => (
                <tr key={tx.id}>
                  <td className="border px-4 py-2">{tx.name}</td>
                  <td className="border px-4 py-2">{tx.amount} zł</td>
                  <td className="border px-4 py-2">{tx.category.name}</td>
                  <td className="border px-4 py-2">
                    <Button
                      variant="destructive"
                      onClick={() => deleteTransaction(tx.id)}
                    >
                      Usuń
                    </Button>
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
