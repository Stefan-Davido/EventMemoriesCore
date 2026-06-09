import React from 'react';
import { Outlet } from 'react-router-dom';
import './AuthLayout.css';

function AuthLayout() {
  return (
    <div className="auth-layout">
      <div className="auth-container">
        <div className="auth-brand">
          <h1>📸 EventMemories</h1>
          <p>Share your special moments</p>
        </div>
        <Outlet />
      </div>
      <div className="auth-background"></div>
    </div>
  );
}

export default AuthLayout;
