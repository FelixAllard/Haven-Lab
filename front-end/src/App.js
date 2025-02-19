import './App.css';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './AXIOS/AuthentificationContext';
import Home from './Pages/Home/Home';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap/dist/js/bootstrap.js';
import './Shared/global.css';

import Navbar from './NavBar/Navbar';
import Footer from './Footer/Footer';
import AboutUs from './Pages/AboutUs/AboutUs';
import Products from './Pages/Products/ProductsPage';
import Orders from './Pages/Admin/Orders/Orders.js';
import OrderDetail from './Pages/Admin/Orders/OrderDetail.js';
import ProductDetailsPage from './Pages/Products/ProductDetailPage';
import AddProductPage from './Pages/Admin/Product/AddProductPage';
import ProductUpdatePage from './Pages/Admin/Product/UpdateProductPage.js';
import Cart from './Pages/Cart/Cart.js';
import OwnerLogin from './Pages/Admin/Authentification/OwnerLogin.js';
import OrderUpdatePage from './Pages/Admin/Orders/OrderUpdatePage.js';
import ProtectedRoute from './AXIOS/ProtectedRoute.js';
import Appointments from './Pages/Admin/Appointments/Appointments.js';
import AppointmentDetails from './Pages/Admin/Appointments/AppointmentDetails.js';
import AppointmentUpdate from './Pages/Admin/Appointments/AppointmentUpdate.js';
import AppointmentCreate from './Pages/Admin/Appointments/AppointmentCreate.js';
import PriceRules from './Pages/Admin/Promos/PriceRules.js';
import PriceRuleDetail from './Pages/Admin/Promos/PriceRuleDetail.js';
import { EmailPage } from './Pages/Admin/Email/EmailPage.js';
import AddTemplatePage from './Pages/Admin/Email/Template/AddTemplatePage.js';
import ModifyTemplatePage from './Pages/Admin/Email/Template/ModifyTemplatePage.js';
import EmailSentPage from './Pages/Admin/Email/Logs/EmailSentPage.js';
import AddPriceRule from './Pages/Admin/Promos/AddPriceRule.js';
import UpdatePriceRule from './Pages/Admin/Promos/UpdatePriceRule.js';

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
              <Route
                path="/product/:productId"
                element={<ProductDetailsPage />}
              />
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
                    <EmailPage />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/admin/email/template/add"
                element={
                  <ProtectedRoute>
                    <AddTemplatePage />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/admin/email/template/modify/:templatename"
                element={
                  <ProtectedRoute>
                    <ModifyTemplatePage />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/admin/email/sent"
                element={
                  <ProtectedRoute>
                    <EmailSentPage />
                  </ProtectedRoute>
                }
              />
              {/* Orders */}
              <Route
                path="/admin/orders"
                element={
                  <ProtectedRoute>
                    <Orders />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/admin/orders/:orderId"
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
              <Route
                path="/admin/appointments"
                element={
                  <ProtectedRoute>
                    <Appointments />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/admin/appointments/:appointmentId"
                element={
                  <ProtectedRoute>
                    <AppointmentDetails />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/admin/appointments/update/:appointmentId"
                element={
                  <ProtectedRoute>
                    <AppointmentUpdate />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/admin/appointments/create"
                element={
                  <ProtectedRoute>
                    <AppointmentCreate />
                  </ProtectedRoute>
                }
              />
              {/* Promo */}
              <Route
                path="/admin/promo/pricerules"
                element={
                  <ProtectedRoute>
                    <PriceRules />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/admin/promo/pricerules/:priceruleId"
                element={
                  <ProtectedRoute>
                    <PriceRuleDetail />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/admin/promo/pricerules/create"
                element={
                  <ProtectedRoute>
                    <AddPriceRule />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/admin/promo/pricerules/update/:priceruleId"
                element={
                  <ProtectedRoute>
                    <UpdatePriceRule />
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
