import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FiMenu, FiBell, FiUser, FiLogOut, FiCalendar } from 'react-icons/fi';
import './Header.css';
import { eventService } from '../services/apiService';

function Header({ onToggleSidebar, user }) {
  const navigate = useNavigate();
  const [notificationCount] = React.useState(3); // Mock data
  const [events, setEvents] = React.useState([]);
  const [selectedEventId, setSelectedEventId] = React.useState('');

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
    localStorage.removeItem('userId');
    localStorage.removeItem('selectedEventId');
    navigate('/auth/login');
  };

  const fetchEvents = async () => {
      try {
        const response = await eventService.getByOwner(localStorage.getItem('userId'));
        setEvents(response.data);
      } catch (err) {
        // do something
      }
    };

  React.useEffect(() => {
    fetchEvents();
    localStorage.setItem("selectedEventId", selectedEventId);
  }, []);

  return (
    <header className="header">
      <div className="header-left">
        <button className="header-toggle" onClick={onToggleSidebar}>
          <FiMenu size={24} />
        </button>
        <h1 className="header-title">EventMemories</h1>
      </div>

      <div className="header-right">
        
        <div className="header-user ">
            <label htmlFor="eventId"><FiCalendar size={24} /></label>
            <select
              id="eventId"
              name="eventId"
              value={selectedEventId}
              onChange={(e) => setSelectedEventId(e.target.value)}
            >
              <option value="">Choose an event...</option>
              {events.map(event => (
                <option key={event.id} value={event.id}>
                  {event.name}
                </option>
              ))}
            </select>
          </div>
          
       
       
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
