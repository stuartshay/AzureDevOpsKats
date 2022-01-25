import React from 'react';
import Footer from './Footer';
import Enzyme, { shallow } from 'enzyme';
import toJson from 'enzyme-to-json';
import Adapter from '@wojtekmaj/enzyme-adapter-react-17';

Enzyme.configure({ adapter: new Adapter() });

// SnapShot Testing for the Footer component

it('renders correctly enzyme', () => {
    const wrapper = shallow(<Footer />)
  
    expect(toJson(wrapper)).toMatchSnapshot();
});