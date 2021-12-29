import React from "react";
import { Routes, Route } from 'react-router-dom';
import HomePage from "./pages/HomePage";
import PageNotFound from "./pages/PageNotFound";

const CustomRoutes = () => {
    return (
      <Routes>
        <Route exact path="/" element={<HomePage />} />        
        <Route path="*" element={<PageNotFound />} />
      </Routes>
    );
}

export default CustomRoutes;
