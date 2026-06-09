import React, { useState } from 'react';
import { FiBell, FiClock, FiTrash2 } from 'react-icons/fi';
import './NotificationsPage.css';

function NotificationsPage() {
  const [notifications, setNotifications] = useState(mockNotifications);
  const [filter, setFilter] = useState('all');

  const handleDelete = (id) => {
    setNotifications(prev => prev.filter(n => n.id !== id));
  };

  const handleCreateNotification = () => {
    alert('This would open a form to create a scheduled notification');
  };

  const filteredNotifications = filter === 'all' 
    ? notifications 
    : notifications.filter(n => n.read === (filter === 'unread' ? false : true));

  return (
    <div className="notifications-page">
      <div className="notifications-container">
        <div className="notifications-header">
          <h1>Notifications</h1>
          <button 
            className="btn btn-primary"
            onClick={handleCreateNotification}
          >
            <FiClock size={18} />
            Schedule Notification
          </button>
        </div>

        <div className="notifications-filter">
          <button
            className={`filter-btn ${filter === 'all' ? 'active' : ''}`}
            onClick={() => setFilter('all')}
          >
            All
          </button>
          <button
            className={`filter-btn ${filter === 'unread' ? 'active' : ''}`}
            onClick={() => setFilter('unread')}
          >
            Unread
          </button>
          <button
            className={`filter-btn ${filter === 'read' ? 'active' : ''}`}
            onClick={() => setFilter('read')}
          >
            Read
          </button>
        </div>

        {filteredNotifications.length > 0 ? (
          <div className="notifications-list">
            {filteredNotifications.map(notification => (
              <div 
                key={notification.id} 
                className={`notification-item ${notification.read ? 'read' : 'unread'}`}
              >
                <div className="notification-icon">
                  <FiBell size={20} />
                </div>
                <div className="notification-content">
                  <h3>{notification.title}</h3>
                  <p>{notification.message}</p>
                  <span className="notification-time">{notification.time}</span>
                </div>
                <button
                  className="notification-delete"
                  onClick={() => handleDelete(notification.id)}
                >
                  <FiTrash2 size={18} />
                </button>
              </div>
            ))}
          </div>
        ) : (
          <div className="no-notifications">
            <FiBell size={48} />
            <p>No notifications yet</p>
          </div>
        )}
      </div>
    </div>
  );
}

const mockNotifications = [
  {
    id: '1',
    title: 'New post from Jane Smith',
    message: 'Jane shared a photo from Summer Vacation 2024',
    time: '2 minutes ago',
    read: false
  },
  {
    id: '2',
    title: 'Event reminder',
    message: 'Your "Beach Party" event starts tomorrow',
    time: '1 hour ago',
    read: false
  },
  {
    id: '3',
    title: 'New comment on your post',
    message: 'John Doe commented: "Amazing sunset!"',
    time: '3 hours ago',
    read: true
  },
  {
    id: '4',
    title: 'Event invitation',
    message: 'You\'ve been invited to "Birthday Celebration"',
    time: '1 day ago',
    read: true
  }
];

export default NotificationsPage;
