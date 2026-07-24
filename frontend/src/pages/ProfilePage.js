import React, { useState, useEffect } from 'react';
import { userService, postService } from '../services/apiService';
import { FiEdit2, FiMail, FiPhone, FiMapPin } from 'react-icons/fi';
import './ProfilePage.css';
import PostCard from '../components/PostCard';

function ProfilePage({ user }) {
  const [userData, setUserData] = useState(user);
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    userName: user?.userName || '',
    email: user?.email || '',
    phoneNumber: user?.phoneNumber || '',
  });
  const [posts, setPosts] = useState([]);
  const [editingPostId, setEditingPostId] = useState(null);
  const [editingCaption, setEditingCaption] = useState('');

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSave = async () => {
    try {
      const response = await userService.update(user.id, formData);
      setUserData(response.data);
      setIsEditing(false);
      alert('Profile updated successfully!');
    } catch (err) {
      alert('Failed to update profile');
    }
  };

  useEffect(() => {
    const fetchPosts = async () => {
      try {
        if (!user?.id) return;
        const res = await postService.getByUser(user.id);
        setPosts(res.data || []);
      } catch (err) {
        console.error('Failed to fetch posts', err);
      }
    };

    fetchPosts();
  }, [user]);

  const handleDeletePost = async (postId) => {
    if (!window.confirm('Delete this post?')) return;
    try {
      await postService.delete(postId);
      setPosts(prev => prev.filter(p => p.id !== postId));
    } catch (err) {
      alert('Failed to delete post');
    }
  };

  const startEditPost = (post) => {
    setEditingPostId(post.id);
    setEditingCaption(post.caption || '');
  };

  const cancelEditPost = () => {
    setEditingPostId(null);
    setEditingCaption('');
  };

  const saveEditPost = async (postId) => {
    try {
      const res = await postService.update(postId, { caption: editingCaption });
      setPosts(prev => prev.map(p => p.id === postId ? res.data : p));
      cancelEditPost();
    } catch (err) {
      alert('Failed to update post');
    }
  };

  return (
    <div className="profile-page">
      <div className="profile-container">
        <div className="profile-header">
          <div className="profile-avatar">
            <img
              src={`https://ui-avatars.com/api/?name=${userData?.userName || 'User'}&size=120&background=1890ff&color=fff`}
              alt="Profile"
            />
          </div>
          <div className="profile-info">
            <h1>{userData?.userName}</h1>
            <p className="member-since">Member since 2024</p>
          </div>
          <button
            className="btn btn-primary"
            onClick={() => setIsEditing(!isEditing)}
          >
            <FiEdit2 size={18} />
            {isEditing ? 'Cancel' : 'Edit Profile'}
          </button>
        </div>

        {isEditing ? (
          <div className="profile-form">
            <h2>Edit Profile</h2>
            <div className="input-group">
              <label htmlFor="userName">Username</label>
              <input
                type="text"
                id="userName"
                name="userName"
                value={formData.userName}
                onChange={handleChange}
              />
            </div>

            <div className="input-group">
              <label htmlFor="email">Email</label>
              <input
                type="email"
                id="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
              />
            </div>

            <div className="input-group">
              <label htmlFor="phoneNumber">Phone Number</label>
              <input
                type="tel"
                id="phoneNumber"
                name="phoneNumber"
                value={formData.phoneNumber}
                onChange={handleChange}
              />
            </div>

            <button
              className="btn btn-primary"
              onClick={handleSave}
            >
              Save Changes
            </button>
          </div>
        ) : (
          <div className="profile-details">
            <div className="detail-item">
              <FiMail size={20} />
              <div>
                <p className="detail-label">Email</p>
                <p className="detail-value">{userData?.email}</p>
              </div>
            </div>

            <div className="detail-item">
              <FiPhone size={20} />
              <div>
                <p className="detail-label">Phone</p>
                <p className="detail-value">{userData?.phoneNumber || 'Not set'}</p>
              </div>
            </div>

            <div className="detail-item">
              <FiMapPin size={20} />
              <div>
                <p className="detail-label">Location</p>
                <p className="detail-value">Not set</p>
              </div>
            </div>
          </div>
        )}

        <div className="profile-stats">
          <div className="stat">
            <p className="stat-value">{posts.length}</p>
            <p className="stat-label">Posts</p>
          </div>
          <div className="stat">
            <p className="stat-value">128</p>
            <p className="stat-label">Followers</p>
          </div>
          <div className="stat">
            <p className="stat-value">5</p>
            <p className="stat-label">Events</p>
          </div>
        </div>

        <div className="user-posts">
          <h2>Your Posts ({posts.length})</h2>
          {posts.length === 0 ? (
            <p>No posts yet.</p>
          ) : (
            posts.map((post) => (
              <div key={post.id} className="profile-post-item">
                {editingPostId === post.id ? (
                  <div className="edit-post">
                    <textarea
                      value={editingCaption}
                      onChange={(e) => setEditingCaption(e.target.value)}
                      rows={3}
                    />
                    <div className="edit-actions">
                      <button className="btn btn-primary" onClick={() => saveEditPost(post.id)}>Save</button>
                      <button className="btn btn-secondary" onClick={cancelEditPost}>Cancel</button>
                    </div>
                  </div>
                ) : (
                  <>
                    <PostCard
                      post={post}
                      showOwnerControls={true}
                      onEdit={() => startEditPost(post)}
                      onDelete={() => handleDeletePost(post.id)}
                    />
                  </>
                )}
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  );
}

export default ProfilePage;
