import { Link } from 'react-router-dom';

const NotFound = () => {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-100">
      <h1 className="text-4xl font-bold mb-4">404</h1>
      <p className="mb-6">Página no encontrada</p>
      <Link to="/" className="px-4 py-2 bg-blue-500 text-white rounded">
        Volver al inicio
      </Link>
    </div>
  );
};

export default NotFound;
