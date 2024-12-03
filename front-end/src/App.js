import './App.css';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

import Home from "./Pages/Home/Home";
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap/dist/js/bootstrap.js';

import Navbar from "./NavBar/Navbar";
import Footer from "./Footer/Footer";

function App() {
  return (
      <Router>
        <Navbar />
        <div>
          <Routes>
            <Route path="/" element={<Home />} />
          </Routes>
        </div>
          <Footer />
      </Router>
  );
}

export default App;
