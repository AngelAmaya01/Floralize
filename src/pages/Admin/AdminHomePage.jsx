import React, { useState, useEffect } from "react";
import { FaPlus } from "react-icons/fa6";
import Header from "../../components/Header";
import Sidebar from "../../components/Sidebar";
import Card from "../../components/Card";
import api from "../../config/api";
import toast from "react-hot-toast";

// const API_URL = 'https://localhost:7227/api';

// Componente para el Modal de Detalles
const ProductDetailModal = ({ isOpen, onClose, product, onEdit, onDelete }) => {
  if (!isOpen || !product) return null;
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
              <h3 className="text-sm font-medium text-gray-500">Stock</h3>
              <p className="text-gray-900">{product.stock} unidades</p>
            </div>
          </div>
        </div>

        <div className="mt-6">
          <h3 className="text-sm font-medium text-gray-500 mb-2">
            Descripción
          </h3>
          <p className="text-gray-700">
            {product.descripcion || "No hay descripción disponible"}
          </p>
        </div>

        <div className="mt-8 flex justify-end space-x-3">
          <button
            onClick={onClose}
            className="px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-800 rounded-md"
          >
            Cerrar
          </button>

          <button
            onClick={async () => {
              const confirmed = window.confirm(
                "¿Estás seguro que deseas eliminar este producto?"
              );
              if (confirmed) {
                try {
                  await onDelete(product.id);
                  onClose();
                  toast.success("Producto eliminado correctamente", {
                    position: "top-center",
                  });
                } catch (error) {
                  toast.error("Error al eliminar el producto", {
                    position: "top-center",
                  });
                }
              }
            }}
            className="px-4 py-2 bg-[#c145aa] hover:bg-[#c156ad] text-white rounded-md"
          >
            Eliminar
          </button>
          <button
            onClick={() => {
              onClose();
              onEdit(product.id);
            }}
            className="px-4 py-2 bg-[#f884e3] hover:bg-[#fb7ce4] text-white rounded-md"
          >
            Editar
          </button>
        </div>
      </div>
    </div>
  );
};

const AdminHomePage = () => {
  const [cards, setCards] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [proveedores, setProveedores] = useState([]);
  const [categorias, setCategorias] = useState([]);
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
    fetchCategorias();
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

  // Obtiene las categorias
  const fetchCategorias = async () => {
    try {
      const response = await api.get("/categoria");
      if (response.data && Array.isArray(response.data.data)) {
        setCategorias(response.data.data);
      }
    } catch (error) {
      console.error("Error al obtener las categorías:", error);
    }
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
        <div className="flex-1 overflow-y-auto p-6 md:p-8 lg:p-10">
          <div className="flex justify-end items-center mb-6">
            <button
              onClick={() => {
                setShowModal(true);
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
              }}
              className="bg-[#EC75D6] hover:bg-[#fa6fe0] text-white p-4 rounded-full shadow-md transition-colors duration-200 fixed bottom-9 right-15 z-30"
            >
              <FaPlus className="text-3xl" />
            </button>
          </div>

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
            onEdit={handleEdit}
            onDelete={handleDelete}
          />

          {/* Modal de formulario para crear/editar producto */}
          {showModal && (
            <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
              <div className="bg-white rounded-lg shadow-xl w-full max-w-md">
                <div className="border-b p-4">
                  <h3 className="text-xl font-semibold text-gray-800">
                    {newCard.id ? "Editar Producto" : "Agregar Nuevo Producto"}
                  </h3>
                </div>
                <form onSubmit={handleSubmit} className="p-4">
                  <div className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Nombre:
                      </label>
                      <input
                        type="text"
                        name="nombre"
                        value={newCard.nombre}
                        onChange={handleChange}
                        required
                        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Descripción:
                      </label>
                      <textarea
                        name="descripcion"
                        value={newCard.descripcion}
                        onChange={handleChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        rows="3"
                      />
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                          Precio:
                        </label>
                        <input
                          type="number"
                          name="precio"
                          value={newCard.precio}
                          onChange={handleChange}
                          required
                          className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        />
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                          Stock:
                        </label>
                        <input
                          type="number"
                          name="stock"
                          value={newCard.stock}
                          onChange={handleChange}
                          required
                          className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        />
                      </div>
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Categoría:
                      </label>
                      <select
                        name="categoria"
                        value={newCard.categoria}
                        onChange={handleChange}
                        required
                        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 bg-white"
                      >
                        <option value="">Seleccionar categoría</option>
                        {categorias.length > 0 ? (
                          categorias.map((categoria) => (
                            <option
                              key={categoria.id}
                              value={categoria.descripcion}
                            >
                              {categoria.descripcion}
                            </option>
                          ))
                        ) : (
                          <option disabled>
                            No hay categorías disponibles
                          </option>
                        )}
                      </select>
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Proveedor:
                      </label>
                      <select
                        name="proveedorId"
                        value={newCard.proveedorId}
                        onChange={handleChange}
                        required
                        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 bg-white"
                      >
                        <option value="">Seleccionar proveedor</option>
                        {proveedores.length > 0 ? (
                          proveedores.map((proveedor) => (
                            <option key={proveedor.id} value={proveedor.id}>
                              {proveedor.nombre}
                            </option>
                          ))
                        ) : (
                          <option disabled>
                            No hay proveedores disponibles
                          </option>
                        )}
                      </select>
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Imagen:
                      </label>
                      <input
                        type="file"
                        onChange={handleFileChange}
                        className="w-full px-3 py-2 text-sm text-gray-700 bg-gray-50 rounded-md border border-gray-300 focus:outline-none"
                      />
                    </div>
                  </div>

                  <div className="mt-6 flex justify-end space-x-3">
                    <button
                      type="button"
                      onClick={() => setShowModal(false)}
                      className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
                    >
                      Cancelar
                    </button>
                    <button
                      type="submit"
                      className="px-4 py-2 text-sm font-medium text-white bg-[#EC75D6] hover:bg-[#fa6fe0] rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                    >
                      Guardar
                    </button>
                  </div>
                </form>
              </div>
            </div>
          )}

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

export default AdminHomePage;
