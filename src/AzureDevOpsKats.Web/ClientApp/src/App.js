import React, { useState, useEffect } from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { Provider } from 'react-redux';
import { ToastContainer, toast } from 'react-toastify';
import store from './store';
import CustomRoutes from "./customRoutes";
import Navbar from "./components/Navbar";
import Footer from "./components/Footer";

const App = () => {  
  
    const [env, setEnv] = useState("aaa");    
    const theme = process.env.REACT_APP_THEME?.trim();
    useEffect(() => {
      if (theme == 'black') 
        setEnv("Aws");
      else if (theme == "blue")
        setEnv("Azure");
      else
        setEnv("");
    }, [])

    return (
      <Provider store = {store}>
        <Router>
          <div className="flyout">
            <Navbar theme={theme} env={env}/>
            <main style={{ marginTop: "4rem", paddingBottom: "3rem" }}>
              <CustomRoutes theme={theme}/>
            </main>
            <Footer theme={theme} env={env}/>
            <ToastContainer />
          </div>
        </Router>
      </Provider>
    );
}

export default App;
