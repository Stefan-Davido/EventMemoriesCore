import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Header from './components/Header';
import Sidebar from './components/Sidebar';
import AuthLayout from './layouts/AuthLayout';
import MainLayout from './layouts/MainLayout';
import LoginPage from './pages/AuthPages/LoginPage';
import RegisterPage from './pages/AuthPages/RegisterPage';
import FeedPage from './pages/FeedPage';
import ProfilePage from './pages/ProfilePage';
import EventPage from './pages/EventPage';
import CreatePostPage from './pages/CreatePostPage';
import NotificationsPage from './pages/NotificationsPage';
import './App.css';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [currentUser, setCurrentUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check if user is logged in
    const token = localStorage.getItem('authToken');
    const user = localStorage.getItem('user');

    if (token && user) {
      setIsAuthenticated(true);
      setCurrentUser(JSON.parse(user));
    }

    setLoading(false);
  }, []);

  const handleLogin = (token, user) => {
    localStorage.setItem('authToken', token);
    localStorage.setItem('user', JSON.stringify(user));
    localStorage.setItem('userId', user.id);
    setIsAuthenticated(true);
    setCurrentUser(user);
  };

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
    localStorage.removeItem('userId');
    setIsAuthenticated(false);
    setCurrentUser(null);
  };

  if (loading) {
    return (
      <div className="flex-center" style={{ height: '100vh' }}>
        <div className="spinner"></div>
      </div>
    );
  }

  return (
    <Router>
      <Routes>
        {/* Auth Routes */}
        <Route element={<AuthLayout />}>
          <Route 
            path="/auth/login" 
            element={<LoginPage onLogin={handleLogin} />} 
          />
          <Route 
            path="/auth/register" 
            element={<RegisterPage onLogin={handleLogin} />} 
          />
        </Route>

        {/* Protected Routes */}
        {isAuthenticated ? (
          <Route element={<MainLayout user={currentUser} onLogout={handleLogout} />}>
            <Route path="/" element={<FeedPage />} />
            <Route path="/profile" element={<ProfilePage user={currentUser} />} />
            <Route path="/event/:eventId" element={<EventPage />} />
            <Route path="/create-post" element={<CreatePostPage />} />
            <Route path="/notifications" element={<NotificationsPage />} />
          </Route>
        ) : (
          <Route path="*" element={<Navigate to="/auth/login" replace />} />
        )}
      </Routes>
    </Router>
  );
}

export default App;
