import { useState, useEffect } from "react";
import Header from "../components/Header";
import Sidebar from "../components/Sidebar";
import api from "../config/api";
import { jwtDecode } from "jwt-decode";

export const OrdersCustomers = () => {
  const [flowerType, setFlowerType] = useState("");
  const [flowerQuantity, setFlowerQuantity] = useState("");
  const [includeBase, setIncludeBase] = useState(false);
  const [baseType, setBaseType] = useState("");
  const [includePresent, setIncludePresent] = useState(false);
  const [presentType, setPresentType] = useState("");
  const [selectedPhoto, setSelectedPhoto] = useState(null);
  const [photoPreview, setPhotoPreview] = useState(null);
  const [clienteId, setClienteId] = useState(null);
  const token = localStorage.getItem("token");

  const flowerTypes = ["Rosas", "Tulipanes", "Lirios", "Orquídeas"];
  const baseTypes = ["Cerámica", "Vidrio", "Madera", "Metálica"];
  const presentTypes = ["Tarjeta", "Chocolates", "Peluche", "Vino"];

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

  const handlePhotoUpload = (event) => {
    const file = event.target.files[0];
    if (file) {
      setSelectedPhoto(file);

      const reader = new FileReader();
      reader.onloadend = () => {
        setPhotoPreview(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (!flowerType || !flowerQuantity) {
      alert("Por favor, selecciona el tipo y cantidad de flores");
      return;
    }

    const formData = new FormData();
    formData.append("TipoFlor", flowerType);
    formData.append("Cantidad", flowerQuantity);
    formData.append("IncluirPresente", includePresent ? "Si" : "No");
    formData.append("IncluirBase", includeBase ? "Si" : "No");
    formData.append("TipoPresente", includePresent ? presentType : "No");
    formData.append("TipoBase", includeBase ? baseType : "No");

    if (selectedPhoto) {
      formData.append("File", selectedPhoto);
    }

    formData.append("UserId", clienteId);

    try {
      const response = await api.post("/personalido", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      console.log("Respuesta del servidor:", response.data);
      alert("Pedido enviado exitosamente!");
      resetForm();
    } catch (error) {
      if (error.response) {
        if (error.response.data.errors) {
          alert("Error al enviar pedido. Por favor complete los datos");
        } else {
          alert(
            `Error del servidor (${error.response.status}): ${
              error.response.data.title || JSON.stringify(error.response.data)
            }`
          );
        }
      }
    }
  };

  const resetForm = () => {
    setFlowerType("");
    setFlowerQuantity("");
    setIncludeBase(false);
    setBaseType("");
    setIncludePresent(false);
    setPresentType("");
    setSelectedPhoto(null);
    setPhotoPreview(null);
  };

  return (
    <div className="flex">
      <Sidebar />
      <div className="flex-1 relative">
        <Header />
        <div className="p-6 bg-white">
          <form onSubmit={handleSubmit} className="grid grid-cols-2 gap-6">
            <div>
              <label className="block mb-2 text-sm font-medium text-gray-700">
                Seleccione Tipo de flor:
              </label>
              <select
                value={flowerType}
                onChange={(e) => setFlowerType(e.target.value)}
                className="w-full p-2 border rounded"
              >
                <option value="">Seleccionar</option>
                {flowerTypes.map((type) => (
                  <option key={type} value={type}>
                    {type}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block mb-2 text-sm font-medium text-gray-700">
                Cantidad de flores:
              </label>
              <select
                value={flowerQuantity}
                onChange={(e) => setFlowerQuantity(e.target.value)}
                className="w-full p-2 border rounded"
              >
                <option value="">Seleccionar</option>
                {[...Array(10)].map((_, i) => (
                  <option key={i + 1} value={i + 1}>
                    {i + 1}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block mb-2 text-sm font-medium text-gray-700">
                ¿Desea incluir un Presente?
              </label>
              <div className="flex space-x-4">
                <label className="inline-flex items-center">
                  <input
                    type="radio"
                    name="presente"
                    checked={includePresent === true}
                    onChange={() => setIncludePresent(true)}
                    className="form-radio h-4 w-4 text-indigo-600 border-gray-300 focus:ring-indigo-500"
                  />
                  <span className="ml-2">Sí</span>
                </label>
                <label className="inline-flex items-center">
                  <input
                    type="radio"
                    name="presente"
                    checked={includePresent === false}
                    onChange={() => setIncludePresent(false)}
                    className="form-radio h-4 w-4 text-indigo-600 border-gray-300 focus:ring-indigo-500"
                  />
                  <span className="ml-2">No</span>
                </label>
              </div>
            </div>

            {includePresent && (
              <div>
                <label className="block mb-2 text-sm font-medium text-gray-700">
                  Seleccione el tipo de Presente:
                </label>
                <select
                  value={presentType}
                  onChange={(e) => setPresentType(e.target.value)}
                  className="w-full p-2 border rounded"
                >
                  <option value="">Seleccionar</option>
                  {presentTypes.map((type) => (
                    <option key={type} value={type}>
                      {type}
                    </option>
                  ))}
                </select>
              </div>
            )}

            <div>
              <label className="block mb-2 text-sm font-medium text-gray-700">
                ¿Desea incluir una Base?
              </label>
              <div className="flex space-x-4">
                <label className="inline-flex items-center">
                  <input
                    type="radio"
                    name="base"
                    checked={includeBase === true}
                    onChange={() => setIncludeBase(true)}
                    className="form-radio h-4 w-4 text-indigo-600 border-gray-300 focus:ring-indigo-500"
                  />
                  <span className="ml-2">Sí</span>
                </label>
                <label className="inline-flex items-center">
                  <input
                    type="radio"
                    name="base"
                    checked={includeBase === false}
                    onChange={() => setIncludeBase(false)}
                    className="form-radio h-4 w-4 text-indigo-600 border-gray-300 focus:ring-indigo-500"
                  />
                  <span className="ml-2">No</span>
                </label>
              </div>
            </div>

            {includeBase && (
              <div>
                <label className="block mb-2 text-sm font-medium text-gray-700">
                  Seleccione Tipo de base:
                </label>
                <select
                  value={baseType}
                  onChange={(e) => setBaseType(e.target.value)}
                  className="w-full p-2 border rounded"
                >
                  <option value="">Seleccionar</option>
                  {baseTypes.map((type) => (
                    <option key={type} value={type}>
                      {type}
                    </option>
                  ))}
                </select>
              </div>
            )}

            <div className="col-span-2">
              <label className="block mb-2 text-sm font-medium text-gray-700">
                Subir Foto de Referencia
              </label>
              <div className="flex items-center space-x-4">
                <input
                  type="file"
                  accept="image/*"
                  onChange={handlePhotoUpload}
                  className="hidden"
                  id="photo-upload"
                />
                <label
                  htmlFor="photo-upload"
                  className="cursor-pointer bg-[#EC75D6] text-white px-4 py-2 rounded-md hover:bg-[#D44BB4] transition-colors"
                >
                  Seleccionar Foto
                </label>

                {photoPreview && (
                  <div className="w-24 h-24 overflow-hidden rounded-md">
                    <img
                      src={photoPreview}
                      alt="Vista previa"
                      className="w-full h-full object-cover"
                    />
                  </div>
                )}
              </div>
            </div>

            <div className="col-span-2">
              <button
                type="submit"
                disabled={!flowerType || !flowerQuantity}
                className="w-full p-3 bg-[#EC75D6] text-white py-3 rounded-md hover:bg-[#D44BB4] transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Enviar Pedido Personalizado
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
