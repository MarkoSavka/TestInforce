// Logout.tsx
import { useEffect } from 'react';
import { useAuth } from '../../features/AuthContext';
import { useNavigate } from 'react-router-dom';

export default function Logout() {
  const { logout } = useAuth();
  const navigate = useNavigate();


  useEffect(() => {
    logout();
    navigate('/login'); 
  }, [logout, navigate]);

  return null;
}