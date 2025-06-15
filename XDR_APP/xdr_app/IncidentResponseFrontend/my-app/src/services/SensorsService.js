import axios from "axios";
import API_KEYS from "../constants/api-keys";
const base_url = `${API_KEYS.API_URL}`;

function updateIsEnabled(sensor_id){
    const url = base_url + `/Sensors/` + sensor_id + `/set-enabled`;
    return axios.put(url);
}

export const SensorsService = {
    updateIsEnabled
}