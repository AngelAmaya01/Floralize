import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";
import toast from "react-hot-toast";
import { EyeIcon, EyeSlashIcon } from "@heroicons/react/24/outline";
import { useAuth } from "../context/AuthContext";
import fondo from "../components/Image/Imagen.jpg";
// import PetalosBackground from "../../styles/PetalosBackground";

// Esquema de validación para el inicio de sesión
const loginSchema = Yup.object().shape({
  username: Yup.string().required("El nombre de usuario es requerido").trim(),
  password: Yup.string()
    .required("La contraseña es requerida")
    .min(6, "La contraseña debe tener al menos 6 caracteres"),
});

export default function Login() {
  const navigate = useNavigate();
  const { login, loading } = useAuth(); // Asegúrate de que useAuth esté correctamente configurado
  const [showPassword, setShowPassword] = useState(false);

  const handleSubmit = async (values, { setSubmitting, setFieldError }) => {
    const loadingToast = toast.loading("Iniciando sesión...", {
      position: "top-center",
    });
    try {
      // Llama a la función login del contexto de autenticación
      const response = await login(values);
      if (response && response.token) {
        localStorage.setItem("token", response.token); // Guarda el token en localStorage
        toast.success("¡Bienvenido de vuelta!", {
          id: loadingToast,
          position: "top-center",
          icon: "✅",
        });
        navigate("/", { replace: true }); // Redirige al usuario a la página principal
      }
    } catch (error) {
      const errorMessage =
        error.response?.data?.message || "Error al iniciar sesión";

      if (errorMessage.toLowerCase().includes("credenciales")) {
        setFieldError("username", "Credenciales incorrectas");
        setFieldError("password", "Credenciales incorrectas");
      } else if (errorMessage.toLowerCase().includes("usuario")) {
        setFieldError("username", errorMessage);
      } else if (errorMessage.toLowerCase().includes("contraseña")) {
        setFieldError("password", errorMessage);
      } else {
        toast.error(errorMessage, {
          id: loadingToast,
          position: "top-center",
        });
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="relative min-h-screen flex justify-center items-center overflow-hidden">
      <div
        className="absolute inset-0 w-full h-full bg-cover bg-center bg-no-repeat"
        style={{ backgroundImage: `url(${fondo})` }}
      ></div>
      {/* Formulario de inicio de sesión */}
      <div className="bg-white/80 px-16 py-5 rounded-lg max-w-sm text-center shadow-lg backdrop-blur-sm">
        {/* <PetalosBackground /> */}
        <h1 className="text-gray-800 text-2xl font-bold mb-6">
          Iniciar Sesión
        </h1>
        <Formik
          initialValues={{ username: "", password: "" }}
          validationSchema={loginSchema}
          onSubmit={handleSubmit}
        >
          {({ errors, touched, isSubmitting, values }) => (
            <Form>
              <div className="mb-6 text-left">
                <label htmlFor="username" className="block text-gray-700 mb-1">
                  Usuario:
                </label>
                <Field
                  id="username"
                  name="username"
                  type="text"
                  className="w-full px-3 py-2 border-b border-purple-600 bg-transparent focus:outline-none focus:border-purple-800 transition-colors"
                  disabled={isSubmitting || loading}
                />
                {errors.username && touched.username && (
                  <p className="text-red-500 text-xs mt-1">{errors.username}</p>
                )}
              </div>

              <div className="mb-6 text-left">
                <label htmlFor="password" className="block text-gray-700 mb-1">
                  Contraseña:
                </label>
                <div className="relative">
                  <Field
                    id="password"
                    name="password"
                    type={showPassword ? "text" : "password"}
                    className="w-full px-3 py-2 border-b border-purple-600 bg-transparent focus:outline-none focus:border-purple-800 transition-colors"
                    disabled={isSubmitting || loading}
                  />
                  <button
                    type="button"
                    className="absolute right-3 top-1/2 transform -translate-y-1/2 bg-transparent border-none cursor-pointer"
                    onClick={() => setShowPassword(!showPassword)}
                  >
                    {showPassword ? (
                      <EyeSlashIcon className="w-5 h-5 text-purple-600" />
                    ) : (
                      <EyeIcon className="w-5 h-5 text-purple-600" />
                    )}
                  </button>
                </div>
                {errors.password && touched.password && (
                  <p className="text-red-500 text-xs mt-1">{errors.password}</p>
                )}
              </div>

              <button
                type="submit"
                className="w-full bg-purple-600 text-white py-3 rounded-lg mt-4 hover:bg-purple-700 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
                disabled={
                  isSubmitting ||
                  loading ||
                  !values.username ||
                  !values.password
                }
              >
                {isSubmitting || loading
                  ? "Iniciando sesión..."
                  : "Iniciar Sesión"}
              </button>
            </Form>
          )}
        </Formik>

        <div className="mt-6">
          <Link
            to="/forgot-password"
            className="text-purple-600 text-sm hover:text-purple-700 transition-colors"
          >
            ¿Olvidaste tu contraseña?
          </Link>
          <br />
          <span className="text-gray-600 text-sm">¿No tienes una cuenta? </span>
          <Link
            to="/register"
            className="text-purple-600 text-sm hover:text-purple-700 transition-colors"
          >
            Regístrate
          </Link>
        </div>
      </div>
    </div>
  );
}
