import { useEffect, useState } from "react";
import Header from "../components/Header";
import Sidebar from "../components/Sidebar";
import { jwtDecode } from "jwt-decode";
import api from "../config/api";

export const OrdersPage = () => {
  const [orders, setOrders] = useState([]); // Estado para almacenar detalles de pedidos
  const [ordersCustom, setOrdersCustom] = useState([]); // Estado para almacenar pedidos perzonalizados
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [clienteId, setClienteId] = useState(null);
  const token = localStorage.getItem("token");

  useEffect(() => {
    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        setClienteId(
          decodedToken[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
          ] || decodedToken.id
        );
      } catch (error) {
        console.error("Error al decodificar el token:", error);
      }
      console.log(clienteId);
    }
  }, [token]);

  useEffect(() => {
    const fetchOrders = async () => {
      if (!clienteId) return; // Esperar hasta que clienteId esté disponible
      try {
        const response = await api.get(`/Detalle/cliente/${clienteId}`);
        if (response.data.status) {
          setOrders(response.data.data); // Extraer solo la data
        } else {
          setError("No se pudieron obtener los pedidos");
        }
      } catch (err) {
        setError("Error al cargar los pedidos");
      } finally {
        setLoading(false);
      }
    };

    fetchOrders();
  }, [clienteId]);

  const fetchOrdersCustom = async () => {
    if (!clienteId) return; // Esperar hasta que clienteId esté disponible
    try {
      const response = await api.get(`/personalido/cliente/${clienteId}`);
      if (response.data.status) {
        setOrdersCustom(response.data.data); // Extraer solo la data
      } else {
        setError("No se pudieron obtener los pedidos");
      }
    } catch (err) {
      setError("Error al cargar los pedidos");
    } finally {
      setLoading(false);
    }
  };

  fetchOrdersCustom(); // Llamar a la función para obtener pedidos personalizados
  useEffect(() => {
    fetchOrdersCustom();
  }, [clienteId]);

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className="flex-1 flex flex-col overflow-hidden">
        <Header />
        <div className="flex-1 overflow-auto p-6 space-y-6">
          <div>
            <h2 className="text-2xl font-bold mb-4 text-center">
              Lista de Pedidos
            </h2>

            {loading ? (
              <p>Cargando pedidos...</p>
            ) : error ? (
              <p className="text-red-500">{error}</p>
            ) : orders.length === 0 ? (
              <p>No hay detalles de pedidos disponibles</p>
            ) : (
              <div className="max-h-96 overflow-y-auto border border-gray-300 rounded">
                <table className="w-full border-collapse">
                  <thead>
                    <tr className="bg-gray-200 sticky top-0">
                      <th className="border border-gray-300 px-4 py-2">
                        Producto
                      </th>
                      <th className="border border-gray-300 px-4 py-2">
                        Cantidad
                      </th>
                      <th className="border border-gray-300 px-4 py-2">
                        Precio Unitario
                      </th>
                      <th className="border border-gray-300 px-4 py-2">
                        Total
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {orders.map((order) => (
                      <tr key={order.id} className="text-center">
                        <td className="border border-gray-300 px-4 py-2">
                          {order.productoNombre}
                        </td>
                        <td className="border border-gray-300 px-4 py-2">
                          {order.cantidad}
                        </td>
                        <td className="border border-gray-300 px-4 py-2">
                          {order.precioUnitario} LPS
                        </td>
                        <td className="border border-gray-300 px-4 py-2">
                          {order.total} LPS
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>

          <div>
            <h3 className="text-2xl font-bold mb-4 text-center">
              Lista de Pedidos Personalizados
            </h3>

            {loading ? (
              <p>Cargando pedidos personalizados...</p>
            ) : error ? (
              <p className="text-red-500">{error}</p>
            ) : ordersCustom.length === 0 ? (
              <p>No hay detalles de pedidos personalizados disponibles</p>
            ) : (
              <div className="max-h-96 overflow-y-auto border border-gray-300 rounded">
                <table className="w-full border-collapse">
                  <thead>
                    <tr className="bg-gray-200 sticky top-0">
                      <th className="border border-gray-300 px-4 py-2">
                        Tipo de Flor
                      </th>
                      <th className="border border-gray-300 px-4 py-2">
                        Cantidad
                      </th>
                      <th className="border border-gray-300 px-4 py-2">
                        Incluye Presente
                      </th>
                      <th className="border border-gray-300 px-4 py-2">
                        Incluye Base
                      </th>
                      <th className="border border-gray-300 px-4 py-2">
                        Tipo de Presente
                      </th>
                      <th className="border border-gray-300 px-4 py-2">
                        Tipo de Base
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {ordersCustom.map((order) => (
                      <tr key={order.id} className="text-center">
                        <td className="border border-gray-300 px-4 py-2">
                          {order.tipoFlor}
                        </td>
                        <td className="border border-gray-300 px-4 py-2">
                          {order.cantidad}
                        </td>
                        <td className="border border-gray-300 px-4 py-2">
                          {order.incluirPresente}
                        </td>
                        <td className="border border-gray-300 px-4 py-2">
                          {order.incluirBase}
                        </td>
                        <td className="border border-gray-300 px-4 py-2">
                          {order.tipoPresente}
                        </td>
                        <td className="border border-gray-300 px-4 py-2">
                          {order.tipoBase}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};
