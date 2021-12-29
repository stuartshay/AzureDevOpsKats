import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { connect, useDispatch } from 'react-redux';
import { 
  MDBNavbar, MDBNavbarBrand, MDBNavbarNav, MDBNavbarToggler, MDBCollapse, MDBNavItem, MDBIcon, 
  MDBContainer, MDBModal, MDBModalHeader, MDBModalBody, MDBModalFooter, MDBBtn 
} from "mdbreact";
import { ReactComponent as Logo } from '../assets/logo.svg';
import { withRouter } from './withRouter';
import { addCatData } from '../actions/cats';
import isEmpty from '../validation/is-empty';

const Navbar = (props) => {

    //----- states and setStates -----
    const [ collapseID, setCollapseId ] = useState("");
    const [ isOpenModal, setIsOpenModal ] = useState(false);
    const [ cat, setCat ] = useState({
      name: '',
      description: '',
      file: ''
    });
    const [ errors, setErrors ] = useState(null);

    const dispatch = useDispatch();
    const navigate = useNavigate();

    //----- Custom Functions -----
    const changeFile = (e) => {
      setCat({
        ...cat,
        file: e.target.files[0]
      });
    }

    const add_CatData = () => {
      setErrors(null);
      dispatch(addCatData(cat));
    }

    const changeInputValue = (e) => {
      setCat({
        ...cat,
        [e.target.name]: e.target.value
      });
    }

    const toggleModal = () => {
      setIsOpenModal(!isOpenModal);
      setErrors(null);
    }

    const openAddDialog = () => {
      setIsOpenModal(true);
      setErrors(null);
      setCat({
        name: '',
        description: '',
        file: '',
      });
    }

    const closeAddDialog = () => {
      setIsOpenModal(false);
      setErrors(null);
    }

    const toggleCollapse = collapse_ID =>
      setCollapseId(collapseID !== collapse_ID ? collapse_ID : "")

    const closeCollapse = collapse_ID =>
      collapseID === collapse_ID && setCollapseId("");

    const overlay = () => {
      return(
        <div
          id="sidenav-overlay"
          style={{ backgroundColor: "transparent" }}
          onClick={() => toggleCollapse("mainNavbarCollapse")}
        ></div>
      );
    }

    const guestLinks = () => {
      return (
        <MDBNavbarNav right>
          <MDBNavItem>
            <MDBBtn  color="add-color"
              className = "add-btn"
              onClick={()=>openAddDialog()}>
              <MDBIcon className="icon" icon="plus" /> Add
            </MDBBtn>
          </MDBNavItem>
        </MDBNavbarNav>
      )
    }
    
    //----- Equal to the componentWillReceiveNewProps -----    
    useEffect(() => {
      if (!isEmpty(props.errors)) {
        setErrors(props.errors);
      } else {
        setErrors(null);
        closeAddDialog();
      }
        
    },[props.errors]);
      
    return (
      <div> <h1>Hello</h1>
        <MDBNavbar className="header-position" color="header-bg" dark expand="md" fixed="top" scrolling>
          <MDBContainer>
            <MDBNavbarBrand onClick={() => navigate("")} style={{cursor: 'pointer'}}>
              <Logo  className= "name-logo"/>
              Azure DevOpsKats
            </MDBNavbarBrand>
            <MDBNavbarToggler onClick={() => toggleCollapse("mainNavbarCollapse")} />
            <MDBCollapse
              id="mainNavbarCollapse"
              isOpen={collapseID}
              navbar
            >
                {guestLinks()}
                
            </MDBCollapse>
          </MDBContainer>
        </MDBNavbar>

        {collapseID && overlay()}


        <MDBModal isOpen={isOpenModal} toggle={toggleModal}>
          <MDBModalHeader toggle={toggleModal}>Add Cat</MDBModalHeader>
          <MDBModalBody>
            {cat &&
              <form id="add-form">
                <div className="form-group">
                  <label htmlFor="name">Name:</label>
                  <input type="text" className="form-control" id="name" name="name"
                    value={cat.name} onChange={(e) => changeInputValue(e)} />
                  {!isEmpty(errors) && errors.Name && (
                    errors.Name.map((item, index) => (
                      <div className="error-msg" key={index}>{item}</div>
                    ))
                  )}
                </div>
                <div className="form-group">
                  <label htmlFor="desc">Description:</label>
                  <input type="text" className="form-control" id="desc" name="description"
                    value={cat.description} onChange={e => changeInputValue(e)} />
                  {!isEmpty(errors) && errors.Description && (
                    errors.Description.map((item, index) => (
                      <div className="error-msg" key={index}>{item}</div>
                    ))
                  )}
                </div>
                <div className="form-group">
                  <label htmlFor="file">Upload Image</label>
                  <input className="form-control" type="file" id="file" name="file" onChange={e => changeFile(e)} />
                  {!isEmpty(errors) && errors.image && (
                    errors.image.map((item, index) => (
                      <div className="error-msg" key={index}>{item}</div>
                    ))
                  )}
                </div>
              </form>
            }
          </MDBModalBody>
          <MDBModalFooter>
            <MDBBtn color="primary" onClick={add_CatData}><MDBIcon far icon="save" />Save changes</MDBBtn>
            <MDBBtn color="secondary" onClick={closeAddDialog}><MDBIcon far icon="window-close" /> Close</MDBBtn>
          </MDBModalFooter>
        </MDBModal>
      </div>
    )
}

//----- mapStateToProps -----
const mapStateToProps = (state) => ({
  errors: state.errors.errors
})

export default connect(mapStateToProps)(withRouter(Navbar));
