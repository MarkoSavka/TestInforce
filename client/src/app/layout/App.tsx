// App.tsx
import { useState, useEffect } from "react";
import "./styles.css";
import Header from "./Header";
import AppRoutes from "../features/router/router";
import { AuthProvider } from "../features/AuthContext";

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      setIsLoggedIn(true);
    }
  }, []);

  return (
    <AuthProvider>
      <div id="app">
        <Header />
        <div className="content">
          <AppRoutes />
        </div>
      </div>
    </AuthProvider>
  );
}

export default App;