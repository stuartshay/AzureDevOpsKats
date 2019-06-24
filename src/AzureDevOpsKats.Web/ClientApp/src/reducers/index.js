import { combineReducers} from 'redux';
import errorReducer from './errorReducer';
import catsReducer from './catsReducer';

const catsApp = combineReducers ({
  errors: errorReducer,
  catsList: catsReducer,
})

export default catsApp;