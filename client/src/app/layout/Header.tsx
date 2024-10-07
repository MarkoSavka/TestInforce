// Header.tsx
import { Link } from "react-router-dom";
import './Header.css';
import { useAuth } from "../features/AuthContext";

export default function Header() {
  const { isLoggedIn } = useAuth();

  return (
    <header className="header">
      <nav className="nav">
        <ul className="nav-list">
          <li className="nav-item"><Link to="/" className="nav-link">Home</Link></li>
          <li className="nav-item"><Link to="/urls" className="nav-link">Urls</Link></li>
          <li className="nav-item"><Link to="/about" className="nav-link">About</Link></li>
        </ul>
        <ul className="auth-list">
          {isLoggedIn ? (
            <li className="auth-item"><Link to="/logout" className="auth-link">Logout</Link></li>
          ) : (
            <>
              <li className="auth-item"><Link to="/login" className="auth-link">Login</Link></li>
              <li className="auth-item"><Link to="/register" className="auth-link">Register</Link></li>
            </>
          )}
        </ul>
      </nav>
    </header>
  );
}