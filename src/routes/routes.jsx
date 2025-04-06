import { lazy } from "react";
import PrivateRoute from "./PrivateRoute ";
import PublicRoute from "./PublicRoute";
import RoleBasedRoute from "../components/RoleBasedRoute";
import { InventoryAdminPage } from "../pages/Admin/InventoryAdminPage";
import ProfilePage from "../pages/ProfilePage";
import { OrdersCustomers } from "../pages/OrdersCustomersPage";
import AdminHomePage from "../pages/Admin/AdminHomePage";
import { OrdersPage } from "../pages/OrdersPage";
import { OrdersAdminPage } from "../pages/Admin/OrdersAdminPage";
import { SuppliersAdminPage } from "../pages/Admin/SuppliersAdminPage";

const Login = lazy(() => import("../pages/Login"));
const Register = lazy(() => import("../pages/Register"));
const ForgotPassword = lazy(() => import("../pages/ForgotPassword"));
const ResetPassword = lazy(() => import("../pages/ResetPassword"));
const Dashboard = lazy(() => import("../pages/Dashboard"));
const UserManagement = lazy(() => import("../pages/Admin/UserManagement"));
const TermsAndPrivacy = lazy(() => import("../pages/TermsAndPrivacy"));

export const routes = [
  {
    path: "/terms",
    element: <TermsAndPrivacy />,
  },
  {
    path: "/login",
    element: (
      <PublicRoute>
        <Login />
      </PublicRoute>
    ),
  },
  {
    path: "/register",
    element: (
      <PublicRoute>
        <Register />
      </PublicRoute>
    ),
  },
  {
    path: "/forgot-password",
    element: (
      <PublicRoute>
        <ForgotPassword />
      </PublicRoute>
    ),
  },
  {
    path: "/reset-password",
    element: (
      <PublicRoute>
        <ResetPassword />
      </PublicRoute>
    ),
  },
  {
    path: "/",
    element: (
      <PrivateRoute>
        <Dashboard />
      </PrivateRoute>
    ),
  },
  {
    path: "/profile",
    element: (
      <PrivateRoute>
        <ProfilePage />
      </PrivateRoute>
    ),
  },
  {
    path: "/admin/users",
    element: (
      <RoleBasedRoute roles={["Admin"]}>
        <UserManagement />
      </RoleBasedRoute>
    ),
  },
  {
    path: "/Suppliers",
    element: (
      <RoleBasedRoute roles={["Admin"]}>
        <SuppliersAdminPage />
      </RoleBasedRoute>
    ),
  },
  {
    path: "/OrdersAdmin",
    element: (
      <RoleBasedRoute roles={["Admin"]}>
        <OrdersAdminPage />
      </RoleBasedRoute>
    ),
  },
  {
    path: "/inventory",
    element: (
      <RoleBasedRoute roles={["Admin", "Inventario"]}>
        <InventoryAdminPage />
      </RoleBasedRoute>
    ),
  },
  {
    path: "/ordersCustomers",
    element: (
      // <RoleBasedRoute roles={["Admin", "Inventario"]}>
      <OrdersCustomers />
      // </RoleBasedRoute>
    ),
  },
  {
    path: "/orders",
    element: (
      <PrivateRoute>
        <OrdersPage />
      </PrivateRoute>
    ),
  },
  {
    path: "/adminHome",
    element: (
      <RoleBasedRoute roles={["Admin"]}>
        <AdminHomePage />
      </RoleBasedRoute>
    ),
  },
];
