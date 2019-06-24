import axios from 'axios';
import { GET_ERRORS, RECEIVED_CATS_LIST, RECEIVED_CAT_DATA, ADD_CAT_DATA } from './types';

export const deleteCatData= (id) => dispatch => {
  axios.delete(`/api/v1/Cats/${id}`)
    .then(res => {
      dispatch(getCatsList());
    })
    .catch(err => {
      dispatch({
        type: GET_ERRORS,
        payload: err.response.data
      });
    });

}
export const addCatData = (cat) => dispatch => {
  console.log(cat);
  // const formData = new FormData();
  // formData.append('file', cat.file);
  // formData.append('name', cat.name);
  // formData.append('description', cat.description);
  // console.log(formData);
  // const config = {
  //   headers: {
  //     'content-type': 'multipart/form-data'
  //   }
  // };

  // axios.post('/api/v1/Cats', formData, config)
  axios.post('/api/v1/Cats', cat, {
    onUploadProgress: progressEvent => {
      console.log(progressEvent.loaded / progressEvent.total);
    }
  }).then(res => {
      dispatch(receivedCatData(res.data));
    })
    .catch(err => {
      dispatch({
        type: GET_ERRORS,
        payload: err.response.data
      });
    });
}

export const getCatData = (id) => dispatch => {
  axios.get(`/api/v1/Cats/${id}`)
    .then(res => {
      dispatch(receivedCatData(res.data));
    })
    .catch(err => {
      dispatch({
        type: GET_ERRORS,
        payload: err.response.data
      });
    });
}

export const getCatsList = () => dispatch => {
  axios.get('/api/v1/Cats')
    .then(res => {
      dispatch(receivedCatsList(res.data));
    })
    .catch(err => {
      dispatch({
        type: GET_ERRORS,
        payload: err.response.data
      });
    });
}

export const updateCatData = (cat) => dispatch => {
  axios.put(`/api/v1/Cats/${cat.id}`, {
    name: cat.name,
    description: cat.description

  })
    .then(res => {
      console.log(res.data);
      dispatch(getCatsList());
    })
    .catch(err => {
      dispatch({
        type: GET_ERRORS,
        payload: err.response.data
      });
    });
}

// ==========================================================

export const receivedCatsList = catsList => {
  return {
    type: RECEIVED_CATS_LIST,
    catsList: catsList
  }
}

export const receivedCatData = catData => {
  return {
    type: RECEIVED_CAT_DATA,
    catData: catData
  }
}
