import axios from "axios";
import API_KEYS from "../constants/api-keys";
const base_url = `${API_KEYS.API_URL}`;

function startOrchestrator(){
    const url = base_url + `/Sensors/start-orchestrator`;
    return axios.post(url);
}

// function stopOrchestrator(){
//     const url = base_url + `/Sensors/stop-orchestrator`;
//     return axios.post(url);
// }

// function getOrchestratorStatus(){
//     const url = base_url + `/Sensors/orchestrator-status`;
//     return axios.get(url);
// }

export const OrchestratorService = {
    startOrchestrator
    // stopOrchestrator,
    // getOrchestratorStatus
}