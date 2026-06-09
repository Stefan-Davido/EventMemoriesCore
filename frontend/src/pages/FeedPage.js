import React, { useState, useEffect } from 'react';
import { postService } from '../services/apiService';
import PostCard from '../components/PostCard';
import './FeedPage.css';

function FeedPage() {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchPosts();
  }, []);

  const fetchPosts = async () => {
    try {
      setLoading(true);
      const response = await postService.getAll();
      setPosts(response.data);
      setError('');
    } catch (err) {
      setError('Failed to load posts');
      // Mock data for demo
      setPosts(mockPosts);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="feed-page">
        <div className="flex-center">
          <div className="spinner"></div>
        </div>
      </div>
    );
  }

  return (
    <div className="feed-page">
      <div className="feed-container">
        <div className="feed-header">
          <h1>My Feed</h1>
          <p>Latest posts from your events</p>
        </div>

        {error && <div className="error-message">{error}</div>}

        <div className="posts-grid">
          {posts.length > 0 ? (
            posts.map(post => (
              <PostCard key={post.id} post={post} />
            ))
          ) : (
            <div className="no-posts">
              <p>📸 No posts yet</p>
              <p>Start sharing your memories!</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

// Mock data
const mockPosts = [
  {
    id: '1',
    userId: 'user1',
    userName: 'John Doe',
    userAvatar: 'https://ui-avatars.com/api/?name=John+Doe&background=1890ff&color=fff',
    eventName: 'Summer Vacation 2024',
    caption: 'Beautiful sunset at the beach! 🌅',
    mediaUrls: ['https://images.unsplash.com/photo-1507525428034-b723cf961d3e'],
    createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000),
    likes: 42,
    comments: 8
  },
  {
    id: '2',
    userId: 'user2',
    userName: 'Jane Smith',
    userAvatar: 'https://ui-avatars.com/api/?name=Jane+Smith&background=1890ff&color=fff',
    eventName: 'Beach Party',
    caption: 'Amazing time with friends! 👯‍♀️',
    mediaUrls: ['https://images.unsplash.com/photo-1519904981063-b0cf448d479e'],
    createdAt: new Date(Date.now() - 4 * 60 * 60 * 1000),
    likes: 128,
    comments: 25
  }
];

export default FeedPage;
