import React from "react";
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom';
import UserNavbar from "../navbars/UserNavbar";
import Sensors from "../../sensors/SensorsTable";
import Incidents from "../../incidents/Incidents";
import GrafanaDashboard from "../../dashboard/GrafanaDashboard";
const RoutesGroup = () => {
    return (
        <BrowserRouter basename="/">
            <UserNavbar />
            <Routes>
                <Route path="/" element={<Navigate replace to="/sensors" />}></Route>
                <Route path="sensors" element={<Sensors />}></Route>
                <Route path="incidents" element={<Incidents />}></Route>
                <Route path="dashboard" element={<GrafanaDashboard/>}></Route>
                <Route path="*" element={<Sensors />}></Route>
            </Routes>
        </BrowserRouter>
    );
};

export default RoutesGroup;