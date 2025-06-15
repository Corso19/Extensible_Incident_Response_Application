import React, { useEffect, useState } from "react";
import { ButtonGroup, ToggleButton } from "react-bootstrap";
import UpdateOrchestratorStateModal from "./OrchestratorModal";
import { OrchestratorService } from "../services/OrchestratorService";

const Orchestrator = () => {

    // const [orchestratorState, setOrchestratorState] = useState(null);
    // const [showUpdateOrchestratorModal, setShowUpdateOrchestratorModal] = useState(false);
    // const orchestratorRadios = [
    //     { name: 'Orchestrator running', value: "start" },
    //     { name: 'Orchestrator stopped', value: "stop" },
    // ];

    useEffect(() => {
        // get sensors
        // OrchestratorService.getOrchestratorStatus().then((response) => {
        //     if (response.status === 200) {
        //         const current_orchestrator_status = (response.data.isRunning) ? "start" : "stop";
        //         setOrchestratorState(current_orchestrator_status);
        //     }
        // });
        // OrchestratorService.startOrchestrator().then((response) => {
        //     if(response.status === 200){
        //         console.log("Orchestrator started successfully!");
        //     }
        // });
    }, []);

    return(
        <React.Fragment>
            {/* <ButtonGroup className="d-flex">
                {orchestratorRadios.map((radio, index) => (
                    <ToggleButton
                        key={index}
                        id={`radio-${index}`}
                        type="radio"
                        title={index % 2 ? 'Stop the orchestrator' : 'Start the orchestrator'}
                        variant={index % 2 ? 'outline-danger' : 'outline-info'}
                        name="radio"
                        value={radio.value}
                        checked={orchestratorState === radio.value}
                        onChange={() => setShowUpdateOrchestratorModal(true)}
                    >
                        {radio.name}
                    </ToggleButton>
                ))}
            </ButtonGroup>
            <UpdateOrchestratorStateModal 
                orchestratorState={orchestratorState}
                setOrchestratorState={setOrchestratorState}
                showModal={showUpdateOrchestratorModal}
                setShowModal={setShowUpdateOrchestratorModal}
            /> */}
        </React.Fragment>
    );
}

export default Orchestrator;