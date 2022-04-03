import React from "react";
import { Routes, Route } from 'react-router-dom';
import HomePage from "./pages/HomePage";
import PageNotFound from "./pages/PageNotFound";

const CustomRoutes = (props) => {
    return (
      <Routes>
        <Route exact path="/" element={<HomePage theme={props.theme}/>} />        
        <Route path="*" element={<PageNotFound />} />
      </Routes>
    );
}

export default CustomRoutes;
