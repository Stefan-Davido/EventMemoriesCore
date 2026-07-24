import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7000/api';

// Create axios instance
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle response errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      localStorage.removeItem('user');
      window.location.href = '/auth/login';
    }
    return Promise.reject(error);
  }
);

// Auth Service
export const authService = {
  login: (email, password) =>
    apiClient.post('/Auth/login', { email, password, rememberMe: true }),

  register: (userName, email, password) =>
    apiClient.post('/Auth/register', { userName, email, password }),

  getCurrentUser: () =>
    apiClient.get(`/user/${localStorage.getItem('userId')}`),
};

// Tenant Service
export const tenantService = {
  getAll: () => apiClient.get('/tenant'),

  getById: (id) => apiClient.get(`/tenant/${id}`),

  getByOwner: (ownerId) => apiClient.get(`/tenant/owner/${ownerId}`),

  create: (data) => apiClient.post('/tenant', data),

  update: (id, data) => apiClient.put(`/tenant/${id}`, data),

  delete: (id) => apiClient.delete(`/tenant/${id}`),
};

// Event Service
export const eventService = {
  getAll: () => apiClient.get('/event'),

  getById: (id) => apiClient.get(`/event/${id}`),

  getByTenant: (tenantId) => apiClient.get(`/event/tenant/${tenantId}`),

  getByOwner: (ownerId) => apiClient.get(`/event/owner/${ownerId}`),

  create: (data) => apiClient.post('/event', data),

  update: (id, data) => apiClient.put(`/event/${id}`, data),

  delete: (id) => apiClient.delete(`/event/${id}`),
};

// Post Service
export const postService = {
  getAll: () => apiClient.get('/post'),

  getById: (id) => apiClient.get(`/post/${id}`),

  getByEvent: (eventId) => apiClient.get(`/post/event/${eventId}`),

  getByUser: (userId) => apiClient.get(`/post/user/${userId}`),

  create: (data) => apiClient.post('/post', data, {
      headers: { 'Content-Type': 'multipart/form-data' }
  }),

  update: (id, data) => apiClient.put(`/post/${id}`, data),

  delete: (id) => apiClient.delete(`/post/${id}`),
};

// Info Service
export const infoService = {
  getAll: () => apiClient.get('/info'),

  getById: (id) => apiClient.get(`/info/${id}`),

  getByEvent: (eventId) => apiClient.get(`/info/event/${eventId}`),

  getByUser: (userId) => apiClient.get(`/info/user/${userId}`),

  getByLevel: (level) => apiClient.get(`/info/level/${level}`),

  create: (data) => apiClient.post('/info', data),

  update: (id, data) => apiClient.put(`/info/${id}`, data),

  delete: (id) => apiClient.delete(`/info/${id}`),
};

// Configuration Service
export const configService = {
  getAll: () => apiClient.get('/configuration'),

  getById: (id) => apiClient.get(`/configuration/${id}`),

  getByEvent: (eventId) => apiClient.get(`/configuration/event/${eventId}`),

  getByName: (eventId, name) =>
    apiClient.get(`/configuration/event/${eventId}/name/${name}`),

  create: (data) => apiClient.post('/configuration', data),

  update: (id, data) => apiClient.put(`/configuration/${id}`, data),

  delete: (id) => apiClient.delete(`/configuration/${id}`),
};

// User Service
export const userService = {
  getAll: () => apiClient.get('/user'),

  getById: (id) => apiClient.get(`/user/${id}`),

  getByEmail: (email) => apiClient.get(`/user/email/${email}`),

  update: (id, data) => apiClient.put(`/user/${id}`, data),

  delete: (id) => apiClient.delete(`/user/${id}`),
};

export default apiClient;
