import React, { useEffect, useState } from "react";
import { CrudService } from "../services/CrudService";
import { ThreeDots } from "react-bootstrap-icons";
import { Accordion, Col, Row } from "react-bootstrap";
import IncidentItem from "./IncidentItem";
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr';

const Incidents = () => {
    const [incidents, setIncidents] = useState([]);
    const [incidentsLoading, setIncidentsLoading] = useState(false);
    const [connection, setConnection] = useState(null);

    // connect to signalr

    useEffect(() => {
        const new_connection = new HubConnectionBuilder()
            .withUrl('https://localhost:7142/incidentHub', {
                withCredentials: true,
                skipNegotiation: false,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .build();
    
        // Add incident handler before starting connection
        new_connection.on('ReceivedIncident', incident => {
            console.log("Received new incident:", incident);
            setIncidents(previous_incidents => [...previous_incidents, incident]);
        });
    
        // Start connection
        new_connection.start()
            .then(() => {
                console.log("SignalR Connected Successfully!");
                setConnection(new_connection);
            })
            .catch(error => console.error("SignalR Connection Error:", error));
    
        // Cleanup
        return () => {
            if (new_connection) {
                new_connection.off('ReceivedIncident');
                new_connection.stop()
                    .then(() => console.log("SignalR connection stopped."))
                    .catch(err => console.error("Error stopping connection:", err));
            }
        };
    }, []);

    useEffect(() => {
        setIncidentsLoading(true);
        // get sensors
        CrudService.list("Incidents").then((response) => {
            if (response.status === 200) {
                setIncidents(response.data);
            }
            setIncidentsLoading(false);
        });
    }, []);

    return(
        incidents && (
        <Row className="mt-3">
            <Col sm={12}>
            {
                incidentsLoading ? (
                    <div
                        style={{ width: "100%" }}
                        className="d-flex justify-content-center"
                    >
                        <ThreeDots
                            height="50"
                            width="50"
                            ariaLabel="three-dots-loading"
                            visible={true}
                            color="#005bb5"
                        />
                    </div>
                ) : (
                    incidents.length ? (
                        <Accordion className="mx-3">
                        {
                            incidents.map((incident) => (
                                <IncidentItem
                                    key={incident.incidentId} 
                                    incident={incident} 
                                />
                            ))
                        }
                        </Accordion>
                    ) : (
                        <h5 className="mx-3">No incidents added.</h5>
                    )
                )
            }
            </Col>
        </Row>
        )
    );
}

export default Incidents;