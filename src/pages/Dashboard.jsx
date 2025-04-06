import React, { useState, useEffect } from "react";
import Header from "../components/Header";
import Sidebar from "../components/Sidebar";
import Card from "../components/Card";
import { jwtDecode } from "jwt-decode";
import api from "../config/api";

// const API_URL = "https://localhost:7227/api";

// Componente para el Modal de Detalles
const ProductDetailModal = ({ isOpen, onClose, product, onAddToOrder }) => {
  const [quantity, setQuantity] = useState(1); // Estado para manejar la cantidad seleccionada

  if (!isOpen || !product) return null;

  // Manejador para cambiar la cantidad
  const handleQuantityChange = (e) => {
    const value = Math.max(1, e.target.value); // Asegura que la cantidad no sea menor a 1
    setQuantity(value);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-2xl shadow-xl">
        <div className="flex justify-between items-start mb-4">
          <h2 className="text-2xl font-bold text-gray-800">{product.nombre}</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="flex items-center justify-center">
            <img
              src={product.imagenUrl}
              alt={product.nombre}
              className="rounded-lg max-h-64 object-contain"
              onError={(e) => (e.target.src = "/placeholder.jpg")}
            />
          </div>

          <div className="space-y-4">
            <div>
              <h3 className="text-sm font-medium text-gray-500">Categoría</h3>
              <p className="text-gray-900">{product.categoria}</p>
            </div>

            <div>
              <h3 className="text-sm font-medium text-gray-500">Precio</h3>
              <p className="text-gray-900">
                L.{" "}
                {typeof product.precio === "number"
                  ? product.precio.toFixed(2)
                  : product.precio}
              </p>
            </div>

            <div>
              <h3 className="text-sm font-medium text-gray-500">Cantidad</h3>
              <input
                type="number"
                value={quantity}
                onChange={handleQuantityChange}
                min="1"
                className="w-20 px-2 py-1 border rounded-md text-center"
              />
            </div>

            <div className="mt-6">
              <h3 className="text-sm font-medium text-gray-500 mb-2">
                Descripción
              </h3>
              <p className="text-gray-700">
                {product.descripcion || "No hay descripción disponible"}
              </p>
            </div>
          </div>
        </div>

        <div className="mt-8 flex justify-end space-x-3">
          <button
            onClick={onClose}
            className="px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-800 rounded-md"
          >
            Cerrar
          </button>
          <button
            onClick={() => {
              onAddToOrder(product, quantity); // Pasar la cantidad seleccionada junto con el producto
              alert(`Producto agregado al pedido. Cantidad: ${quantity}`);
              onClose();
            }}
            className="px-4 py-2 bg-[#c145aa] hover:bg-[#c156ad] text-white rounded-md"
          >
            Comprar
          </button>
        </div>
      </div>
    </div>
  );
};

const Dashboard = () => {
  const [cards, setCards] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [proveedores, setProveedores] = useState([]);
  const [newCard, setNewCard] = useState({
    id: null,
    nombre: "",
    descripcion: "",
    precio: 0,
    categoria: "",
    file: null,
    stock: 0,
    proveedorId: "",
  });

  // Estados para el modal de acciones (editar/eliminar)
  const [actionModalOpen, setActionModalOpen] = useState(false);
  const [selectedCardId, setSelectedCardId] = useState(null);

  // Estado para el modal de detalles
  const [detailModalOpen, setDetailModalOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

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
    }
  }, [token]);

  const handleBuyProduct = async (product, quantity) => {
    if (!clienteId) {
      alert("No se pudo obtener el ID del usuario.");
      return;
    }

    const data = {
      clienteId: clienteId,
      detalles: [
        {
          productoId: product.id,
          cantidad: quantity,
        },
      ],
    };

    try {
      const response = await api.post("/pedido", data, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`, // Si tu API requiere autenticación
        },
      });

      alert("Pedido realizado con éxito.");
    } catch (error) {
      console.error(
        "Error al realizar el pedido:",
        error.response?.data || error.message
      );
      alert("Error al realizar el pedido.");
    }
  };

  // Función para cargar tarjetas desde la API
  const fetchCards = async () => {
    try {
      const response = await api.get("/producto");
      if (response.data && Array.isArray(response.data.data)) {
        setCards(response.data.data);
      } else {
        console.error("La API no devolvió un array válido.");
      }
    } catch (error) {
      console.error("Error al obtener las tarjetas", error);
    }
  };

  // Cargar tarjetas desde la API
  useEffect(() => {
    fetchCards();
  }, []);

  // Cargar proveedores
  useEffect(() => {
    const fetchProveedores = async () => {
      try {
        const response = await api.get("/proveedor");
        if (response.data && Array.isArray(response.data.data)) {
          setProveedores(response.data.data);
        }
      } catch (error) {
        console.error("Error al obtener los proveedores:", error);
      }
    };

    fetchProveedores();
  }, []);

  // Manejar cambios en el formulario
  const handleChange = (e) => {
    const { name, value } = e.target;
    setNewCard({ ...newCard, [name]: value });
  };

  // Manejar archivos (imagen)
  const handleFileChange = (e) => {
    setNewCard({ ...newCard, file: e.target.files[0] });
  };

  // Enviar datos a la API
  const handleSubmit = async (e) => {
    e.preventDefault();

    let dataToSend;
    let headers = {};

    if (newCard.file) {
      const formData = new FormData();
      formData.append("file", newCard.file);
      formData.append("nombre", newCard.nombre);
      formData.append("descripcion", newCard.descripcion);
      formData.append("precio", newCard.precio);
      formData.append("categoria", newCard.categoria);
      formData.append("stock", newCard.stock);
      formData.append("proveedorId", newCard.proveedorId);

      dataToSend = formData;
      headers["Content-Type"] = "multipart/form-data";
    } else {
      dataToSend = {
        nombre: newCard.nombre,
        descripcion: newCard.descripcion,
        precio: newCard.precio,
        categoria: newCard.categoria,
        stock: newCard.stock,
        proveedorId: newCard.proveedorId,
      };
      headers["Content-Type"] = "application/json";
    }
    try {
      if (newCard.id) {
        // Editar producto (PUT)
        await api.put(`/producto/${newCard.id}`, dataToSend, {
          headers,
        });
      } else {
        // Crear producto (POST)
        await api.post("/producto", dataToSend, { headers });
      }
      await fetchCards();

      setShowModal(false);
      setNewCard({
        id: null,
        nombre: "",
        descripcion: "",
        precio: 0,
        categoria: "",
        file: null,
        stock: 0,
        proveedorId: "",
      });
    } catch (error) {
      console.error(
        "Error al agregar o editar la tarjeta:",
        error.response?.data || error.message
      );
    }
  };

  // Manejador para abrir el modal de detalles
  const handleCardClick = (id) => {
    const product = cards.find((card) => card.id === id);
    if (product) {
      setSelectedProduct(product);
      setDetailModalOpen(true);
    }
  };

  // Manejador para editar
  const handleEdit = (id) => {
    const cardToEdit = cards.find((card) => card.id === id);
    if (cardToEdit) {
      setNewCard({ ...cardToEdit }); // Cargar los datos del producto en el formulario
      setShowModal(true); // Abrir el modal
    }
    setActionModalOpen(false);
    setDetailModalOpen(false);
  };

  // Manejador para eliminar
  const handleDelete = async (id) => {
    try {
      await api.delete(`/producto/${id}`);
      // Recargar los datos desde la API después de eliminar
      await fetchCards();
      setActionModalOpen(false);
    } catch (error) {
      console.error("Error al eliminar la tarjeta:", error);
    }
  };

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className="flex-1 flex flex-col overflow-hidden">
        <Header />
        <div className="flex-1 overflow-y-auto p-4 md:p-8 lg:p-10">
          {/* Cards Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {cards.length > 0 ? (
              cards.map((card, index) => (
                <div key={card.id || `card-${index}`} className="relative">
                  <Card
                    onClick={() => handleCardClick(card.id)}
                    imagenUrl={card.imagenUrl}
                    nombre={card.nombre}
                    precio={card.precio}
                  />
                </div>
              ))
            ) : (
              <p className="text-gray-500 text-lg"></p>
            )}
          </div>

          {/* Modal de Detalles del Producto */}
          <ProductDetailModal
            isOpen={detailModalOpen}
            onClose={() => setDetailModalOpen(false)}
            product={selectedProduct}
            onAddToOrder={handleBuyProduct}
          />

          {cards.length === 0 && (
            <div className="text-center py-12">
              <p className="text-gray-500 text-lg mb-4">
                No hay productos disponibles
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
