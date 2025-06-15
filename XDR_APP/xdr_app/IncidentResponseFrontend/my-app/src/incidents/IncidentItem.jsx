import React from "react";
import { Accordion, Tab, Tabs, Row, Col, ListGroup, Badge } from "react-bootstrap";

const IncidentItem = ({incident}) => {
    return(
        <Accordion.Item eventKey={incident.incidentId} className="mb-1">
            <Accordion.Header className="accordion-header">{incident.title} - {incident.detectedAt}</Accordion.Header>
            <Accordion.Body className="accordion-body">
                <Tabs
                    defaultActiveKey="entity"
                    id={`incident-id-${incident.incidentId}`}
                    className="mt-2 incident-tabs"
                    fill
                >
                    <Tab eventKey="entity" title="Entity" className="incident-tab">
                        <ListGroup>
                                <Row className="d-flex">
                                    <Col sm={12} md={6} className="mb-2">
                                        <ListGroup.Item
                                            as="li"
                                            className="d-flex justify-content-between align-items-start h-100"
                                            variant="primary"
                                            action
                                        >
                                            <div className="my-1 me-auto">
                                                <div className="fw-bold">Sender</div>
                                                {incident.event.sender}
                                            </div>
                                        </ListGroup.Item>
                                    </Col>
                                    <Col sm={12} md={6} className="mb-2">
                                        <ListGroup.Item
                                            as="li"
                                            className="d-flex justify-content-between align-items-start h-100"
                                            variant="primary"
                                            action
                                        >
                                            <div className="my-1 me-auto">
                                                <div className="fw-bold">Status</div>
                                                <Badge className="py-2 px-3 my-2" bg={(incident.status === "Open") ? "danger" : ((incident.status === "In progress") ? "warning" : "success") }>{incident.status}</Badge>
                                            </div>
                                        </ListGroup.Item>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col sm={12} md={6} className="mb-2">
                                        <ListGroup.Item
                                            as="li"
                                            className="d-flex justify-content-between align-items-start h-100"
                                            variant="primary"
                                            action
                                        >
                                            <div className="my-1 me-auto">
                                                <div className="fw-bold">Severity</div>
                                                {incident.severity}
                                            </div>
                                        </ListGroup.Item>
                                    </Col>
                                    <Col sm={12} md={6} className="mb-2">
                                        <ListGroup.Item
                                            as="li"
                                            className="d-flex justify-content-between align-items-start h-100"
                                            variant="primary"
                                            action
                                        >
                                            <div className="my-1 me-auto">
                                                <div className="fw-bold">Description</div>
                                                {incident.description}
                                            </div>
                                        </ListGroup.Item>
                                    </Col>
                                </Row>
                        </ListGroup>
                    </Tab>
                    <Tab eventKey="resource" title="Resource" className="incident-tab">
                        <ListGroup>
                            <Row className="d-flex">
                                <Col sm={12} md={6} className="mb-2">
                                    <ListGroup.Item
                                        as="li"
                                        className="d-flex justify-content-between align-items-start h-100"
                                        variant="primary"
                                        action
                                    >
                                        <div className="my-1 me-auto">
                                            <div className="fw-bold">Subject</div>
                                            {incident.event.subject}
                                        </div>
                                    </ListGroup.Item>
                                </Col>
                                <Col sm={12} md={6} className="mb-2">
                                    <ListGroup.Item
                                        as="li"
                                        className="d-flex justify-content-between align-items-start h-100"
                                        variant="primary"
                                        action
                                    >
                                        <div className="my-1 me-auto">
                                            <div className="fw-bold">Timestamp</div>
                                            {incident.event.timestamp}
                                        </div>
                                    </ListGroup.Item>
                                </Col>
                            </Row>
                            <Row>
                                <Col sm={12} md={6} className="mb-2">
                                    <ListGroup.Item
                                        as="li"
                                        className="d-flex justify-content-between align-items-start h-100"
                                        variant="primary"
                                        action
                                    >
                                        <div className="my-1 me-auto">
                                            <div className="fw-bold">Attachments</div>
                                            <ul className="mb-0">
                                                {incident.event.attachments.map((attachment)=>(
                                                    <li key={attachment.attachmentId}>{attachment.name}</li>
                                                ))}
                                            </ul>
                                        </div>
                                    </ListGroup.Item>
                                </Col>
                                <Col sm={12} md={6} className="mb-2">
                                    <ListGroup.Item
                                        as="li"
                                        className="d-flex justify-content-between align-items-start h-100"
                                        variant="primary"
                                        action
                                    >
                                        <div className="my-1 me-auto">
                                            <div className="fw-bold">Type</div>
                                            {incident.event.typeName}
                                        </div>
                                    </ListGroup.Item>
                                </Col>
                            </Row>
                        </ListGroup>
                    </Tab>
                    <Tab eventKey="recomandation" title="Recomandation" className="incident-tab">
                    <ListGroup>
                        {incident.recommendations.map((recommendation, index) => (
                            <ListGroup.Item 
                                key={index}
                                variant="primary"
                                action
                                className="border-1 fw-semibold"
                            >
                                {recommendation}
                            </ListGroup.Item>
                        ))}
                    </ListGroup>
                    </Tab>
                </Tabs>
            </Accordion.Body>
        </Accordion.Item>
    );
}

export default IncidentItem;