import './App.css';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './AXIOS/AuthentificationContext';

import Home from "./Pages/Home/Home";
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap/dist/js/bootstrap.js';

import Navbar from "./NavBar/Navbar";
import Footer from "./Footer/Footer";
import AboutUs from "./AboutUs/AboutUs";
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


function App() {
  return (
    <AuthProvider>
      <Router>
        <Navbar />
        <div>
          <Routes>
            {/* Public routes */}
            <Route path="/" element={<Home />} />
            <Route path="/products" element={<Products />} />
            <Route path="/aboutus" element={<AboutUs />} />
            <Route path="/cart" element={<Cart />} />
            <Route path="/product/:productId" element={<ProductDetailsPage />} />
            <Route path="/admin/login" element={<OwnerLogin />} />

            {/* Protected routes - Owner access*/}

            {/* Products */}
            <Route path="/admin/product/update/:productId" element={
              <ProtectedRoute>
                <ProductUpdatePage />
              </ProtectedRoute>
            } />
            <Route path="/admin/product/create" element={
              <ProtectedRoute>
                <AddProductPage />
              </ProtectedRoute>
            } />

            {/* Orders */}
            <Route path="/orders" element={
              <ProtectedRoute>
                <Orders />
              </ProtectedRoute>
            } />

            <Route path="/orders/:orderId" element={
              <ProtectedRoute>
                <OrderDetail />
              </ProtectedRoute>
            } />
            <Route path="/admin/order/update/:orderId" element={
              <ProtectedRoute>
                <OrderUpdatePage />
              </ProtectedRoute>
            } />
        </Routes>

        </div>
        <br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/>
          <Footer />
      </Router>
    </AuthProvider>
  ); 
}

export default App;
