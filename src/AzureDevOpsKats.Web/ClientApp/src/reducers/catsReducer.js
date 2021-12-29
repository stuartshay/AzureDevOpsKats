import { RECEIVED_CATS_LIST, RECEIVED_CAT_DATA, REFRESH_LIST, ADDED_ITEM, UPDATED_ITEM, DELETED_ITEM } from '../actions/types';

const initialState = {
  catsList: [],
  count: 0,
  cat: null,
  refreshFlag: false,
  action: null,
}

const catsReducer = (state = initialState, action) => {  
  switch(action.type) {
    case RECEIVED_CATS_LIST:
      return {
        ...state,
        refreshFlag: false,
        catsList: action.catsList,
        count: action.count,
        action: null
      }
    case RECEIVED_CAT_DATA:
      return {
        ...state,
        cat: action.catData,
        action: null
      }
    case REFRESH_LIST:
      return {
        ...state,
        action: null,
        refreshFlag: true
      }
    case ADDED_ITEM:
    case UPDATED_ITEM:
    case DELETED_ITEM:
    return {
      ...state,
      action: action.type
    };
    default:
      return {
        ...state,
        action: null
      };
  }
}

export default catsReducer;
