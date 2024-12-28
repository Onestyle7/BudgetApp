// components/PrivateRoute.tsx
import { useEffect } from "react";
import { useRouter } from "next/router";
import { JSX } from "react/jsx-runtime";

const PrivateRoute = ({ children }: { children: JSX.Element }) => {
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) {
      router.push("/login"); // Jeśli brak tokena, przekieruj na stronę logowania
    }
  }, [router]);

  return <>{children}</>;
};

export default PrivateRoute;
