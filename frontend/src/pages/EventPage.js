import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { eventService } from '../services/apiService';
import PostCard from '../components/PostCard';
import './EventPage.css';

function EventPage() {
  const { eventId } = useParams();
  const [event, setEvent] = useState(null);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchEventAndPosts();
  }, [eventId]);

  const fetchEventAndPosts = async () => {
    try {
      const eventResponse = await eventService.getById(eventId);
      setEvent(eventResponse.data);
      // Fetch posts for this event
      setPosts(mockPosts);
    } catch (err) {
      // Use mock data
      setEvent(mockEvent);
      setPosts(mockPosts);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="spinner"></div>;
  }

  return (
    <div className="event-page">
      <div className="event-header">
        <div className="event-cover">
          <img 
            src="https://images.unsplash.com/photo-1492684223066-81342ee5ff30?w=1200&h=400&fit=crop"
            alt={event?.name}
          />
        </div>
        <div className="event-info">
          <h1>{event?.name}</h1>
          <p className="event-description">{event?.description}</p>
          <div className="event-meta">
            <span>📅 {new Date(event?.eventDate).toLocaleDateString()}</span>
            {event?.eventDateEnd && (
              <span>to {new Date(event?.eventDateEnd).toLocaleDateString()}</span>
            )}
            <span>👥 {posts.length} posts</span>
          </div>
        </div>
      </div>

      <div className="event-content">
        <div className="posts-section">
          <h2>Event Posts</h2>
          <div className="posts-grid">
            {posts.length > 0 ? (
              posts.map(post => (
                <PostCard key={post.id} post={post} />
              ))
            ) : (
              <div className="no-posts">
                <p>No posts yet for this event</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

const mockEvent = {
  id: '1',
  name: 'Summer Vacation 2024',
  description: 'A wonderful summer trip with family and friends',
  eventDate: new Date(),
  eventDateEnd: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
};

const mockPosts = [
  {
    id: '1',
    userName: 'John Doe',
    caption: 'Beautiful sunset at the beach! 🌅',
    mediaUrls: ['https://images.unsplash.com/photo-1507525428034-b723cf961d3e'],
    likes: 42,
    comments: 8
  }
];

export default EventPage;
