import { useSearchParams } from "react-router-dom";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";
import toast from "react-hot-toast";
import { useAuth } from "../context/AuthContext";
import { useState } from "react";
import fondo from "../components/Image/Imagen.jpg";

const resetPasswordSchema = Yup.object().shape({
  newPassword: Yup.string()
    .min(6, "La contraseña debe tener al menos 6 caracteres")
    .required("La nueva contraseña es requerida"),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref("newPassword"), null], "Las contraseñas deben coincidir")
    .required("Confirma tu contraseña"),
});

export default function ResetPassword() {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token");
  const { resetPassword } = useAuth();
  const [submitted, setSubmitted] = useState(false);

  if (!token) {
    return (
      <div className="flex min-h-[calc(100vh-4rem)] items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900">Enlace inválido</h2>
          <p className="mt-2 text-gray-600">
            El enlace para restablecer la contraseña es inválido o ha expirado.
          </p>
        </div>
      </div>
    );
  }

  const handleSubmit = async (values, { setSubmitting }) => {
    try {
      await resetPassword(token, values.newPassword);
      toast.success(
        "Contraseña actualizada exitosamente. Puedes cerrar esta página.",
        {
          position: "top-center",
        }
      );
      setSubmitted(true); // Cambiar el estado a "enviado"
    } catch (error) {
      toast.error(
        error.response?.data?.message || "Error al restablecer la contraseña",
        {
          position: "top-center",
        }
      );
    } finally {
      setSubmitting(false);
    }
  };

  if (submitted) {
    return (
      <div className="flex min-h-[calc(100vh-4rem)] items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
        <div className="w-full max-w-md space-y-8 text-center">
          <h2 className="text-2xl font-bold text-gray-900">
            Contraseña restablecida
          </h2>
          <p className="text-gray-600">
            Tu contraseña ha sido restablecida. Ahora puedes cerrar esta página.
          </p>
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
      <div className="bg-white/80 px-8 py-8 rounded-lg max-w-sm text-center shadow-lg backdrop-blur-sm">
        <div>
          <h2 className="mt-6 text-center text-3xl font-bold tracking-tight text-gray-900">
            Restablecer contraseña
          </h2>
        </div>
        <Formik
          initialValues={{ newPassword: "", confirmPassword: "" }}
          validationSchema={resetPasswordSchema}
          onSubmit={handleSubmit}
        >
          {({ errors, touched, isSubmitting }) => (
            <Form className="mt-8 space-y-6">
              <div className="space-y-4">
                <div>
                  <label
                    htmlFor="newPassword"
                    className="block text-sm font-medium text-gray-700"
                  >
                    Nueva contraseña
                  </label>
                  <Field
                    id="newPassword"
                    name="newPassword"
                    type="password"
                    className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 sm:text-sm"
                  />
                  {errors.newPassword && touched.newPassword && (
                    <p className="mt-1 text-sm text-red-600">
                      {errors.newPassword}
                    </p>
                  )}
                </div>

                <div>
                  <label
                    htmlFor="confirmPassword"
                    className="block text-sm font-medium text-gray-700"
                  >
                    Confirmar contraseña
                  </label>
                  <Field
                    id="confirmPassword"
                    name="confirmPassword"
                    type="password"
                    className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 sm:text-sm"
                  />
                  {errors.confirmPassword && touched.confirmPassword && (
                    <p className="mt-1 text-sm text-red-600">
                      {errors.confirmPassword}
                    </p>
                  )}
                </div>
              </div>

              <div>
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="group relative flex w-full justify-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 disabled:bg-indigo-400"
                >
                  {isSubmitting ? "Actualizando..." : "Actualizar contraseña"}
                </button>
              </div>
            </Form>
          )}
        </Formik>
      </div>
    </div>
  );
}
