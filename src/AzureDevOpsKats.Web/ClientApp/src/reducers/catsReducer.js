import { RECEIVED_CATS_LIST, RECEIVED_CAT_DATA, REFRESH_LIST } from '../actions/types';

const initialState = {
  catsList: [],
  count: 0,
  cat: null,
  refreshFlag: false
}

const catsReducer = (state = initialState, action) => {
  
  switch(action.type) {
    case RECEIVED_CATS_LIST:
      return {
        ...state,
        refreshFlag: false,
        catsList: action.catsList,
        count: action.count
      }
    case RECEIVED_CAT_DATA:
      return {
        ...state,
        cat: action.catData
      }
    case REFRESH_LIST:
      return {
        ...state,
        refreshFlag: true
      }
    default:
      return state;
  }
}

export default catsReducer;
