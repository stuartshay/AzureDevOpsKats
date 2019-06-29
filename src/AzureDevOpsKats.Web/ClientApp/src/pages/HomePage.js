import React from "react";
import {connect} from 'react-redux';
import PropTypes from 'prop-types';
import { getCats, getCatsList, getCatData, updateCatData, deleteCatData } from '../actions/cats';

import { MDBContainer, MDBModal, MDBModalHeader, MDBModalBody, MDBModalFooter, MDBBtn,
  MDBRow, MDBCol, MDBPagination, MDBPageItem, MDBPageNav
} from "mdbreact";
import "./HomePage.css";
import isEmpty from "../validation/is-empty";

class HomePage extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isOpenModal: false,
      cat: this.props.cat,
      cat_counts: 30,
      cat_counts_per_page: 6,
      current_page: 0,
      errors: null,
    }

    this.openEditDialog = this.openEditDialog.bind(this);
    this.toggleModal = this.toggleModal.bind(this);
    this.deleteCat = this.deleteCat.bind(this);
    this.changeInputValue = this.changeInputValue.bind(this);
    this.updateCatData = this.updateCatData.bind(this);

    this.clickPage = this.clickPage.bind(this);
    this.clickPrevPage = this.clickPrevPage.bind(this);
    this.clickNextPage = this.clickNextPage.bind(this);
  }
  pageCounts(){
    return parseInt((this.state.cat_counts - 1) / this.state.cat_counts_per_page) + 1
  }
  clickNextPage() {
    const page = this.state.current_page < this.pageCounts()-1 ? this.state.current_page + 1 : this.state.current_page;
    this.setState({
      current_page: page
    });

    this.props.getCats(this.state.cat_counts_per_page, page);
  }
  clickPrevPage(){

    const page = this.state.current_page > 0 ? this.state.current_page - 1 : 0;
    this.setState({
      current_page: page
    })
    this.props.getCats(this.state.cat_counts_per_page, page);
  }
  clickPage(no){
    this.setState({
      current_page:no
    })
    this.props.getCats(this.state.cat_counts_per_page, no);
  }

  updateCatData = () => {
    this.setState({
      errors: null
    });
    this.props.updateCatData(this.state.cat);
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
    this.props.getCats(this.state.cat_counts_per_page, this.state.current_page);
  }

  UNSAFE_componentWillReceiveProps(nextProps) {
    console.log(nextProps.errors);
    if(!isEmpty(nextProps.errors)){
      console.log("errors");
      this.setState({
        errors: nextProps.errors
      });
    } else { 
      console.log("ok");
      this.setState({
        errors: null,
        cat: nextProps.cat,
        cat_counts: nextProps.count
      })
      if(nextProps.refreshFlag){
        this.props.getCats(this.state.cat_counts_per_page, this.state.current_page);
        this.closeEditDialog();
      }
    }
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

  renderPagination() {
    const items = [];

    for (let page_no = 0; page_no < this.pageCounts(); page_no++) {
      items.push(
        <MDBPageItem active={page_no === this.state.current_page} key={page_no} onClick={()=>this.clickPage(page_no)}>
          <MDBPageNav >
            {page_no+1} 
            {page_no === this.state.current_page && <span className="sr-only">(current)</span> }
          </MDBPageNav>
        </MDBPageItem>
      )

    }
    return (
      <MDBRow>
        <MDBCol>
          <MDBPagination className="d-flex justify-content-end">
            <MDBPageItem onClick={()=>this.clickPrevPage()}>
              <MDBPageNav aria-label="Previous">
                <span aria-hidden="true">Previous</span>
              </MDBPageNav>
            </MDBPageItem>
            {items}
            <MDBPageItem onClick={()=>this.clickNextPage()}>
              <MDBPageNav aria-label="Next">
                <span aria-hidden="true">Next</span>
              </MDBPageNav>
            </MDBPageItem>
          </MDBPagination>
        </MDBCol>
      </MDBRow>
    )

  }

  render() {
    return (
      <div>
        <MDBContainer className="pt-5">
          {this.renderPagination()}
          {this.props.catsList.length ? this.renderCatsList(this.props) : null}
          {this.renderPagination()}
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
                  { !isEmpty(this.state.errors) && this.state.errors.Name && (
                    this.state.errors.Name.map((item, index)=>(
                      <div className="error-msg" key={index}>{item}</div>
                    ))
                  )}
                </div>
                <div className="form-group">
                  <label htmlFor="desc">Description:</label>
                  <input type="text" className="form-control" id="desc" name ="description" 
                  value={this.state.cat.description} onChange={e => this.changeInputValue(e)}/>
                  {!isEmpty(this.state.errors) && this.state.errors.Description && (
                    this.state.errors.Description.map((item, index) => (
                      <div className="error-msg" key={index}>{item}</div>
                    ))
                  )}
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
  console.log(state);
  return ({
    catsList: state.catsList.catsList,
    count: state.catsList.count,
    cat: state.catsList.cat,
    refreshFlag: state.catsList.refreshFlag,
    errors: state.errors.errors
  })
}

HomePage.propTypes = {
  getCats: PropTypes.func.isRequired,
  getCatsList: PropTypes.func.isRequired,
  getCatData: PropTypes.func.isRequired,
  updateCatData: PropTypes.func.isRequired,
  deleteCatData: PropTypes.func.isRequired,

  catsList: PropTypes.array,
  count: PropTypes.number,
  cat: PropTypes.object,
  refreshFlag: PropTypes.bool,
  errors: PropTypes.object,
}

export default connect(mapStateToProps, { deleteCatData, getCatsList, getCatData, updateCatData, getCats })(HomePage);
