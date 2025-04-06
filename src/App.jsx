import React, { Suspense } from "react";
import { Routes, Route } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { Toaster } from "react-hot-toast";
import { routes } from "./routes/routes";

function App() {
  return (
    <AuthProvider>
      <div className="min-h-screen bg-gray-50">
        <Suspense
          fallback={
            <div className="flex min-h-[calc(100vh-4rem)] items-center justify-center">
              <div className="text-center">
                <div className="h-8 w-8 animate-spin rounded-full border-4 border-indigo-600 border-t-transparent"></div>
                <p className="mt-2 text-gray-600">Cargando...</p>
              </div>
            </div>
          }
        >
          <Routes>
            {routes.map((route, index) => (
              <Route key={index} path={route.path} element={route.element} />
            ))}
          </Routes>
        </Suspense>
        <Toaster position="top-right" />
      </div>
    </AuthProvider>
  );
}

export default App;
