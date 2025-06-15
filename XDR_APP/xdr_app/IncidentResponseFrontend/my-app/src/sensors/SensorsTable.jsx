import React, { useEffect, useState } from "react";
import { Col, Row } from "react-bootstrap";
import { CrudService } from "../services/CrudService";
import { ThreeDots } from "react-loader-spinner";
import { PlusSquareFill } from "react-bootstrap-icons";
import SensorRow from "./SensorRow";
import AddUpdateSensorModal from "./AddUpdateSensorModal";

const SensorsTable = () => {

    const [sensors, setSensors] = useState([]);
    const [sensorsLoading, setSensorsLoading] = useState(false);
    const [showAddSensorModal, setShowAddSensorModal] = useState(false);

    useEffect(() => {
        setSensorsLoading(true);
        // get sensors
        CrudService.list("Sensors").then((response) => {
            if (response.status === 200) {
                setSensors(response.data);
            }
            setSensorsLoading(false);
        });
    }, []);

    return(
        <Row className="mt-3">
            <Col>
                <table data-toggle="table" className="table table-bordered table-striped sensors-table">
                    <thead className="text-center">
                        <tr>
                            <th style={{ width: "35%" }}>Sensor</th>
                            <th style={{ width: "35%" }}>Type</th>
                            <th style={{ width: "20%" }}>Enabled</th>
                            <th>
                                <PlusSquareFill
                                    size={22}
                                    title="Add sensor"
                                    className="clickable color-bg-lightr"
                                    cursor="pointer"
                                    onClick={() => setShowAddSensorModal(true)}
                                />
                                <AddUpdateSensorModal 
                                    sensor={null}
                                    setSensors={setSensors}
                                    showModal={showAddSensorModal}
                                    setShowModal={setShowAddSensorModal}
                                />
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                    {
                        sensorsLoading ? (
                            <tr>
                                <td colSpan={4}>
                                    <div
                                        style={{ width: "100%" }}
                                        className="d-flex justify-content-center"
                                    >
                                        <ThreeDots
                                            height="50"
                                            width="50"
                                            ariaLabel="three-dots-loading"
                                            visible={true}
                                            color="#005bb5"
                                        />
                                    </div>
                                </td>
                            </tr>
                        ) : (
                            sensors.length ? (
                                sensors.map((sensor) => (
                                    <SensorRow
                                        key={`table-row-${sensor.sensorId}`}
                                        sensor={sensor}
                                        setSensors={setSensors}
                                    />
                                ))
                            ) : (
                                // if there are no sensors, display a message
                                <tr>
                                    <td colSpan={4} className="py-3"><span className="fw-semibold">No sensors added.</span></td>
                                </tr>
                            )
                        )
                    }
                    </tbody>
                </table>
            </Col>
        </Row>
    );
}

export default SensorsTable;