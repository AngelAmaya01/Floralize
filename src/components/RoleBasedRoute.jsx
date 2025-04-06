import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function RoleBasedRoute({ children, roles }) {
  const { user, hasRole } = useAuth();

  if (!user) {
    return <Navigate to="/login" />;
  }

  // Check if user has any of the required roles
  const hasRequiredRole = roles.some(role => hasRole(role));
  
  if (!hasRequiredRole) {
    return <Navigate to="/" />;
  }

  return children;
}