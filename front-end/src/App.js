import './App.css';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

import Home from "./Pages/Home/Home";
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap/dist/js/bootstrap.js';

import Navbar from "./NavBar/Navbar";
import Footer from "./Footer/Footer";
import AboutUs from "./AboutUs/AboutUs";
import Products from "./Pages/Products/Products";
import Orders from "./Pages/Orders/Orders.js";

function App() {
  return (
      <Router>
        <Navbar />
        <div>
          <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/products" element={<Products />} />
          <Route path="/aboutus" element={<AboutUs />} />
          <Route path="/orders" element={<Orders />} />
          </Routes>
        </div>
          <Footer />
      </Router>
  );
}

export default App;
