import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { FiHome, FiUser, FiPlus, FiBell, FiCalendar, FiLogOut } from 'react-icons/fi';
import './Sidebar.css';

function Sidebar({ isOpen, user, onLogout }) {
  const location = useLocation();

  const menuItems = [
    { icon: FiHome, label: 'Feed', path: '/' },
    { icon: FiPlus, label: 'Create Post', path: '/create-post' },
    { icon: FiBell, label: 'Notifications', path: '/notifications' },
    { icon: FiUser, label: 'Profile', path: '/profile' },
  ];

  const isActive = (path) => location.pathname === path;

  return (
    <aside className={`sidebar ${isOpen ? 'open' : 'closed'}`}>
      <div className="sidebar-content">
        <nav className="sidebar-menu">
          {menuItems.map((item) => (
            <Link
              key={item.path}
              to={item.path}
              className={`menu-item ${isActive(item.path) ? 'active' : ''}`}
            >
              <item.icon size={20} />
              <span className="menu-label">{item.label}</span>
            </Link>
          ))}
        </nav>
      </div>

      <div className="sidebar-footer">
        <button 
          className="logout-btn"
          onClick={onLogout}
        >
          <FiLogOut size={20} />
          <span>Logout</span>
        </button>
      </div>
    </aside>
  );
}

export default Sidebar;
