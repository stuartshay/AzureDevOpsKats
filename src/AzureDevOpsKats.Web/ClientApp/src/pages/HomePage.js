import React , { useState, useEffect } from "react";
import { connect, useDispatch } from 'react-redux';
import { getCats, getCatData, updateCatData, deleteCatData } from '../actions/cats';
import { 
  MDBContainer, MDBModal, MDBModalHeader, MDBModalBody, MDBModalFooter, 
  MDBRow, MDBCol, MDBPagination, MDBPageItem, MDBPageNav, MDBIcon, MDBBtn
} from "mdbreact";
import isEmpty from "../validation/is-empty";
import { ADDED_ITEM, UPDATED_ITEM, DELETED_ITEM } from "../actions/types";
import { toast } from "react-toastify";
import $ from 'jquery';
import "./HomePage.css";

const HomePage = (props) => {

  //----- states and setStates -----
  const [isOpenModal, setIsOpenModal] = useState(false);
  const [cat, setCat] = useState(props.cat);
  const [cat_counts, setCat_counts] = useState(30);
  const [cat_counts_per_page, setCat_counts_per_page] = useState(9);
  const [current_page, setCurrent_page] = useState(0);
  const [errors, setErrors] = useState(null);

  const dispatch = useDispatch();

  //----- custom functions -----
  const pageCounts = () => {
    return parseInt((cat_counts - 1) / cat_counts_per_page) + 1
  }

  const clickNextPage = () => {
    const page = current_page < pageCounts()-1 ? current_page + 1 : current_page;
    
    setCurrent_page (page);

    dispatch(getCats(cat_counts_per_page, page));
      scrollTop();
  }

  const clickFirstPage = () => {
    clickPage(0);
      scrollTop();
  }

  const clickLastPage = () => {
    clickPage(pageCounts() - 1);
      scrollTop();
  }

  const clickPrevPage = () => {
    const page = current_page > 0 ? current_page - 1 : 0;
    setCurrent_page(page);
    dispatch(getCats(cat_counts_per_page, page));
      scrollTop();
  }

  const clickPage = (no) => {
    setCurrent_page(no);
    dispatch(getCats(cat_counts_per_page, no));
    scrollTop();
  }

  const scrollTop = () => {
      $('html, body').animate({scrollTop: 0}, 800);
  }

  const update_CatData = () => {
    setErrors(null);
    dispatch(updateCatData(cat));
  }

  const changeInputValue = (e) => {
    if(cat[e.target.name].length < 40)
    setCat({
      ...cat,
      [e.target.name]: e.target.value
    });
  }

  const deleteCat = (id) => {
    dispatch(deleteCatData(id));
  }

  const toggleModal = () => {
    setIsOpenModal(!isOpenModal);
    setErrors(null);
  }

  const openEditDialog = (id) => {
    dispatch(getCatData(id));
    setIsOpenModal(true);
    setErrors(null);
  }

  const closeEditDialog = () => {
    setIsOpenModal(false);
    setErrors(null);
  }    

  const renderPagination = () => {
    const items = [];
    let start = 0;
    let end = pageCounts();
    if(pageCounts()>5){
      if(current_page-2>0){
        start = current_page-2;
        end = start + 5;
        if(end > pageCounts()){
          end = pageCounts();
          start = end - 5;
        }
      } else {
        start = 0; end = 5;
      }
    }

    for (let page_no = start; page_no < end; page_no++) {
      items.push(
        <MDBPageItem active={page_no === current_page} key={page_no} onClick={()=>clickPage(page_no)}>
          <MDBPageNav >
            {page_no+1}
            {/* {page_no === current_page && <span className="sr-only">(current)</span> } */}
          </MDBPageNav>
        </MDBPageItem>
      )

    }
      return (
          <MDBRow>
              <MDBCol>
                  <MDBPagination className="d-flex justify-content-center">
                      <MDBPageItem onClick={() => clickFirstPage()} disabled={0 === current_page}>
                          <MDBPageNav aria-label="Previous">
                           <MDBIcon icon="angle-double-left" />
                       </MDBPageNav>
                      </MDBPageItem>
                      <MDBPageItem onClick={() => clickPrevPage()} disabled={0 === current_page}>
                          <MDBPageNav aria-label="Previous">
                            <MDBIcon icon="angle-left" />
                          </MDBPageNav>
                      </MDBPageItem>
                      {items}
                      <MDBPageItem onClick={() => clickNextPage()} disabled={pageCounts() - 1 === current_page}>
                          <MDBPageNav aria-label="Next">
                           <MDBIcon icon="angle-right" />
                          </MDBPageNav>
                      </MDBPageItem>
                      <MDBPageItem onClick={() => clickLastPage()} disabled={pageCounts() - 1 === current_page}>
                          <MDBPageNav aria-label="Previous">
                          <MDBIcon icon="angle-double-right" />
                          </MDBPageNav>
                      </MDBPageItem>
                  </MDBPagination>
              </MDBCol>
          </MDBRow>
      )
  }

  const renderCatsList = (props) => {
    return (
      <div className="row" id="cat-list">

        {
          props.catsList.map(d =>
            <div className="container-items col-sm-12 col-md-4" key={d.id}>
              <div className="card mt-3 mb-3">
                <div className="image-wrapper">
                  <img src={d.photo.substring(1).replace("\\", "/")} alt="Cat Photo" />
                </div>
                <div className="card-body">
                  <h4 className="card-title">{d.name}</h4>
                  <p className="card-text">{d.description}</p>
                <div className="btn-block">
                  <button className="btn btn-success" onClick={() => openEditDialog(d.id)}>Edit</button>
                  <button className="btn btn-danger" onClick={() => deleteCat(d.id)}> Delete</button>
                </div>
                </div>
              </div>
            </div>
          )
        }
      </div>
    );
  }

  //----- equal to the componentWillMount -----
  useEffect(() => {
    dispatch(getCats(cat_counts_per_page, current_page));
  },[]);

  //----- equal to the componentWillReceiveNewProps -----
  useEffect(() => {
    if (props.action) {
      if (props.action === ADDED_ITEM) {
        toast("Added an item successfully!", { type: toast.TYPE.INFO});
      }
      if (props.action === UPDATED_ITEM) {
        toast("Updated an item successfully!", { type: toast.TYPE.SUCCESS });
        closeEditDialog();
      }
      if (props.action === DELETED_ITEM) {
        toast("Deleted an item successfully!", { type: toast.TYPE.WARNING });
      }
    }
    if(!isEmpty(props.errors)){
      setErrors(props.errors)
    } else {
      setErrors(null);
      setCat(props.cat);
      setCat_counts(props.count);
      console.log(current_page, pageCounts());
      if (current_page >= pageCounts() ) {
        clickLastPage();
        return;
      }
      if(props.refreshFlag){
        dispatch(getCats(cat_counts_per_page, current_page));
      }
    }
  },[props]);

  return (
      <div className="cat-main-container">
        <div className="abs-bg absolute-bg-1"> </div>
        <div className="abs-bg absolute-bg-2"> </div>
        <div className="abs-bg absolute-bg-3"> </div>
        <MDBContainer className="p-3">
          {props.catsList.length ? renderCatsList(props) : null}
          {renderPagination()}
        </MDBContainer>
        <MDBModal isOpen={isOpenModal} toggle={toggleModal}>
          <MDBModalHeader toggle={toggleModal}>Edit Cat</MDBModalHeader>
          <MDBModalBody>
            { cat &&
              <form id="add-form">
                <div className="form-group">
                  <label htmlFor="name">Name:</label>
                  <input type="text" className="form-control" id="name" name="name"
                  value={cat.name} onChange={(e) => changeInputValue(e)}/>
                  { !isEmpty(errors) && errors.Name && (
                      errors.Name.map((item, index)=>(
                          <div className="error-msg" key={index}>{item}</div>
                  ))
                  )}
                </div>
                <div className="form-group">
                  <label htmlFor="desc">Description:</label>
                  <input type="text" className="form-control" id="desc" name ="description"
                  value={cat.description} onChange={e => changeInputValue(e)}/>
                  {!isEmpty(errors) && errors.Description && (
                      errors.Description.map((item, index) => (
                          <div className="error-msg" key={index}>{item}</div>
                  ))
                  )}
                </div>
              </form>
            }
          </MDBModalBody>
          <MDBModalFooter>
            <MDBBtn color="primary" onClick={() => update_CatData()}><MDBIcon far icon="save" /> Save changes</MDBBtn>
            <MDBBtn color="secondary" onClick={() => closeEditDialog()}><MDBIcon far icon="window-close" /> Close</MDBBtn>
          </MDBModalFooter>
        </MDBModal>
      </div>
    );
}

//----- mapStateToProps -----
const mapStateToProps = (state) =>{
  return ({
    action: state.catsList.action,
    catsList: state.catsList.catsList,
    count: state.catsList.count,
    cat: state.catsList.cat,
    refreshFlag: state.catsList.refreshFlag,
    errors: state.errors.errors
  })
}

export default connect(mapStateToProps)(HomePage);