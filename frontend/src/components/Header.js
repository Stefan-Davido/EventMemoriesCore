import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FiMenu, FiBell, FiUser, FiLogOut } from 'react-icons/fi';
import './Header.css';

function Header({ onToggleSidebar, user }) {
  const navigate = useNavigate();
  const [notificationCount] = React.useState(3); // Mock data

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
    localStorage.removeItem('userId');
    navigate('/auth/login');
  };

  return (
    <header className="header">
      <div className="header-left">
        <button className="header-toggle" onClick={onToggleSidebar}>
          <FiMenu size={24} />
        </button>
        <h1 className="header-title">EventMemories</h1>
      </div>

      <div className="header-right">
        <Link to="/notifications" className="header-icon-btn">
          <FiBell size={24} />
          {notificationCount > 0 && (
            <span className="notification-badge">{notificationCount}</span>
          )}
        </Link>

        <div className="header-user">
          <img 
            src={`https://ui-avatars.com/api/?name=${user?.userName || 'User'}&background=1890ff&color=fff`}
            alt="User"
            className="user-avatar"
          />
          <span className="user-name">{user?.userName}</span>
        </div>

        <button 
          className="header-icon-btn"
          onClick={handleLogout}
          title="Logout"
        >
          <FiLogOut size={24} />
        </button>
      </div>
    </header>
  );
}

export default Header;
