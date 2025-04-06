import React from "react";

const Card = ({ nombre, precio, imagenUrl, onClick }) => {
  return (
    <div
      className="w-64 bg-white rounded-xl shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-300 cursor-pointer transform hover:-translate-y-1 transition-transform"
      onClick={onClick}
    >
      <div className="h-48 overflow-hidden">
        <img
          src={imagenUrl}
          alt={nombre}
          className="w-full h-full object-cover"
          onError={(e) => {
            e.target.src = "/placeholder.jpg";
            e.target.className = "w-full h-full object-contain p-4 bg-gray-100";
          }}
        />
      </div>
      <div className="p-4">
        <h3 className="text-xl font-bold text-gray-800 truncate">{nombre}</h3>
        <div className="mt-3 flex justify-between items-center">
          <span className="italic text-gray-700 font-semibold">
            {precio.toLocaleString("es-HN", {
              style: "currency",
              currency: "HNL",
              minimumFractionDigits: 2,
            })}
          </span>
          <button
            className="px-3 py-1 text-white text-sm font-medium rounded-full bg-[#EC75D6] hover:bg-[#fa6fe0] focus:outline-none"
            onClick={(e) => {
              e.stopPropagation();
              onClick?.();
            }}
          >
            Ver más
          </button>
        </div>
      </div>
    </div>
  );
};

export default Card;
