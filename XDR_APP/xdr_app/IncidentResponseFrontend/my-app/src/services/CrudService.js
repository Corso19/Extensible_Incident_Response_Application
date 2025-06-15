import axios from "axios";
import API_KEYS from "../constants/api-keys";

function read(source, id, params) {
  const url = `${API_KEYS.API_URL}/${source}/${id}`;
  return axios.get(url, {
    params: params
  });
}

function list(source, params, cancelToken) {
  const url = `${API_KEYS.API_URL}/${source}`;
  return axios.get(url, {
    params: {
      ...params,
    },
    cancelToken: cancelToken 
  });
}

function update(source, id, data) {
  const url = `${API_KEYS.API_URL}/${source}/${id}`;
  return axios.put(url, data);
}

function create(source, data) {
  const url = `${API_KEYS.API_URL}/${source}`;
  return axios.post(url, data, {
    headers: {
      'Content-Type': 'application/json'
    }
  });
}

function destroy(source, id){
  const url = `${API_KEYS.API_URL}/${source}/${id}`;
  return axios.delete(url);
}

export const CrudService = {
  create,
  update,
  read,
  list,
  destroy
};