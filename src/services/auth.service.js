import api from '../config/api';

export const authService = {
  async login(credentials) {
    const response = await api.post('/Auth/login', credentials);
    return response.data;
  },

  async register(userData) {
    const response = await api.post('/Auth/register', userData);
    return response.data;
  },

  async forgotPassword(email) {
    const response = await api.post('/Auth/forgot-password', { email });
    return response.data;
  },

  async resetPassword(token, newPassword) {
    const response = await api.post('/Auth/reset-password', { token, newPassword });
    return response.data;
  },

  logout() {
    localStorage.removeItem('token');
  }
};