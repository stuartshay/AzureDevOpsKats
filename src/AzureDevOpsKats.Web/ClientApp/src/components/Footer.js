import React from 'react';
import  {MDBFooter}  from "mdbreact";

const Footer = () => {
    return (
      < MDBFooter color="footer-bg" style={{ position: "absolute", width: '100%' }}>
        <p className="footer-copyright mb-0 py-3 text-center">
          &copy; {new Date().getFullYear()} Copyright: Azure DevOpsKats
        </p>
      </MDBFooter >
    )
}

export default Footer;
