import { createContext, useContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import api from "../config/api";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        api.defaults.headers.common["Authorization"] = `Bearer ${token}`;

        // Extrae los roles de los claim del token
        const roles =
          decodedToken[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ] || [];

        setUser({
          token,
          roles: Array.isArray(roles) ? roles : [roles],
          email:
            decodedToken[
              "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
            ],
          name: decodedToken[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
          ],
          username:
            decodedToken[
              "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
            ],
        });
      } catch (error) {
        console.error("Error decoding token:", error);
        localStorage.removeItem("token");
      }
    }
    setLoading(false);
  }, []);

  const hasRole = (role) => {
    return user?.roles?.includes(role) || false;
  };

  const login = async (credentials) => {
    const response = await api.post("/Auth/login", credentials);
    const { token } = response.data;
    const decodedToken = jwtDecode(token);

    // Extract roles from token claims
    const roles =
      decodedToken[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
      ] || [];

    localStorage.setItem("token", token);
    api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
    setUser({
      token,
      roles: Array.isArray(roles) ? roles : [roles],
      email:
        decodedToken[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
        ],
      name: decodedToken[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
      ],
      username:
        decodedToken[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        ],
    });
    return response.data;
  };

  const register = async (userData) => {
    const response = await api.post("/Auth/register", userData);
    return response.data;
  };

  const logout = () => {
    localStorage.removeItem("token");
    delete api.defaults.headers.common["Authorization"];
    setUser(null);
  };

  const forgotPassword = async (email) => {
    const response = await api.post("/Auth/forgot-password", { email });
    return response.data;
  };

  const resetPassword = async (token, newPassword) => {
    const response = await api.post("/Auth/reset-password", {
      token,
      newPassword,
    });
    return response.data;
  };

  // Función para editar los datos del perfil
  const editProfile = async (updatedData) => {
    try {
      const response = await api.put("/User/edit-profile", updatedData);
      // Actualiza el usuario en el contexto con los nuevos datos
      setUser({
        ...user,
        ...updatedData,
      });
      return response.data;
    } catch (error) {
      console.error("Error updating profile:", error);
      throw error;
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        login,
        register,
        logout,
        forgotPassword,
        resetPassword,
        hasRole,
        editProfile,
      }}
    >
      {!loading && children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
