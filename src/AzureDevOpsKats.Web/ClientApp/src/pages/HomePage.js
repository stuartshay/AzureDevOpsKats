import React from "react";
import {connect} from 'react-redux';
import PropTypes from 'prop-types';
import { getCatsList, getCatData, updateCatData, deleteCatData } from '../actions/cats';

import { MDBContainer, MDBModal, MDBModalHeader, MDBModalBody, MDBModalFooter, MDBBtn} from "mdbreact";
import "./HomePage.css";

class HomePage extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isOpenModal: false,
      cat: this.props.cat,
    }

    this.openEditDialog = this.openEditDialog.bind(this);
    this.toggleModal = this.toggleModal.bind(this);
    this.deleteCat = this.deleteCat.bind(this);
    this.changeInputValue = this.changeInputValue.bind(this);
    this.updateCatData = this.updateCatData.bind(this);
    

    
  }

  updateCatData = () => {
    this.props.updateCatData(this.state.cat);

    this.closeEditDialog();

  }

  changeInputValue(e){
    this.setState({
      cat: {
        ...this.state.cat,
        [e.target.name]: e.target.value
      }
    })
  }

  deleteCat = (id) => {
    this.props.deleteCatData(id);
  }

  toggleModal = () => {
    

    this.setState({
      isOpenModal: !this.state.isOpenModal
    });

  }
  openEditDialog = (id) => {
    this.props.getCatData(id);
    this.setState({
      isOpenModal: true
    });
  }

  closeEditDialog = () => {
    this.setState({
      isOpenModal: false
    });
  }

  componentDidMount() {
    this.props.getCatsList();
  }

  UNSAFE_componentWillReceiveProps(nextProps) {
    this.setState({
      cat: nextProps.cat
    })
  }

  renderCatsList(props) {
    return (
      <div className="row" id="cat-list">
        {
          props.catsList.map(d =>
            <div className="col-sm-12 col-md-4" key={d.id}>
              <div className="card mt-3 mb-3">
                <div className="image-wrapper">
                  <img src={d.photo.substring(1).replace("\\", "/")} alt="Card image" />
                </div>
                <div className="card-body">
                  <h4 className="card-title">{d.name}</h4>
                  <p className="card-text">{d.description}</p>
                  <button className="btn btn-success" onClick={() => this.openEditDialog(d.id)}>Edit</button>
                  <button className="btn btn-danger" onClick={() => this.deleteCat(d.id)}>Delete</button>
                </div>
              </div>
            </div>
          )
        }
      </div>
    );
  }

  render() {
    return (
      <div>
        <MDBContainer>
          {this.props.catsList.length ? this.renderCatsList(this.props) : null}
        </MDBContainer>
        <MDBModal isOpen={this.state.isOpenModal} toggle={this.toggleModal}>
          <MDBModalHeader toggle={this.toggle}>Edit Cat</MDBModalHeader>
          <MDBModalBody>
            { this.state.cat &&
              <form id="add-form">
                <div className="form-group">
                  <label htmlFor="name">Name:</label>
                  <input type="text" className="form-control" id="name" name="name"
                     value={this.state.cat.name} onChange={(e) => this.changeInputValue(e)}/>
                </div>
                <div className="form-group">
                  <label htmlFor="desc">Description:</label>
                  <input type="text" className="form-control" id="desc" name ="description" 
                  value={this.state.cat.description} onChange={e => this.changeInputValue(e)}/>
                </div>
              </form>
            }
        </MDBModalBody>
          <MDBModalFooter>
            <MDBBtn color="primary" onClick={() => this.updateCatData()}>Save changes</MDBBtn>
            <MDBBtn color="secondary" onClick={()=>this.closeEditDialog()}>Close</MDBBtn>
          </MDBModalFooter>
        </MDBModal>
      </div>
    );
  }
}

const mapStateToProps = (state) =>{
  return ({
    catsList: state.catsList.catsList,
    cat: state.catsList.cat,
    errors: state.errors
  })
}

HomePage.propTypes = {
  getCatsList: PropTypes.func.isRequired,
  getCatData: PropTypes.func.isRequired,
  updateCatData: PropTypes.func.isRequired,
  deleteCatData: PropTypes.func.isRequired,

  catsList: PropTypes.array,
  cat: PropTypes.object,
}

export default connect(mapStateToProps, { deleteCatData, getCatsList, getCatData, updateCatData })(HomePage);
