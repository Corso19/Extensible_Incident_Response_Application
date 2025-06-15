import React, { useState } from "react";
import FormModal from '../components/modal/Modal';
import { OrchestratorService } from "../services/OrchestratorService";

const UpdateOrchestratorStateModal = ({orchestratorState, setOrchestratorState, showModal, setShowModal}) => {
    const [updateOrchestratorStateLoading, setUpdateOrchestratorLoading] = useState(false);
    const ORCHESTRATOR_IS_RUNNING = "start";
    const ORCHESTRATOR_IS_STOPPED = "stop";

    const updateOrchestratorState = () => {
        // update orchestrator state
        setUpdateOrchestratorLoading(true);
        if(orchestratorState === ORCHESTRATOR_IS_RUNNING){
            // stop orchestrator
            OrchestratorService.stopOrchestrator().then((response) => {
                if(response.status === 200){
                    setOrchestratorState(ORCHESTRATOR_IS_STOPPED);
                }
                setUpdateOrchestratorLoading(false);
                setShowModal(false);
            });
        }
        else{
            // start orchestrator
            OrchestratorService.startOrchestrator().then((response) => {
                if(response.status === 200){
                    setOrchestratorState(ORCHESTRATOR_IS_RUNNING);
                }
                setUpdateOrchestratorLoading(false);
                setShowModal(false);
            });
        }
    }

    return (
        <FormModal
            showModal={showModal}
            setShowModal={setShowModal}
            title={orchestratorState === ORCHESTRATOR_IS_RUNNING ? "Stop orchestrator" : "Start orchestrator"}
            width="40%"
            onSubmit={updateOrchestratorState}
            loadingSubmitButton={updateOrchestratorStateLoading}
        >
            <p className="my-2">
                Do you want to <b>{orchestratorState === ORCHESTRATOR_IS_RUNNING ? "stop" : "start"}</b> the orchestrator ?
            </p>
        </FormModal>
    );
}
export default UpdateOrchestratorStateModal;