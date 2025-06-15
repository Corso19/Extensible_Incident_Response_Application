import React from "react";
import { Navbar, Nav, Container } from "react-bootstrap";
import Orchestrator from "../../orchestrator/Orchestrator";

const UserNavbar = () => {
  return (
    <Navbar expand="sm" className="xdr-navbar">
      <Container>
        <Navbar.Brand href="/sensors" title="Sensors">
          XDR App
        </Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="col d-flex justify-content-start">
            <Nav.Link
              eventKey={1}
              href="/sensors"
              className="mx-3"
              title="Sensors"
            >
              Sensors
            </Nav.Link>
            <Nav.Link
              eventKey={2}
              href="/incidents"
              className="mx-3"
              title="Incidents"
            >
              Incidents
            </Nav.Link>
          </Nav>
          {/* <Orchestrator /> */}
          <Nav.Link
            eventKey={3}
            href="/dashboard"
            className="mx-3"
            title="Dashboard"
          >
            Dashboard
          </Nav.Link>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default UserNavbar;
