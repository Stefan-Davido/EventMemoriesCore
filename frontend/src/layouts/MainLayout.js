import React from 'react';
import { Outlet } from 'react-router-dom';
import Header from '../components/Header';
import Sidebar from '../components/Sidebar';
import './MainLayout.css';

function MainLayout({ user, onLogout }) {
  const [sidebarOpen, setSidebarOpen] = React.useState(true);

  return (
    <div className="main-layout">
      <Sidebar isOpen={sidebarOpen} user={user} onLogout={onLogout} />
      <div className="main-content">
        <Header 
          onToggleSidebar={() => setSidebarOpen(!sidebarOpen)}
          user={user}
        />
        <div className="main-area">
          <Outlet />
        </div>
      </div>
    </div>
  );
}

export default MainLayout;
