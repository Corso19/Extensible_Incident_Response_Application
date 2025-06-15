import React, {useState} from "react";
import { PencilFill, TrashFill } from 'react-bootstrap-icons';
import IconButton from "../components/buttons/IconButton";
import AddUpdateSensorModal from "./AddUpdateSensorModal";
import ToggleButtonCustom from "../components/buttons/ToggleButtonCustom";
import DeleteSensorModal from "./DeleteSensorModal";
import { SensorsService } from "../services/SensorsService";
import { Spinner } from "react-bootstrap";

const SensorRow = ({sensor, setSensors}) => {
    const [showDeleteSensorModal, setShowDeleteSensorModal] = useState(false);
    const [showUpdateSensorModal, setShowUpdateSensorModal] = useState(false);
    const [isEnabledLoading, setIsEnabledLoading] = useState(false);

    const handleIsEnabledChanged = () => {
        setIsEnabledLoading(true);
        SensorsService.updateIsEnabled(sensor.sensorId).then((response) => {
            if (response.status === 200){
                // update sensor local data
                const updatedIsEnabled = !sensor.isEnabled;
                setSensors(previousSensors => 
                    previousSensors.map(current_sensor => 
                        current_sensor.sensorId === sensor.sensorId
                            ? { ...sensor, isEnabled: updatedIsEnabled }
                            : current_sensor
                    )
                );
            }
            setIsEnabledLoading(false);
        });
    }

    return(
        <tr>
            <td className="align-middle">
                {sensor.sensorName}
            </td>
            <td className="align-middle">
                {sensor.type}
            </td>
            <td className="text-center">
            {
                isEnabledLoading ? (
                    <Spinner
                        as="span"
                        variant="primary"
                        size="sm"
                        role="status"
                        aria-hidden="true"
                        animation="border"
                    />
                ):(
                    <ToggleButtonCustom
                        index={sensor.sensorId}
                        value={sensor.isEnabled}
                        setValue={handleIsEnabledChanged}
                    />
                )
            }
            </td>
            <td className="align-middle">
                <div className="d-flex align-items-center justify-content-evenly">
                    <PencilFill
                        title="Edit sensor"
                        size={18}
                        className="clickable color-primary-hover"
                        cursor="pointer"
                        onClick={() => {setShowUpdateSensorModal(true)}}
                    />
                    <AddUpdateSensorModal 
                        sensor={sensor}
                        setSensors={setSensors}
                        showModal={showUpdateSensorModal}
                        setShowModal={setShowUpdateSensorModal}
                    />
                    <IconButton
                        icon={TrashFill}
                        title="Delete sensor"
                        linkClassName="text-danger"
                        iconSize={18}
                        variant="danger"
                        onClick={() => setShowDeleteSensorModal(true)}
                    />
                    <DeleteSensorModal 
                        sensor={sensor}
                        setSensors={setSensors}
                        showModal = {showDeleteSensorModal}
                        setShowModal ={setShowDeleteSensorModal}
                    />
                </div>
            </td>
        </tr>
    );
}

export default SensorRow;