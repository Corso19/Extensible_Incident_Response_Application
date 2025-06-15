import React, { useState } from "react";
import FormModal from '../components/modal/Modal';
import { CrudService } from '../services/CrudService';

const DeleteSensorModal = ({sensor, setSensors, showModal, setShowModal}) => {
    const [deleteSensorLoading, setDeleteSensorLoading] = useState(false);

    const deleteSensor = () => {
        // delete sensor by id
        setDeleteSensorLoading(true);
        CrudService.destroy("Sensors", sensor.sensorId).then((response) => {
            if(response.status === 200){
                // delete sensor from local data
                setSensors((previousSensors) => previousSensors.filter(
                    currentSensor => currentSensor.sensorId !== sensor.sensorId
                ));
            }
            setDeleteSensorLoading(false);
        });
    }

    return (
        <FormModal
            showModal={showModal}
            setShowModal={setShowModal}
            title="Delete sensor"
            width="40%"
            onSubmit={deleteSensor}
            loadingSubmitButton={deleteSensorLoading}
            deleteAction={true}
        >
            <p className="my-2">
                Do you want to delete the <b>{sensor.sensorName}</b> of the <b>{sensor.type}</b> type?
            </p>
        </FormModal>
    );
}
export default DeleteSensorModal;