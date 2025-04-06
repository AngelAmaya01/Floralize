import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import packageJson from "../../package.json";

export default function TermsAndPrivacy() {
  const [accepted, setAccepted] = useState(false);
  const navigate = useNavigate();
  const { user } = useAuth();

  return (
    <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
      <div className="mb-8 flex items-center justify-between">
        <h1 className="text-3xl font-bold text-gray-900">
          Términos y Condiciones
        </h1>
        <span className="text-sm text-gray-500">
          Versión {packageJson.version}
        </span>
      </div>

      <div className="rounded-lg bg-white p-6 shadow">
        <div className="prose max-w-none">
          <h2 className="text-xl font-semibold text-gray-900">
            1. Términos del Servicio
          </h2>
          <p className="mt-4 text-gray-600">
            Al acceder y utilizar este sistema de gestión de compra e
            inventario, usted acepta cumplir y estar sujeto a los siguientes
            términos y condiciones de uso.
          </p>

          <h2 className="mt-8 text-xl font-semibold text-gray-900">
            2. Privacidad de los Datos
          </h2>
          <p className="mt-4 text-gray-600">
            Nos comprometemos a proteger su privacidad. La información personal
            recopilada se utilizará únicamente para:
          </p>
          <ul className="mt-2 list-disc pl-5 text-gray-600">
            <li>Gestionar sus pedidos</li>
            <li>Mantener un registro de su historial de compra</li>
            <li>Comunicarnos con usted sobre su seguimiento del pedido</li>
            <li>Enviar recordatorios de el estado de su pedido</li>
          </ul>

          <h2 className="mt-8 text-xl font-semibold text-gray-900">
            3. Uso de la Información
          </h2>
          <p className="mt-4 text-gray-600">
            La información proporcionada en este sistema será utilizada
            exclusivamente para fines administrativos relacionados con su
            atención.
          </p>

          <h2 className="mt-8 text-xl font-semibold text-gray-900">
            4. Seguridad
          </h2>
          <p className="mt-4 text-gray-600">
            Implementamos medidas de seguridad técnicas y organizativas para
            proteger sus datos personales contra accesos no autorizados, pérdida
            o alteración.
          </p>

          <h2 className="mt-8 text-xl font-semibold text-gray-900">
            5. Sus Derechos
          </h2>
          <p className="mt-4 text-gray-600">Usted tiene derecho a:</p>
          <ul className="mt-2 list-disc pl-5 text-gray-600">
            <li>Acceder a sus datos personales</li>
            <li>Solicitar la rectificación de datos inexactos</li>
            <li>Solicitar la eliminación de sus datos</li>
            <li>Oponerse al tratamiento de sus datos</li>
          </ul>
        </div>
      </div>

      <div className="mt-8 text-center text-sm text-gray-500">
        <Link
          to="/register"
          className="font-medium text-indigo-600 hover:text-indigo-500"
        >
          Aceptar y Continuar
        </Link>
      </div>

      <div className="mt-8 text-center text-sm text-gray-500">
        TodoSistema de Gestión Dental v{packageJson.version}
      </div>
    </div>
  );
}
