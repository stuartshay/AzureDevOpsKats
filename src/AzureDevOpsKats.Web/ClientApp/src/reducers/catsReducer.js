import { RECEIVED_CATS_LIST, RECEIVED_CAT_DATA } from '../actions/types';

const initialState = {
  catsList: [],
  cat: null,
}

const catsReducer = (state = initialState, action) => {
  
  switch(action.type) {
    case RECEIVED_CATS_LIST:
      return {
        ...state,
        catsList: action.catsList
      }
    case RECEIVED_CAT_DATA:
      return {
        ...state,
        cat: action.catData
      }
    default:
      return state;
  }
}

export default catsReducer;
