import React, { Component } from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { Provider } from 'react-redux';
import { ToastContainer, toast } from 'react-toastify';
import store from './store';

import Routes from "./Routes";
import Navbar from "./components/Navbar";
import Footer from "./components/Footer";

class App extends Component {

  render() {

    return (
      <Provider store = {store}>
        <Router>
          <div className="flyout">
            <Navbar />
            <main style={{ marginTop: "4rem", paddingBottom: "3rem" }}>
              <Routes />
            </main>
            <Footer/>

            <ToastContainer />
          </div>
        </Router>
      </Provider>
    );
  }
}

export default App;
