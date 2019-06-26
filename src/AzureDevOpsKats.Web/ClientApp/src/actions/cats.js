import axios from 'axios';
import { GET_ERRORS, RECEIVED_CATS_LIST, RECEIVED_CAT_DATA, REFRESH_LIST } from './types';

export const deleteCatData= (id) => dispatch => {
  axios.delete(`/api/v1/Cats/${id}`)
    .then(res => {
      dispatch(refreshList());
    })
    .catch(err => {
      dispatch({
        type: GET_ERRORS,
        payload: err.response.data
      });
    });

}
export const addCatData = (cat) => dispatch => {
  const formData = new FormData();
  formData.append('file', cat.file);
  formData.append('name', cat.name);
  formData.append('description', cat.description);

  axios.post('/api/v2/Cats', formData)
    .then(res => {
      dispatch(refreshList());
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

export const getCats = (limit, page) => dispatch => {
  axios.get(`/api/v2/Cats/${limit}/${page}`)
    .then(res => {
      axios.get('/api/v2/Cats/results/total').then(count => {
        dispatch(receivedCatsList(res.data, count.data));
      })      
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
      dispatch(receivedCatsList(res.data, 0));
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
      dispatch(refreshList());
    })
    .catch(err => {
      dispatch({
        type: GET_ERRORS,
        payload: err.response.data
      });
    });
}

// ==========================================================

export const receivedCatsList = (catsList,count) => {
  return {
    type: RECEIVED_CATS_LIST,
    catsList: catsList,
    count: count
  }
}

export const receivedCatData = catData => {
  return {
    type: RECEIVED_CAT_DATA,
    catData: catData
  }
}

export const refreshList = () => {
  return {
    type: REFRESH_LIST
  }
}