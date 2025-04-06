import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";
import toast from "react-hot-toast";
import { useAuth } from "../context/AuthContext";
import fondo from "../components/Image/Imagen.jpg";

const forgotPasswordSchema = Yup.object().shape({
  email: Yup.string()
    .email("Email inválido")
    .required("El email es requerido")
    .trim(),
});

export default function ForgotPassword() {
  const [submitted, setSubmitted] = useState(false);
  const { forgotPassword } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (values, { setSubmitting }) => {
    const loadingToast = toast.loading("Enviando instrucciones...", {
      position: "top-center",
    });
    try {
      await forgotPassword(values.email);
      setSubmitted(true);
      toast.success("Se han enviado las instrucciones a tu correo", {
        id: loadingToast,
        position: "top-center",
      });
    } catch (error) {
      toast.error(
        error.response?.data?.message || "Error al procesar la solicitud",
        {
          id: loadingToast,
          position: "top-center",
        }
      );
    } finally {
      setSubmitting(false);
    }
  };

  if (submitted) {
    return (
      <div className="relative min-h-screen flex justify-center items-center overflow-hidden">
        <div
          className="absolute inset-0 w-full h-full bg-cover bg-center bg-no-repeat"
          style={{ backgroundImage: `url(${fondo})` }}
        ></div>
        <div className="bg-white/80 px-16 py-5 rounded-lg max-w-sm text-center shadow-lg backdrop-blur-sm">
          <h2 className="text-gray-800 text-2xl font-bold mb-6">
            Revisa tu correo
          </h2>
          <p className="text-gray-600 mb-6">
            Si existe una cuenta asociada a este correo, recibirás las
            instrucciones para restablecer tu contraseña.
          </p>
          <button
            onClick={() => navigate("/login")}
            className="w-full bg-purple-600 text-white py-3 rounded-lg mt-4 hover:bg-purple-700 transition-colors"
          >
            Volver al inicio de sesión
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="relative min-h-screen flex justify-center items-center overflow-hidden">
      <div
        className="absolute inset-0 w-full h-full bg-cover bg-center bg-no-repeat"
        style={{ backgroundImage: `url(${fondo})` }}
      ></div>
      <div className="bg-white bg-opacity-90 p-10 rounded-lg shadow-lg w-full max-w-md text-center z-10">
        <h1 className="text-2xl font-bold text-gray-800 mb-8">
          Recuperar contraseña
        </h1>
        <Formik
          initialValues={{ email: "" }}
          validationSchema={forgotPasswordSchema}
          onSubmit={handleSubmit}
        >
          {({ errors, touched, isSubmitting }) => (
            <Form>
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

              <button
                type="submit"
                className="w-full bg-purple-600 text-white py-3 rounded-lg mt-4 hover:bg-purple-700 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
                disabled={isSubmitting}
              >
                {isSubmitting ? "Enviando..." : "Enviar instrucciones"}
              </button>
            </Form>
          )}
        </Formik>

        <div className="mt-6">
          <Link
            to="/login"
            className="text-purple-600 text-sm hover:text-purple-700 transition-colors"
          >
            Volver al inicio de sesión
          </Link>
        </div>
      </div>
    </div>
  );
}
