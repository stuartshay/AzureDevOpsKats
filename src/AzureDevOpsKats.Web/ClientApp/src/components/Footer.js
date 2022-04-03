import React from 'react';
import  {MDBFooter}  from "mdbreact";

const Footer = (props) => {
    return (
      < MDBFooter color={!props.theme?"green":props.theme} style={{ position: "absolute", width: '100%' }}>
        <p className="footer-copyright mb-0 py-3 text-center">
          &copy; {new Date().getFullYear()} Copyright: {props.env} DevOpsKats
        </p>
      </MDBFooter >
    )
}

export default Footer;
