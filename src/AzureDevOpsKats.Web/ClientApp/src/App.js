import React, { useState, useEffect } from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { Provider } from 'react-redux';
import { ToastContainer, toast } from 'react-toastify';
import store from './store';
import CustomRoutes from "./customRoutes";
import Navbar from "./components/Navbar";
import Footer from "./components/Footer";

const App = () => {  
  
    const [theme, setTheme] = useState("");    
    const env = process.env.REACT_APP_ENV?.trim();

    console.log("This is the REACT_APP_ENV : ", process.env.REACT_APP_ENV);

    useEffect(() => {
      if (env == 'AWS') 
        setTheme("black");
      else if (env == "AZURE")
        setTheme("blue");
      else
        setTheme("green");
    }, [])

    return (
      <Provider store = {store}>
        <Router>
          <div className="flyout">
            <Navbar theme={theme} env={env}/>
            <main style={{ marginTop: "4rem", paddingBottom: "3rem" }}>
              {/* <CustomRoutes theme={theme}/> */}
              <h1>I've removed this part for testing, because this part occurs an error after built.</h1>
            </main>
            <Footer theme={theme} env={env}/>
            <ToastContainer />
          </div>
        </Router>
      </Provider>
    );
}

export default App;
