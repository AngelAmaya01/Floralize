import api from '../config/api';

export const roleService = {
  async getRoles() {
    const response = await api.get('/Role');
    return response.data;
  },

  async getUsersWithRoles() {
    const response = await api.get('/Role/users');
    return response.data;
  },

  async assignRole(userId, roleId) {
    const response = await api.post(`/Role/assign/${userId}`, roleId);
    return response.data;
  },

  async removeRole(userId, roleId) {
    const response = await api.delete(`/Role/remove/${userId}/${roleId}`);
    return response.data;
  }
};