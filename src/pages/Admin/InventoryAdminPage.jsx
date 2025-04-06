import React, { useState, useEffect } from "react";
import Header from "../../components/Header";
import Sidebar from "../../components/Sidebar";
import api from "../../config/api";

export const InventoryAdminPage = () => {
  const [descripcion, setDescripcion] = useState("");
  const [nombre, setNombre] = useState("");
  const [cantidad, setCantidad] = useState(0);
  const [ubicacion, setUbicacion] = useState("");
  const [categorias, setCategorias] = useState([]);
  const [inventarios, setInventarios] = useState([]);
  const [isCategoriaModalOpen, setIsCategoriaModalOpen] = useState(false);
  const [isInventarioModalOpen, setIsInventarioModalOpen] = useState(false);
  const [categoriaEdit, setCategoriaEdit] = useState(null);
  const [isEditInventarioModalOpen, setIsEditInventarioModalOpen] =
    useState(false);
  const [inventarioEdit, setInventarioEdit] = useState(null);
  const [selectedCategoriaId, setSelectedCategoriaId] = useState("");

  // Obtener categorías
  const fetchCategorias = async () => {
    try {
      const response = await api.get("/categoria");
      console.log("Respuesta de categorías:", response.data);

      // Ajuste según la estructura real de tu respuesta
      if (response.data && response.data.status) {
        setCategorias(response.data.data || []);
      } else {
        console.error("Error al obtener categorías:", response.data?.message);
        alert(response.data?.message || "Error al obtener categorías");
      }
    } catch (error) {
      console.error("Error al obtener categorías", error);
      alert(error.response?.data?.message || "Error al obtener categorías");
    }
  };

  // Obtener inventarios
  const fetchInventarios = async () => {
    try {
      const response = await api.get("/Inventario");
      console.log("Respuesta de inventarios:", response.data);

      // Ajuste según la estructura real de tu respuesta
      if (response.data && response.data.status) {
        setInventarios(response.data.data || []);
      } else {
        console.error("Error al obtener inventarios:", response.data?.message);
        alert(response.data?.message || "Error al obtener inventarios");
      }
    } catch (error) {
      console.error("Error al obtener inventarios", error);
      alert(error.response?.data?.message || "Error al obtener inventarios");
    }
  };

  useEffect(() => {
    fetchCategorias();
    fetchInventarios();
  }, []);

  // Crear categoría
  const handleCrearCategoria = async (e) => {
    e.preventDefault();
    const categoriaData = { descripcion };

    try {
      const response = await api.post("/categoria", categoriaData);
      console.log("Respuesta crear categoría:", response.data);

      if (response.data && response.data.status) {
        alert("Categoría creada con éxito!");
        await fetchCategorias();
        setDescripcion("");
        setIsCategoriaModalOpen(false);
      } else {
        alert(response.data?.message || "Error al crear la categoría");
      }
    } catch (error) {
      console.error("Error al crear categoría:", error);
      alert(error.response?.data?.message || "Error al crear la categoría");
    }
  };

  // Editar categoría
  const handleEditarCategoria = (categoria) => {
    setCategoriaEdit(categoria);
    setDescripcion(categoria.descripcion);
    setIsCategoriaModalOpen(true);
  };

  const handleActualizarCategoria = async (e) => {
    e.preventDefault();
    if (!categoriaEdit) return;

    const categoriaData = { descripcion };
    try {
      const response = await api.put(
        `/categoria/${categoriaEdit.id}`,
        categoriaData
      );

      if (response.data && response.data.status) {
        alert("Categoría actualizada con éxito!");
        await fetchCategorias();
        setDescripcion("");
        setCategoriaEdit(null);
        setIsCategoriaModalOpen(false);
      } else {
        alert(response.data?.message || "Error al actualizar la categoría");
      }
    } catch (error) {
      console.error("Error al actualizar categoría:", error);
      alert(
        error.response?.data?.message || "Error al actualizar la categoría"
      );
    }
  };

  // Eliminar categoría
  const handleEliminarCategoria = async (id) => {
    const confirmacion = window.confirm(
      "¿Estás seguro de que quieres eliminar esta categoría?"
    );
    if (!confirmacion) return;

    try {
      const response = await api.delete(`/categoria/${id}`);
      console.log("Respuesta eliminar categoría:", response.data);

      if (response.data && response.data.status) {
        alert("Categoría eliminada con éxito!");
        await fetchCategorias();
      } else {
        alert(response.data?.message || "Error al eliminar la categoría");
      }
    } catch (error) {
      console.error("Error al eliminar categoría:", error);
      alert(error.response?.data?.message || "Error al eliminar la categoría");
    }
  };

  // Modal para categoría
  const renderModalCategoria = () => (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center">
      <div className="bg-white p-6 rounded-lg w-1/3">
        <h3 className="text-lg font-semibold mb-4">
          {categoriaEdit ? "Editar Categoría" : "Crear Categoría"}
        </h3>
        <form
          onSubmit={
            categoriaEdit ? handleActualizarCategoria : handleCrearCategoria
          }
          className="space-y-4"
        >
          <div>
            <input
              type="text"
              placeholder="Descripción de la categoría"
              value={descripcion}
              onChange={(e) => setDescripcion(e.target.value)}
              className="w-full p-3 border border-gray-300 rounded-md"
              required
            />
          </div>
          <div className="flex justify-between items-center">
            <button
              type="submit"
              className="w-full p-3 bg-blue-500 text-white rounded-md"
            >
              {categoriaEdit ? "Actualizar" : "Crear"} Categoría
            </button>
            <button
              type="button"
              onClick={() => {
                setIsCategoriaModalOpen(false);
                setCategoriaEdit(null);
                setDescripcion("");
              }}
              className="ml-4 text-gray-500 hover:text-gray-800"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  );

  // Crear inventario
  const handleCrearInventario = async (e) => {
    e.preventDefault();
    if (!selectedCategoriaId) {
      alert("Por favor seleccione una categoría");
      return;
    }

    const inventarioData = {
      nombre,
      cantidad: parseInt(cantidad),
      ubicacion,
      categoriaId: selectedCategoriaId,
    };

    try {
      const response = await api.post("/Inventario", inventarioData);
      console.log("Respuesta crear inventario:", response.data);

      if (response.data && response.data.status) {
        alert("Inventario creado con éxito!");
        await fetchInventarios();
        resetInventarioForm();
        setIsInventarioModalOpen(false);
      } else {
        alert(response.data?.message || "Error al crear el inventario");
      }
    } catch (error) {
      console.error("Error al crear inventario:", error);
      alert(error.response?.data?.message || "Error al crear el inventario");
    }
  };

  // Eliminar inventario
  const handleEliminarInventario = async (id) => {
    const confirmacion = window.confirm(
      "¿Estás seguro de que quieres eliminar este inventario?"
    );
    if (!confirmacion) return;

    try {
      const response = await api.delete(`/Inventario/${id}`);
      console.log("Respuesta eliminar inventario:", response.data);

      if (response.data && response.data.status) {
        alert("Inventario eliminado con éxito!");
        await fetchInventarios();
      } else {
        alert(response.data?.message || "Error al eliminar el inventario");
      }
    } catch (error) {
      console.error("Error al eliminar inventario:", error);
      alert(error.response?.data?.message || "Error al eliminar el inventario");
    }
  };

  // Editar inventario
  const handleEditarInventario = (inventario) => {
    setInventarioEdit(inventario);
    setNombre(inventario.nombre);
    setCantidad(inventario.cantidad);
    setUbicacion(inventario.ubicacion);
    setSelectedCategoriaId(inventario.categoriaId);
    setIsEditInventarioModalOpen(true);
  };

  // Actualizar inventario
  const handleActualizarInventario = async (e) => {
    e.preventDefault();
    if (!inventarioEdit || !selectedCategoriaId) {
      alert("Error: Datos incompletos para actualizar el inventario.");
      return;
    }

    const inventarioData = {
      nombre,
      cantidad: parseInt(cantidad),
      ubicacion,
      categoriaId: selectedCategoriaId,
    };

    try {
      const response = await api.put(
        `/Inventario/${inventarioEdit.id}`,
        inventarioData
      );
      console.log("Respuesta actualizar inventario:", response.data);

      if (response.data && response.data.status) {
        alert("Inventario actualizado con éxito!");
        await fetchInventarios();
        resetInventarioForm();
        setInventarioEdit(null);
        setIsEditInventarioModalOpen(false);
      } else {
        alert(response.data?.message || "Error al actualizar el inventario");
      }
    } catch (error) {
      console.error("Error al actualizar inventario:", error);
      alert(
        error.response?.data?.message || "Error al actualizar el inventario"
      );
    }
  };

  // Resetear formulario de inventario
  const resetInventarioForm = () => {
    setNombre("");
    setCantidad(0);
    setUbicacion("");
    setSelectedCategoriaId("");
  };

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className="flex-1 flex flex-col overflow-hidden overflow-y-auto">
        <Header />
        <div className="container mx-auto p-6">
          <h2 className="text-2xl font-semibold mb-4">
            Administrar Inventario
          </h2>

          {/* Botón para abrir el modal de categoría */}
          <button
            onClick={() => {
              setCategoriaEdit(null);
              setDescripcion("");
              setIsCategoriaModalOpen(true);
            }}
            className="mb-6 p-3 bg-[#9a5ea7] text-white rounded-md"
          >
            Crear Categoría
          </button>

          {/* Modal para categoría */}
          {isCategoriaModalOpen && renderModalCategoria()}

          {/* Lista de categorías */}
          <div className="bg-white shadow-md rounded-lg p-6 mb-6">
            <h3 className="text-lg font-semibold mb-4">Lista de Categorías</h3>
            <table className="w-full border-collapse border border-gray-200">
              <thead>
                <tr className="bg-gray-100">
                  <th className="border p-3 text-left">Descripción</th>
                  <th className="border p-3 text-center">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {categorias.length > 0 ? (
                  categorias.map((categoria) => (
                    <tr key={categoria.id} className="hover:bg-gray-50">
                      <td className="border p-3">{categoria.descripcion}</td>
                      <td className="border p-3 text-center space-x-2">
                        <button
                          onClick={() => handleEditarCategoria(categoria)}
                          className="bg-blue-500 text-white px-4 py-2 rounded-md"
                        >
                          Editar
                        </button>
                        <button
                          onClick={() => handleEliminarCategoria(categoria.id)}
                          className="bg-red-500 text-white px-4 py-2 rounded-md"
                        >
                          Eliminar
                        </button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="2" className="border p-3 text-center">
                      No hay categorías registradas.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>

          {/* Botón para abrir el modal de inventario */}
          <button
            onClick={() => {
              setInventarioEdit(null);
              resetInventarioForm();
              setIsInventarioModalOpen(true);
            }}
            className="mb-6 p-3 bg-[#9a5ea7] text-white rounded-md"
          >
            Crear Inventario
          </button>

          {/* Modal para crear inventario */}
          {isInventarioModalOpen && (
            <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center">
              <div className="bg-white p-6 rounded-lg w-1/3">
                <h3 className="text-lg font-semibold mb-4">Crear Inventario</h3>
                <form onSubmit={handleCrearInventario} className="space-y-4">
                  <div>
                    <input
                      type="text"
                      placeholder="Nombre del inventario"
                      value={nombre}
                      onChange={(e) => setNombre(e.target.value)}
                      className="w-full p-3 border border-gray-300 rounded-md"
                      required
                    />
                  </div>
                  <div>
                    <input
                      type="number"
                      placeholder="Cantidad"
                      value={cantidad}
                      onChange={(e) => setCantidad(e.target.value)}
                      className="w-full p-3 border border-gray-300 rounded-md"
                      required
                      min="0"
                    />
                  </div>
                  <div>
                    <input
                      type="text"
                      placeholder="Ubicación"
                      value={ubicacion}
                      onChange={(e) => setUbicacion(e.target.value)}
                      className="w-full p-3 border border-gray-300 rounded-md"
                      required
                    />
                  </div>
                  <div>
                    <select
                      className="w-full p-3 border border-gray-300 rounded-md"
                      value={selectedCategoriaId}
                      onChange={(e) => setSelectedCategoriaId(e.target.value)}
                      required
                    >
                      <option value="" disabled>
                        Seleccione una categoría
                      </option>
                      {categorias.map((categoria) => (
                        <option key={categoria.id} value={categoria.id}>
                          {categoria.descripcion}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="flex justify-between items-center">
                    <button
                      type="submit"
                      className="w-full p-3 bg-blue-500 text-white rounded-md"
                    >
                      Crear Inventario
                    </button>
                    <button
                      type="button"
                      onClick={() => {
                        setIsInventarioModalOpen(false);
                        resetInventarioForm();
                      }}
                      className="ml-4 text-gray-500 hover:text-gray-800"
                    >
                      Cancelar
                    </button>
                  </div>
                </form>
              </div>
            </div>
          )}

          {/* Modal para editar inventario */}
          {isEditInventarioModalOpen && (
            <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center">
              <div className="bg-white p-6 rounded-lg w-1/3">
                <h3 className="text-lg font-semibold mb-4">
                  Editar Inventario
                </h3>
                <form
                  onSubmit={handleActualizarInventario}
                  className="space-y-4"
                >
                  <div>
                    <input
                      type="text"
                      placeholder="Nombre"
                      value={nombre}
                      onChange={(e) => setNombre(e.target.value)}
                      className="w-full p-3 border border-gray-300 rounded-md"
                      required
                    />
                  </div>
                  <div>
                    <input
                      type="number"
                      placeholder="Cantidad"
                      value={cantidad}
                      onChange={(e) => setCantidad(e.target.value)}
                      className="w-full p-3 border border-gray-300 rounded-md"
                      required
                      min="0"
                    />
                  </div>
                  <div>
                    <input
                      type="text"
                      placeholder="Ubicación"
                      value={ubicacion}
                      onChange={(e) => setUbicacion(e.target.value)}
                      className="w-full p-3 border border-gray-300 rounded-md"
                      required
                    />
                  </div>
                  <div>
                    <select
                      value={selectedCategoriaId}
                      onChange={(e) => setSelectedCategoriaId(e.target.value)}
                      className="w-full p-3 border border-gray-300 rounded-md"
                      required
                    >
                      <option value="" disabled>
                        Seleccionar Categoría
                      </option>
                      {categorias.map((categoria) => (
                        <option key={categoria.id} value={categoria.id}>
                          {categoria.descripcion}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="flex justify-between items-center">
                    <button
                      type="submit"
                      className="w-full p-3 bg-blue-500 text-white rounded-md"
                    >
                      Actualizar Inventario
                    </button>
                    <button
                      type="button"
                      onClick={() => {
                        setIsEditInventarioModalOpen(false);
                        resetInventarioForm();
                      }}
                      className="ml-4 text-gray-500 hover:text-gray-800"
                    >
                      Cancelar
                    </button>
                  </div>
                </form>
              </div>
            </div>
          )}

          {/* Lista de inventarios */}
          <div className="bg-white shadow-md rounded-lg p-6">
            <h3 className="text-lg font-semibold mb-4">Lista de Inventarios</h3>
            <table className="w-full border-collapse border border-gray-200">
              <thead>
                <tr className="bg-gray-100">
                  <th className="border p-3 text-left">Nombre</th>
                  <th className="border p-3 text-left">Cantidad</th>
                  <th className="border p-3 text-left">Ubicación</th>
                  <th className="border p-3 text-left">Categoría</th>
                  <th className="border p-3 text-center">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {inventarios.length > 0 ? (
                  inventarios.map((inventario) => {
                    return (
                      <tr key={inventario.id} className="hover:bg-gray-50">
                        <td className="border p-3">{inventario.nombre}</td>
                        <td className="border p-3">{inventario.cantidad}</td>
                        <td className="border p-3">{inventario.ubicacion}</td>
                        <td className="border p-3">
                          {inventario.categoriaNombre || "Sin categoria"}
                        </td>
                        <td className="border p-3 text-center space-x-2">
                          <button
                            onClick={() => handleEditarInventario(inventario)}
                            className="bg-blue-500 text-white px-4 py-2 rounded-md"
                          >
                            Editar
                          </button>
                          <button
                            onClick={() =>
                              handleEliminarInventario(inventario.id)
                            }
                            className="bg-red-500 text-white px-4 py-2 rounded-md"
                          >
                            Eliminar
                          </button>
                        </td>
                      </tr>
                    );
                  })
                ) : (
                  <tr>
                    <td colSpan="5" className="border p-3 text-center">
                      No hay inventarios registrados.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
};
