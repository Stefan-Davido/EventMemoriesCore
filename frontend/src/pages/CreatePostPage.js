import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { FiImage, FiVideo, FiX } from 'react-icons/fi';
import { postService, eventService } from '../services/apiService';
import './CreatePostPage.css';

function CreatePostPage() {
  const navigate = useNavigate();
  const [events, setEvents] = React.useState([]);
  const [loading, setLoading] = React.useState(false);
  const [formData, setFormData] = useState({
    eventId: '',
    caption: '',
    mediaUrls: [],
  });
  const [mediaPreview, setMediaPreview] = useState([]);

  React.useEffect(() => {
    fetchEvents();
  }, []);

  const fetchEvents = async () => {
    try {
      const response = await eventService.getByOwner(localStorage.getItem('userId'));
      setEvents(response.data);
    } catch (err) {
      // Mock data
      setEvents([
        { id: '1', name: 'Summer Vacation 2024' },
        { id: '2', name: 'Beach Party' },
      ]);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleMediaUpload = (e) => {
    const files = e.target.files;
    const newUrls = [];
    const newPreviews = [];

    for (let i = 0; i < files.length && formData.mediaUrls.length + i < 10; i++) {
      const file = files[i];
      const reader = new FileReader();

      reader.onload = (event) => {
        newPreviews.push({
          id: Date.now() + i,
          url: event.target.result,
          type: file.type.startsWith('video') ? 'video' : 'image'
        });

        if (newPreviews.length === i + 1) {
          setMediaPreview(prev => [...prev, ...newPreviews]);
        }
      };

      reader.readAsDataURL(file);
      newUrls.push(file.name);
    }

    setFormData(prev => ({
      ...prev,
      mediaUrls: [...prev.mediaUrls, ...newUrls].slice(0, 10)
    }));
  };

  const removeMedia = (index) => {
    setMediaPreview(prev => prev.filter((_, i) => i !== index));
    setFormData(prev => ({
      ...prev,
      mediaUrls: prev.mediaUrls.filter((_, i) => i !== index)
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.eventId || !formData.caption.trim()) {
      alert('Please fill in all required fields');
      return;
    }

    setLoading(true);

    try {
      await postService.create({
        eventId: '2d54167d-7f81-4b45-a197-ab87852bad78', // mock event Id
        caption: formData.caption,
        mediaUrls: formData.mediaUrls
      });
      navigate('/');
    } catch (err) {
      alert('Failed to create post');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="create-post-page">
      <div className="create-post-container">
        <h1>Create a New Post</h1>
        <p>Share your special moments with your event community</p>

        <form onSubmit={handleSubmit} className="create-post-form">
          <div className="input-group">
            <label htmlFor="eventId">Select Event *</label>
            <select
              id="eventId"
              name="eventId"
              value={formData.eventId}
              onChange={handleChange}
              required
            >
              <option value="">Choose an event...</option>
              {events.map(event => (
                <option key={event.id} value={event.id}>
                  {event.name}
                </option>
              ))}
            </select>
          </div>

          <div className="input-group">
            <label htmlFor="caption">Caption *</label>
            <textarea
              id="caption"
              name="caption"
              placeholder="What's on your mind? Share your thoughts about this moment..."
              value={formData.caption}
              onChange={handleChange}
              rows="6"
              maxLength="1000"
              required
            />
            <p className="char-count">{formData.caption.length}/1000</p>
          </div>

          <div className="media-upload-section">
            <h3>Add Media (Max 10 files)</h3>

            {mediaPreview.length < 10 && (
              <div className="upload-area">
                <label className="upload-label">
                  <div className="upload-content">
                    <FiImage size={32} />
                    <FiVideo size={32} />
                    <p>Click to upload or drag and drop</p>
                    <p className="upload-hint">PNG, JPG, GIF, MP4 up to 100MB each</p>
                  </div>
                  <input
                    type="file"
                    multiple
                    accept="image/*,video/*"
                    onChange={handleMediaUpload}
                    style={{ display: 'none' }}
                  />
                </label>
              </div>
            )}

            {mediaPreview.length > 0 && (
              <div className="media-preview">
                <h4>Selected Media ({mediaPreview.length}/10)</h4>
                <div className="media-grid">
                  {mediaPreview.map((media, index) => (
                    <div key={media.id} className="media-item">
                      {media.type === 'image' ? (
                        <img src={media.url} alt="preview" />
                      ) : (
                        <video src={media.url} />
                      )}
                      <button
                        type="button"
                        className="remove-media-btn"
                        onClick={() => removeMedia(index)}
                      >
                        <FiX size={20} />
                      </button>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>

          <div className="form-actions">
            <button
              type="button"
              className="btn btn-secondary"
              onClick={() => navigate('/')}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading}
            >
              {loading ? 'Publishing...' : 'Publish Post'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default CreatePostPage;
