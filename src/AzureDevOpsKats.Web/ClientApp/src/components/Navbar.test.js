import React from 'react';
import {Provider} from 'react-redux';
import { BrowserRouter as Router } from "react-router-dom";
import Enzyme, { mount } from 'enzyme';
import { expect } from 'chai';
import Adapter from '@wojtekmaj/enzyme-adapter-react-17';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import Navbar from './Navbar';
import { 
  MDBCollapse, MDBModal, MDBModalBody, MDBBtn 
} from "mdbreact";

Enzyme.configure({ adapter: new Adapter() });
const mockStore = configureMockStore([thunk]);

describe('Navbar testing', () => {
  let store;
  beforeEach(() => {
    store = mockStore({
      catsList: {
        catsList: []
      },
      errors: {
        errors: []
      }
    });
  });

  //---------- Nabvar mouting test --------------

  it('should render the Navbar component', () => {
    const wrapper = mount(
      <Provider store={store}>
        <Router>
          <Navbar />
        </Router>
      </Provider>
    );

    expect(wrapper.find(Navbar).length).to.equal(1);
    const container = wrapper.find(Navbar);
    expect(container.find(MDBCollapse).props().isOpen).to.eql("");
    expect(container.find(MDBModal).props().isOpen).to.eql(false);

    const modal = container.find(MDBModal);
    expect(modal.find(MDBModalBody).length).to.equal(0);
  });

  //---------- Navbar mothods testing ( toggle Modal Window ) --------------
    
  it('Test show / hide add modal function', () => {
    const wrapper = mount(
      <Provider store={store}>
        <Router>
          <Navbar/>
        </Router>
      </Provider>
    );
    const modalBtn = wrapper.find(MDBBtn);
    //----- when click the + Add button the modal window should be opened
    modalBtn.simulate('click');
    expect(wrapper.find(MDBModal).props().isOpen).to.equal(true);

    //----- when click the close button of the modal, the modal window should be closed
    wrapper.find('.close').simulate('click');
    expect(wrapper.find(MDBModal).props().isOpen).to.equal(false);
  });
});