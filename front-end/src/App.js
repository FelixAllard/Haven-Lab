import './App.css';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './AXIOS/AuthentificationContext';

import Home from "./Pages/Home/Home";
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap/dist/js/bootstrap.js';
import "./Shared/global.css"

import Navbar from "./NavBar/Navbar";
import Footer from "./Footer/Footer";
import AboutUs from "./Pages/AboutUs/AboutUs";
import Products from "./Pages/Products/ProductsPage";
import Orders from "./Pages/Orders/Orders.js";
import OrderDetail from './Pages/Orders/OrderDetail';
import ProductDetailsPage from "./Pages/Products/ProductDetailPage";
import AddProductPage from "./Pages/Admin/Product/AddProductPage";
import ProductUpdatePage from "./Pages/Admin/Product/UpdateProductPage.js";
import Cart from "./Pages/Cart/Cart.js"
import OwnerLogin from "./Pages/Admin/Authentification/OwnerLogin.js"
import OrderUpdatePage from './Pages/Orders/OrderUpdatePage.js';
import ProtectedRoute from './AXIOS/ProtectedRoute.js';
import Appointments from './Pages/Appointments/Appointments.js';
import EmailSendPage from "./Pages/Email/EmailSendPage";
import AppointmentDetails from './Pages/Appointments/AppointmentDetails.js';
import AppointmentUpdate from './Pages/Appointments/AppointmentUpdate.js';
import AppointmentCreate from './Pages/Appointments/AppointmentCreate.js';


function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="app-container">
          <Navbar />
          <main className="content">
            <Routes>
              {/* Public routes */}
              <Route path="/" element={<Home />} />
              <Route path="/aboutus" element={<AboutUs />} />
              <Route path="/products" element={<Products />} />
              <Route path="/product/:productId" element={<ProductDetailsPage />} />
              <Route path="/cart" element={<Cart />} />
              <Route path="/admin/login" element={<OwnerLogin />} />

              {/* Protected routes - Owner access */}
              <Route
                path="/admin/product/update/:productId"
                element={
                  <ProtectedRoute>
                    <ProductUpdatePage />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/admin/product/create"
                element={
                  <ProtectedRoute>
                    <AddProductPage />
                  </ProtectedRoute>
                }
              />
                
                <Route 
                    path="/admin/email/send" 
                    element={
                    <ProtectedRoute>
                        <EmailSendPage />
                    </ProtectedRoute>
                } />

              {/* Orders */}
              <Route
                path="/orders"
                element={
                  <ProtectedRoute>
                    <Orders />
                  </ProtectedRoute>
                }
              />
                
              <Route
                path="/orders/:orderId"
                element={
                  <ProtectedRoute>
                    <OrderDetail />
                  </ProtectedRoute>
                }
              />
                
              <Route
                path="/admin/order/update/:orderId"
                element={
                  <ProtectedRoute>
                    <OrderUpdatePage />
                  </ProtectedRoute>
                }
              />

                {/* Appointments */}
              <Route path="/appointments" 
                element={
                    <ProtectedRoute>
                        <Appointments />
                    </ProtectedRoute>
                } 
              />

              <Route path="/appointments/:appointmentId" 
                element={
                    <ProtectedRoute>
                        <AppointmentDetails />
                    </ProtectedRoute>
                } 
              />

              <Route path="/appointments/update/:appointmentId" 
                element={
                    <ProtectedRoute>
                        <AppointmentUpdate />
                    </ProtectedRoute>
                } 
              />

              <Route path="/appointments/create" 
                element={
                    <ProtectedRoute>
                        <AppointmentCreate />
                    </ProtectedRoute>
                } 
              />
            </Routes>
          </main>
          <Footer />
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;
