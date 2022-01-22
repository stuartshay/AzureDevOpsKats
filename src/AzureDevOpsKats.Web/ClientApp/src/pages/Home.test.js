import React from 'react';
import {Provider} from 'react-redux';
import Enzyme, { mount } from 'enzyme';
import { expect } from 'chai';
import Adapter from '@wojtekmaj/enzyme-adapter-react-17';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import { MDBModal } from "mdbreact";
import HomePage from './HomePage';

Enzyme.configure({ adapter: new Adapter() });
const mockStore = configureMockStore([thunk]);


describe('HomePage testing', () => {
  let store, store1;
  beforeEach(() => {
    store = mockStore({
      catsList: {
        catsList: []
      },
      errors: {
        errors: []
      }
    });
    store1 = mockStore({
      catsList: {
        catsList: [{     
          id: 0,     
          name: 'aaa',
          description: 'aaa',
          photo: 'aaa',
        }],
        count: 1,
      },
      errors: {
        errors: []
      }
    });
  });

  //---------- Hompage Mounting Test --------------

  it('should render the HomePage component', () => {
    const wrapper = mount(
      <Provider store={store}>
          <HomePage />
      </Provider>
    );

    expect(wrapper.find(HomePage).length).to.equal(1);
    const container = wrapper.find(HomePage);
    expect(container.find(MDBModal).props().isOpen).to.eql(false);
  });

  //---------- Hompage Method Testing ( Toggle Edit Modal ) --------------
     
  it('Test show / hide edit modal function', () => {
    const wrapper = mount(
      <Provider store={store1}>
          <HomePage />
      </Provider>
    );

    //----- when click the Edit button the edit modal window should be opened
    const openModalBtn = wrapper.find('.btn-success');
    openModalBtn.simulate('click');
    expect(wrapper.find(MDBModal).props().isOpen).to.equal(true);

    //----- when click the close button the edit modal window should be hide
    const closeBtn = wrapper.find('.close');
    closeBtn.simulate('click');
    expect(wrapper.find(MDBModal).props().isOpen).to.equal(false);
  });
});