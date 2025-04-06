import { useNavigate, Link } from "react-router-dom";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";
import toast from "react-hot-toast";
import { useAuth } from "../context/AuthContext";
import { useState } from "react";
import fondo from "../components/Image/Imagen.jpg";

// Esquema de validación para el registro
const registerSchema = Yup.object().shape({
  username: Yup.string()
    .required("El nombre de usuario es requerido")
    .min(5, "El nombre de usuario debe tener al menos 5 caracteres")
    .trim(),
  email: Yup.string()
    .email("Email inválido")
    .required("El email es requerido")
    .trim(),
  address: Yup.string()
    .required("La direccion es requerida")
    .min(3, "La dirreccion debe tener al menos 3 caracteres")
    .trim(),
  password: Yup.string()
    .min(6, "La contraseña debe tener al menos 6 caracteres")
    .required("La contraseña es requerida"),
  firstName: Yup.string().required("El nombre es requerido").trim(),
  lastName: Yup.string().required("El apellido es requerido").trim(),
  phoneNumber: Yup.string()
    .matches(/^\+?[\d\s-]{7,}$/, "Número de teléfono inválido")
    .trim(),
});

export default function Register() {
  const navigate = useNavigate();
  const { register } = useAuth(); // Asegúrate de que useAuth esté correctamente configurado
  const [termsAccepted, setTermsAccepted] = useState(false);

  const handleSubmit = async (values, { setSubmitting }) => {
    const loadingToast = toast.loading("Registrando...", {
      position: "top-center",
    });
    try {
      if (!termsAccepted) {
        throw new Error(
          "Debes aceptar los términos y condiciones para registrarte."
        );
      }

      // Llama a la función register del contexto de autenticación
      await register(values);
      toast.success("Registro exitoso", {
        id: loadingToast,
        position: "top-center",
      });
      navigate("/login"); // Redirige al usuario a la página de inicio de sesión
    } catch (error) {
      console.error(
        "Error en el registro:",
        error.response?.data || error.message
      );
      toast.error(error.response?.data?.message || error.message, {
        id: loadingToast,
        position: "top-center",
      });
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
      <div className="bg-white/80 px-16 py-5 rounded-lg max-w-sm text-center shadow-lg backdrop-blur-sm">
        <h1 className="text-gray-800 text-2xl font-bold mb-6">
          Crear una cuenta
        </h1>
        <Formik
          initialValues={{
            username: "",
            email: "",
            address: "",
            password: "",
            firstName: "",
            lastName: "",
            phoneNumber: "",
          }}
          validationSchema={registerSchema}
          onSubmit={handleSubmit}
        >
          {({ errors, touched, isSubmitting }) => (
            <Form>
              <div className="mb-6 text-left">
                <label htmlFor="username" className="block text-gray-800 mb-2">
                  Nombre de usuario:
                </label>
                <Field
                  id="username"
                  name="username"
                  type="text"
                  className="w-full px-3 py-2 border-b border-purple-600 bg-transparent focus:outline-none focus:border-purple-800 transition-colors"
                  disabled={isSubmitting}
                />
                {errors.username && touched.username && (
                  <p className="text-red-500 text-xs mt-1">{errors.username}</p>
                )}
              </div>

              <div className="mb-6 text-left">
                <label htmlFor="firstName" className="block text-gray-800 mb-2">
                  Nombre:
                </label>
                <Field
                  id="firstName"
                  name="firstName"
                  type="text"
                  className="w-full px-3 py-2 border-b border-purple-600 bg-transparent focus:outline-none focus:border-purple-800 transition-colors"
                  disabled={isSubmitting}
                />
                {errors.firstName && touched.firstName && (
                  <p className="text-red-500 text-xs mt-1">
                    {errors.firstName}
                  </p>
                )}
              </div>

              <div className="mb-6 text-left">
                <label htmlFor="lastName" className="block text-gray-800 mb-2">
                  Apellido:
                </label>
                <Field
                  id="lastName"
                  name="lastName"
                  type="text"
                  className="w-full px-3 py-2 border-b border-purple-600 bg-transparent focus:outline-none focus:border-purple-800 transition-colors"
                  disabled={isSubmitting}
                />
                {errors.lastName && touched.lastName && (
                  <p className="text-red-500 text-xs mt-1">{errors.lastName}</p>
                )}
              </div>

              <div className="mb-6 text-left">
                <label htmlFor="email" className="block text-gray-800 mb-2">
                  Email:
                </label>
                <Field
                  id="email"
                  name="email"
                  type="email"
                  className="w-full px-3 py-2 border-b border-purple-600 bg-transparent focus:outline-none focus:border-purple-800 transition-colors"
                  disabled={isSubmitting}
                />
                {errors.email && touched.email && (
                  <p className="text-red-500 text-xs mt-1">{errors.email}</p>
                )}
              </div>

              <div className="mb-6 text-left">
                <label htmlFor="address" className="block text-gray-800 mb-2">
                  Direccion:
                </label>
                <Field
                  id="address"
                  name="address"
                  type="address"
                  className="w-full px-3 py-2 border-b border-purple-600 bg-transparent focus:outline-none focus:border-purple-800 transition-colors"
                  disabled={isSubmitting}
                />
                {errors.address && touched.address && (
                  <p className="text-red-500 text-xs mt-1">{errors.address}</p>
                )}
              </div>

              <div className="mb-6 text-left">
                <label htmlFor="password" className="block text-gray-800 mb-2">
                  Contraseña:
                </label>
                <Field
                  id="password"
                  name="password"
                  type="password"
                  className="w-full px-3 py-2 border-b border-purple-600 bg-transparent focus:outline-none focus:border-purple-800 transition-colors"
                  disabled={isSubmitting}
                />
                {errors.password && touched.password && (
                  <p className="text-red-500 text-xs mt-1">{errors.password}</p>
                )}
              </div>

              <div className="mb-6 text-left">
                <label
                  htmlFor="phoneNumber"
                  className="block text-gray-800 mb-2"
                >
                  Teléfono:
                </label>
                <Field
                  id="phoneNumber"
                  name="phoneNumber"
                  type="tel"
                  className="w-full px-3 py-2 border-b border-purple-600 bg-transparent focus:outline-none focus:border-purple-800 transition-colors"
                  disabled={isSubmitting}
                />
                {errors.phoneNumber && touched.phoneNumber && (
                  <p className="text-red-500 text-xs mt-1">
                    {errors.phoneNumber}
                  </p>
                )}
              </div>

              <div className="mb-6 text-left">
                <div className="flex items-start">
                  <div className="flex h-6 items-center">
                    <input
                      id="terms"
                      type="checkbox"
                      checked={termsAccepted}
                      onChange={(e) => setTermsAccepted(e.target.checked)}
                      className="h-4 w-4 rounded border-purple-600 text-purple-600 focus:ring-purple-600"
                    />
                  </div>
                  <div className="ml-3">
                    <label htmlFor="terms" className="text-sm text-gray-600">
                      He leído y acepto los{" "}
                      <Link
                        to="/terms"
                        className="text-purple-600 hover:text-purple-700"
                      >
                        términos y condiciones
                      </Link>{" "}
                      y la{" "}
                      <Link
                        to="/privacy"
                        className="text-purple-600 hover:text-purple-700"
                      >
                        política de privacidad
                      </Link>
                    </label>
                  </div>
                </div>
              </div>

              <button
                type="submit"
                className="w-full bg-purple-600 text-white py-3 rounded-lg mt-4 hover:bg-purple-700 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
                disabled={isSubmitting}
              >
                {isSubmitting ? "Registrando..." : "Registrarse"}
              </button>

              <div className="mt-6 text-center">
                <span className="text-gray-600 text-sm">
                  ¿Ya tienes una cuenta?{" "}
                </span>
                <Link
                  to="/login"
                  className="text-purple-600 text-sm hover:text-purple-700 transition-colors"
                >
                  Inicia sesión
                </Link>
              </div>
            </Form>
          )}
        </Formik>
      </div>
    </div>
  );
}
