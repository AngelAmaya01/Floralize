import { useState, useEffect, useRef } from "react";
import { GoPerson } from "react-icons/go";
import { useAuth } from "../context/AuthContext";

const Header = () => {
  const { user, hasRole, logout } = useAuth();
  const isAdmin = hasRole("Admin");
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef(null);

  // Log para ver la información del usuario
  useEffect(() => {
    if (user) console.log("Usuario del backend:", user);
  }, [user]);

  // Cierra el menú si se hace clic fuera de él
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (menuRef.current && !menuRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  return (
    <header className="bg-[#9a5ea7] p-4 top-2  flex items-center justify-between w-full rounded-lg shadow-lg relative">
      <h1 className="text-white text-4xl font-bold font-dancing">Floralize</h1>

      <div className="relative" ref={menuRef}>
        {user ? (
          <div className="flex items-center space-x-4">
            <div className="flex flex-col items-end">
              <span className="text-white font-semibold text-lg">
                {user.username || user.name || "Usuario"}
              </span>
              {isAdmin && (
                <span className="text-yellow-300 text-sm font-bold">
                  Administrador
                </span>
              )}
            </div>

            {/* Avatar o ícono de usuario */}
            <button
              onClick={() => setIsOpen(!isOpen)}
              className="focus:outline-none"
            >
              {user.avatar ? (
                <img
                  src={user.avatar}
                  alt="Avatar"
                  className="w-12 h-12 rounded-full border-2 border-white shadow-md"
                />
              ) : (
                <GoPerson className="text-white text-4xl transition-colors duration-300 hover:text-yellow-300 cursor-pointer" />
              )}
            </button>
          </div>
        ) : (
          <a href="/login">
            <GoPerson className="text-white text-4xl transition-colors duration-300 hover:text-yellow-300 cursor-pointer" />
          </a>
        )}

        {/* Menú de perfil */}
        {isOpen && user && (
          <div className="absolute right-0 mt-3 w-56 bg-white shadow-xl rounded-lg py-4 px-6 z-50 animate-fadeIn">
            <div className="flex items-center space-x-3">
              {user.avatar ? (
                <img
                  src={user.avatar}
                  alt="Avatar"
                  className="w-14 h-14 rounded-full border border-gray-300"
                />
              ) : (
                <GoPerson className="text-gray-500 text-4xl" />
              )}
              <div>
                <p className="text-gray-800 font-semibold text-lg">
                  {user.username || user.name}
                </p>
                <p className="text-gray-500 text-sm">{user.email}</p>
              </div>
            </div>

            <hr className="my-3" />

            <a
              href="/profile"
              className="block text-sm text-gray-700 hover:bg-gray-100 p-2 rounded-md transition"
            >
              Ver perfil
            </a>
            <button
              onClick={logout}
              className="w-full text-left text-sm text-red-500 hover:bg-gray-100 p-2 rounded-md transition"
            >
              Cerrar sesión
            </button>
          </div>
        )}
      </div>
    </header>
  );
};

export default Header;
