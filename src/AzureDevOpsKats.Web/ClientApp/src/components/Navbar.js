import React, { Component } from 'react';
import { MDBNavbar, MDBNavbarBrand, MDBNavbarNav, MDBNavbarToggler, MDBCollapse, MDBNavItem, MDBIcon, MDBContainer, MDBModal, MDBModalHeader, MDBModalBody, MDBModalFooter, MDBBtn } from "mdbreact";
import { ReactComponent as Logo } from '../assets/logo.svg';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import {addCatData} from '../actions/cats';
import isEmpty from '../validation/is-empty';

class Navbar extends Component {
  constructor(props) {
    super(props);
    this.state = {
      collapseID: "",
      isOpenModal: false,
      cat: {
        name: '',
        description: '',
        file: '',
      },
      errors: null,
    }

    this.openAddDialog = this.openAddDialog.bind(this);
    this.toggleModal = this.toggleModal.bind(this);
    this.changeInputValue = this.changeInputValue.bind(this);
    this.addCatData = this.addCatData.bind(this);
    this.changeFile = this.changeFile.bind(this);

  }

  changeFile = (e) => {

    // let reader = new FileReader();
    // let bytes = [];
    // reader.onload = function () {
    //   let arrayBuffer = this.result;
    //   bytes = new Uint8Array(arrayBuffer);
    //   // binaryString = String.fromCharCode.apply(null, array);
    // }
    // reader.readAsArrayBuffer(e.target.files[0]);

    this.setState({
      cat: {
        ...this.state.cat,
        file: e.target.files[0]
      }
    })
  }

  addCatData = () => {
    this.setState({
      errors: null
    });
    this.props.addCatData(this.state.cat);
  }

  changeInputValue(e) {
    this.setState({
      cat: {
        ...this.state.cat,
        [e.target.name]: e.target.value
      }
    })
  }

  toggleModal = () => {
    this.setState({
      isOpenModal: !this.state.isOpenModal,
      errors: null
    });

  }
  openAddDialog = () => {
    this.setState({
      isOpenModal: true,
      errors: null,
      cat: {
        name: '',
        description: '',
        file: '',
      }
    });
  }

  closeAddDialog = () => {
    this.setState({
      isOpenModal: false,
      errors: null
    });
  }

  UNSAFE_componentWillReceiveProps(nextProps) {
    if (!isEmpty(nextProps.errors)) {
      this.setState({
        errors: nextProps.errors
      });
    } else {
      this.setState({
        errors: null,
      })
      this.closeAddDialog();
    }
  }

  toggleCollapse = collapseID => () =>
    this.setState(prevState => ({
      collapseID: prevState.collapseID !== collapseID ? collapseID : ""
    }));

  closeCollapse = collapseID => () =>
    this.state.collapseID === collapseID && this.setState({ collapseID: "" });

  render() {

    const overlay = (
      <div
        id="sidenav-overlay"
        style={{ backgroundColor: "transparent" }}
        onClick={this.toggleCollapse("mainNavbarCollapse")}
      />
    );

    const { collapseID } = this.state;

    const guestLinks = (
      <MDBNavbarNav right>
        <MDBNavItem>
          <MDBBtn color="primary"
            onClick={()=>this.openAddDialog()}
          >
            <MDBIcon icon="plus" /> Add
          </MDBBtn>
        </MDBNavItem>
      </MDBNavbarNav>
    );
    
    return (
      <div>
        <MDBNavbar color="indigo" dark expand="md" fixed="top" scrolling>
          <MDBContainer>
            <MDBNavbarBrand href="/">
              <Logo style={{ height: '2.5rem', width: "2.5rem" }} />
              Azure DevOpsKats
            </MDBNavbarBrand>
            <MDBNavbarToggler onClick={this.toggleCollapse("mainNavbarCollapse")} />
            <MDBCollapse
              id="mainNavbarCollapse"
              isOpen={this.state.collapseID}
              navbar
            >
                {guestLinks}
                
            </MDBCollapse>
          </MDBContainer>
        </MDBNavbar>

        {collapseID && overlay}


        <MDBModal isOpen={this.state.isOpenModal} toggle={this.toggleModal}>
          <MDBModalHeader toggle={this.toggle}>Add Cat</MDBModalHeader>
          <MDBModalBody>
            {this.state.cat &&
              <form id="add-form">
                <div className="form-group">
                  <label htmlFor="name">Name:</label>
                  <input type="text" className="form-control" id="name" name="name"
                    value={this.state.cat.name} onChange={(e) => this.changeInputValue(e)} />
                  {!isEmpty(this.state.errors) && this.state.errors.Name && (
                    this.state.errors.Name.map((item, index) => (
                      <div className="error-msg" key={index}>{item}</div>
                    ))
                  )}
                </div>
                <div className="form-group">
                  <label htmlFor="desc">Description:</label>
                  <input type="text" className="form-control" id="desc" name="description"
                    value={this.state.cat.description} onChange={e => this.changeInputValue(e)} />
                  {!isEmpty(this.state.errors) && this.state.errors.Description && (
                    this.state.errors.Description.map((item, index) => (
                      <div className="error-msg" key={index}>{item}</div>
                    ))
                  )}
                </div>
                <div className="form-group">
                  <label htmlFor="file">Upload Image</label>
                  <input className="form-control" type="file" id="file" name="file" onChange={e => this.changeFile(e)} />
                  {!isEmpty(this.state.errors) && this.state.errors.image && (
                    this.state.errors.image.map((item, index) => (
                      <div className="error-msg" key={index}>{item}</div>
                    ))
                  )}
                </div>
              </form>
            }
          </MDBModalBody>
          <MDBModalFooter>
            <MDBBtn color="primary" onClick={() => this.addCatData()}><MDBIcon far icon="save" />Save changes</MDBBtn>
            <MDBBtn color="secondary" onClick={() => this.closeAddDialog()}><MDBIcon far icon="window-close" /> Close</MDBBtn>
          </MDBModalFooter>
        </MDBModal>
      </div>
    )
  }
}

Navbar.propTypes = {
  addCatData: PropTypes.func.isRequired,
  errors: PropTypes.object,
}

const mapStateToProps = (state) => ({
  errors: state.errors.errors
})

export default connect(mapStateToProps, {addCatData})(withRouter(Navbar));
