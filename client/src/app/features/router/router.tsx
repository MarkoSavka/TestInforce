// router.tsx
import React from 'react';
import { Routes, Route } from 'react-router-dom';
import About from '../../layout/about/About';
import Register from '../../layout/register/Register';
import UrlsListComponent from '../../layout/Url/UrlsListComponent';
import Login from '../../layout/login/Login';
import Home from '../../layout/home/Home';
import Logout from '../../layout/login/Logout';


const AppRoutes: React.FC = () => (
  <Routes>
    <Route path="/" element={<Home />} />
    <Route path="/about" element={<About />} />
    <Route path="/register" element={<Register />} />
    <Route path="/login" element={<Login />} />
    <Route path="/urls" element={<UrlsListComponent />} />
    <Route path="/logout" element={<Logout />} />

  </Routes>
);

export default AppRoutes;