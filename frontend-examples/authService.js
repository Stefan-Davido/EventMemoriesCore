/**
 * Authentication Service - Example Implementation
 * Works with ASP.NET Core AuthController
 * Supports: Login, Register, Logout, Token Refresh
 */

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7001';
const AUTH_ENDPOINTS = {
  LOGIN: `${API_BASE_URL}/api/auth/login`,
  REGISTER: `${API_BASE_URL}/api/auth/register`,
  LOGOUT: `${API_BASE_URL}/api/auth/logout`,
  REFRESH: `${API_BASE_URL}/api/auth/refresh-token`,
  VERIFY: `${API_BASE_URL}/api/auth/verify`,
};

// Token storage keys
const TOKEN_KEY = 'access_token';
const REFRESH_TOKEN_KEY = 'refresh_token';
const USER_KEY = 'user_info';

/**
 * User Registration
 */
export const register = async (userData) => {
  try {
    const response = await fetch(AUTH_ENDPOINTS.REGISTER, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(userData),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Registration failed');
    }

    const data = await response.json();

    // Store tokens if provided
    if (data.accessToken) {
      localStorage.setItem(TOKEN_KEY, data.accessToken);
    }
    if (data.refreshToken) {
      localStorage.setItem(REFRESH_TOKEN_KEY, data.refreshToken);
    }
    if (data.user) {
      localStorage.setItem(USER_KEY, JSON.stringify(data.user));
    }

    return data;
  } catch (error) {
    console.error('Registration error:', error);
    throw error;
  }
};

/**
 * User Login
 */
export const login = async (email, password) => {
  try {
    const response = await fetch(AUTH_ENDPOINTS.LOGIN, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Login failed');
    }

    const data = await response.json();

    // Store tokens and user info
    if (data.accessToken) {
      localStorage.setItem(TOKEN_KEY, data.accessToken);
    }
    if (data.refreshToken) {
      localStorage.setItem(REFRESH_TOKEN_KEY, data.refreshToken);
    }
    if (data.user) {
      localStorage.setItem(USER_KEY, JSON.stringify(data.user));
    }

    return data;
  } catch (error) {
    console.error('Login error:', error);
    throw error;
  }
};

/**
 * User Logout
 */
export const logout = async () => {
  try {
    const token = localStorage.getItem(TOKEN_KEY);

    if (token) {
      await fetch(AUTH_ENDPOINTS.LOGOUT, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      }).catch(() => {
        // Logout from server failed but continue clearing local data
      });
    }

    // Clear local storage
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  } catch (error) {
    console.error('Logout error:', error);
    throw error;
  }
};

/**
 * Refresh Access Token
 */
export const refreshAccessToken = async () => {
  try {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);

    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    const response = await fetch(AUTH_ENDPOINTS.REFRESH, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ refreshToken }),
    });

    if (!response.ok) {
      throw new Error('Token refresh failed');
    }

    const data = await response.json();

    if (data.accessToken) {
      localStorage.setItem(TOKEN_KEY, data.accessToken);
    }
    if (data.refreshToken) {
      localStorage.setItem(REFRESH_TOKEN_KEY, data.refreshToken);
    }

    return data;
  } catch (error) {
    console.error('Token refresh error:', error);
    // Clear tokens if refresh fails
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    throw error;
  }
};

/**
 * Get Current User
 */
export const getCurrentUser = () => {
  const userInfo = localStorage.getItem(USER_KEY);
  return userInfo ? JSON.parse(userInfo) : null;
};

/**
 * Get Access Token
 */
export const getAccessToken = () => {
  return localStorage.getItem(TOKEN_KEY);
};

/**
 * Check if user is authenticated
 */
export const isAuthenticated = () => {
  return !!localStorage.getItem(TOKEN_KEY);
};

/**
 * Verify token is valid
 */
export const verifyToken = async () => {
  try {
    const token = localStorage.getItem(TOKEN_KEY);

    if (!token) {
      return false;
    }

    const response = await fetch(AUTH_ENDPOINTS.VERIFY, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    return response.ok;
  } catch (error) {
    console.error('Token verification error:', error);
    return false;
  }
};

/**
 * Authenticated API Call Helper
 */
export const authenticatedFetch = async (url, options = {}) => {
  const token = localStorage.getItem(TOKEN_KEY);

  if (!token) {
    throw new Error('User not authenticated');
  }

  const headers = {
    'Authorization': `Bearer ${token}`,
    ...options.headers,
  };

  try {
    let response = await fetch(url, {
      ...options,
      headers,
    });

    // If 401, try to refresh token
    if (response.status === 401) {
      try {
        await refreshAccessToken();
        const newToken = localStorage.getItem(TOKEN_KEY);
        headers['Authorization'] = `Bearer ${newToken}`;

        response = await fetch(url, {
          ...options,
          headers,
        });
      } catch (refreshError) {
        throw new Error('Session expired. Please login again.');
      }
    }

    return response;
  } catch (error) {
    console.error('Authenticated fetch error:', error);
    throw error;
  }
};
