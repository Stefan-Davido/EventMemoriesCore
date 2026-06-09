# EventMemories Frontend

A modern React application for sharing memories and photos from your events.

## Features

- 🔐 **Authentication** - Secure login and registration with JWT tokens
- 👤 **User Profile** - Manage your profile information and settings
- 📸 **Create Posts** - Upload photos and videos with captions (max 10 files per post)
- 🎉 **Event Management** - Create and manage multiple events
- 📰 **Feed** - View posts from your events and community
- 🔔 **Notifications** - Get notified about new posts and scheduled events
- 🎨 **Modern UI** - Light blue and white theme with responsive design

## Tech Stack

- **React 18** - UI library
- **React Router v6** - Client-side routing
- **Axios** - HTTP client for API calls
- **CSS3** - Styling with custom CSS
- **Ant Design Icons** - Icon library

## Project Structure

```
frontend/
├── public/
│   └── index.html
├── src/
│   ├── components/        # Reusable components
│   │   ├── Header.js
│   │   ├── Sidebar.js
│   │   └── PostCard.js
│   ├── pages/            # Page components
│   │   ├── AuthPages/
│   │   │   ├── LoginPage.js
│   │   │   └── RegisterPage.js
│   │   ├── FeedPage.js
│   │   ├── ProfilePage.js
│   │   ├── EventPage.js
│   │   ├── CreatePostPage.js
│   │   └── NotificationsPage.js
│   ├── layouts/          # Layout components
│   │   ├── AuthLayout.js
│   │   └── MainLayout.js
│   ├── services/         # API services
│   │   └── apiService.js
│   ├── App.js
│   ├── App.css
│   ├── index.js
│   └── index.css
├── package.json
└── README.md
```

## Getting Started

### Prerequisites

- Node.js 16+ 
- npm or yarn

### Installation

1. Navigate to the frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Create a `.env` file (optional):
```env
REACT_APP_API_URL=https://localhost:7000/api
```

### Running the Application

Development mode:
```bash
npm start
```

The app will open in your browser at `http://localhost:3000`

### Building for Production

```bash
npm run build
```

## Components Overview

### Authentication
- **LoginPage** - User login with email and password
- **RegisterPage** - New user registration

### Main Pages
- **FeedPage** - Display all posts from events
- **ProfilePage** - User profile with edit functionality
- **EventPage** - View event details and associated posts
- **CreatePostPage** - Create new posts with media upload
- **NotificationsPage** - View and manage notifications

### Reusable Components
- **Header** - Top navigation bar with user menu
- **Sidebar** - Navigation menu with active route indication
- **PostCard** - Card component for displaying individual posts

## API Integration

The frontend communicates with the backend API endpoints:

### Authentication
- `POST /api/user` - Login/Register

### Tenants
- `GET/POST /api/tenant` - Get/Create tenants
- `GET /api/tenant/{id}` - Get specific tenant
- `GET /api/tenant/owner/{ownerId}` - Get user's tenants

### Events
- `GET/POST /api/event` - Get/Create events
- `GET /api/event/tenant/{tenantId}` - Get tenant's events
- `GET /api/event/owner/{ownerId}` - Get user's events

### Posts
- `GET/POST /api/post` - Get/Create posts
- `GET /api/post/event/{eventId}` - Get event's posts
- `GET /api/post/user/{userId}` - Get user's posts

### Notifications
- `GET/POST /api/info` - Get/Create notifications
- `GET /api/info/event/{eventId}` - Get event's notifications

## Styling

The application uses a consistent color scheme:

- Primary Blue: `#1890ff`
- Light Blue: `#e6f7ff`
- Lighter Blue: `#f0f5ff`
- White: `#ffffff`
- Light Gray: `#f5f5f5`

All components follow the defined design system in `src/index.css`

## Key Features Implementation

### Authentication Flow
1. User enters credentials on login/register page
2. Credentials sent to backend API
3. JWT token received and stored in localStorage
4. Token automatically added to all API requests
5. On token expiration or 401 response, user redirected to login

### Post Creation
- Select event for the post
- Write caption (max 1000 chars)
- Upload up to 10 media files (images/videos)
- Preview media before publishing
- Publish post to API

### Feed Display
- Loads posts from all user's events
- Displays user avatar, name, event name
- Shows media carousel if multiple files
- Like and comment functionality
- Delete post option

## Error Handling

- API errors display user-friendly messages
- Fallback to mock data for demo purposes
- Network error handling with retry options
- Form validation before submission

## Security Features

- JWT token stored securely in localStorage
- Token automatically added to Authorization header
- 401 responses trigger automatic logout
- HTTPS for API communication

## Performance Optimizations

- Lazy loading of images
- Component memoization where applicable
- Efficient re-rendering with proper key usage
- CSS transitions for smooth animations

## Browser Support

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)

## Future Enhancements

- [ ] Real-time notifications with WebSockets
- [ ] Image cropping and filters
- [ ] Dark mode theme toggle
- [ ] Direct messaging between users
- [ ] Event analytics and statistics
- [ ] Advanced search and filtering
- [ ] Comment threads
- [ ] Photo gallery view
- [ ] User mentions and tags
- [ ] Share to social media

## Troubleshooting

### API Connection Issues
- Ensure backend is running on `https://localhost:7000`
- Check `REACT_APP_API_URL` in `.env`
- Verify CORS configuration on backend

### Authentication Issues
- Clear browser localStorage if stuck on login
- Check that JWT token is being stored correctly
- Verify token format in Authorization header

### Media Upload Issues
- Check file size (max 100MB recommended)
- Verify supported formats (JPG, PNG, GIF, MP4)
- Ensure 10 file limit is respected

## Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Commit changes: `git commit -am 'Add your feature'`
3. Push to branch: `git push origin feature/your-feature`
4. Submit a pull request

## License

MIT License - See LICENSE file for details

## Support

For issues or questions, please create an issue in the GitHub repository.

---

**Happy coding!** 🎉
