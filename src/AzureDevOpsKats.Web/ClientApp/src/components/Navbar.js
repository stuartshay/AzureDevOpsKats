import React, { Component } from 'react';
import { MDBNavbar, MDBNavbarBrand, MDBNavbarNav, MDBNavbarToggler, MDBCollapse, MDBNavItem, MDBNavLink, MDBContainer, MDBModal, MDBModalHeader, MDBModalBody, MDBModalFooter, MDBBtn } from "mdbreact";
import { ReactComponent as Logo } from '../assets/logo.svg';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import {addCatData} from '../actions/cats';

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
      }
    }

    this.openAddDialog = this.openAddDialog.bind(this);
    this.toggleModal = this.toggleModal.bind(this);
    this.changeInputValue = this.changeInputValue.bind(this);
    this.addCatData = this.addCatData.bind(this);
    this.changeFile = this.changeFile.bind(this);

  }

  changeFile = (e) => {

    let reader = new FileReader();
    let bytes = [];
    reader.onload = function () {
      let arrayBuffer = this.result;
      bytes = new Uint8Array(arrayBuffer);
      // binaryString = String.fromCharCode.apply(null, array);
    }
    reader.readAsArrayBuffer(e.target.files[0]);

    this.setState({
      cat: {
        ...this.state.cat,
        file: e.target.files[0]
      }
    })
  }

  addCatData = () => {
    this.props.addCatData(this.state.cat);
    this.closeAddDialog();

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
      isOpenModal: !this.state.isOpenModal
    });

  }
  openAddDialog = () => {
    this.setState({
      isOpenModal: true,
      cat: {
        name: '',
        description: '',
        file: '',
      }
    });
  }

  closeAddDialog = () => {
    this.setState({
      isOpenModal: false
    });
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
            Add
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
              Cats Database
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
                </div>
                <div className="form-group">
                  <label htmlFor="desc">Description:</label>
                  <input type="text" className="form-control" id="desc" name="description"
                    value={this.state.cat.description} onChange={e => this.changeInputValue(e)} />
                </div>
                <div className="form-group">
                  <label htmlFor="file">Upload Image</label>
                  <input className="form-control" type="file" id="file" name="file" onChange={e => this.changeFile(e)} />
                </div>
              </form>
            }
          </MDBModalBody>
          <MDBModalFooter>
            <MDBBtn color="primary" onClick={() => this.addCatData()}>Save changes</MDBBtn>
            <MDBBtn color="secondary" onClick={() => this.closeAddDialog()}>Close</MDBBtn>
          </MDBModalFooter>
        </MDBModal>
      </div>
    )
  }
}

Navbar.propTypes = {
  addCatData: PropTypes.func.isRequired
}

const mapStateToProps = (state) => ({
})

export default connect(mapStateToProps, {addCatData})(withRouter(Navbar));