import React from "react";
import {Row, Col } from 'react-bootstrap';

const GrafanaDashboard = () => {
    const grafanaUrl="http://localhost:3000";
    const dashboardId="aehpettuqq5fkf";
    const orgId = 1;

    return(
        <Row className="mt-3">
            <Col>
                <div className="grafana-container">
                    <iframe
                        src={`${grafanaUrl}/d/${dashboardId}?orgId=${orgId}&kiosk`}
                        width="100%"
                        height="800px"
                        title = "Grafana Dashboard"
                        style={{ border: 'none' }}
                    />
                </div>
            </Col>
        </Row>
    );
}

export default GrafanaDashboard;