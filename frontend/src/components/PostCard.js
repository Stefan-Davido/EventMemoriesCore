import React, { useState, useRef, useEffect } from 'react';
import { FiHeart, FiMessageCircle, FiShare2, FiMoreVertical } from 'react-icons/fi';
import './PostCard.css';

function PostCard({ post, showOwnerControls = false, onEdit, onDelete }) {
  const [liked, setLiked] = useState(false);
  const [likes, setLikes] = useState(post.likes || 0);
  const [menuOpen, setMenuOpen] = useState(false);
  const menuRef = useRef(null);

  const handleLike = () => {
    setLiked(!liked);
    setLikes(liked ? likes - 1 : likes + 1);
  };

  useEffect(() => {
    const onDocClick = (e) => {
      if (menuRef.current && !menuRef.current.contains(e.target)) {
        setMenuOpen(false);
      }
    };
    document.addEventListener('click', onDocClick);
    return () => document.removeEventListener('click', onDocClick);
  }, []);

  return (
    <div className="post-card card">
      <div className="post-header">
        <div className="post-author">
          <img 
            src={post.userAvatar || `https://ui-avatars.com/api/?name=${post.userName}&background=1890ff&color=fff`}
            alt={post.userName}
            className="author-avatar"
          />
          <div className="author-info">
            <h3>{post.userName}</h3>
            <p className="author-event">{post.eventName || 'Event'}</p>
          </div>
        </div>

        <div className="post-menu" ref={menuRef}>
          <button className="post-menu-btn" onClick={(e) => { e.stopPropagation(); setMenuOpen(!menuOpen); }}>
            <FiMoreVertical size={20} />
          </button>

          {menuOpen && showOwnerControls && (
            <div className="post-menu-dropdown">
              <button className="menu-item" onClick={() => { setMenuOpen(false); onEdit && onEdit(post); }}>Edit</button>
              <button className="menu-item danger" onClick={() => { setMenuOpen(false); onDelete && onDelete(post.id); }}>Delete</button>
            </div>
          )}
        </div>
      </div>

      {post.mediaUrls && post.mediaUrls.length > 0 && (
        <div className="post-media">
          <img 
            src={post.mediaUrls[0]} 
            alt="Post" 
            onError={(e) => {
              e.target.src = 'https://via.placeholder.com/400x300?text=Media+Not+Available';
            }}
          />
          {post.mediaUrls.length > 1 && (
            <div className="media-count">+{post.mediaUrls.length - 1}</div>
          )}
        </div>
      )}

      <div className="post-content">
        <p className="post-caption">{post.caption}</p>

        <div className="post-stats">
          <span>{likes} likes</span>
          <span>{post.comments || 0} comments</span>
        </div>

        <div className="post-actions">
          <button 
            className={`action-btn ${liked ? 'liked' : ''}`}
            onClick={handleLike}
          >
            <FiHeart size={20} fill={liked ? 'currentColor' : 'none'} />
            Like
          </button>
          <button className="action-btn">
            <FiMessageCircle size={20} />
            Comment
          </button>
          <button className="action-btn">
            <FiShare2 size={20} />
            Share
          </button>
        </div>
      </div>
    </div>
  );
}

export default PostCard;
