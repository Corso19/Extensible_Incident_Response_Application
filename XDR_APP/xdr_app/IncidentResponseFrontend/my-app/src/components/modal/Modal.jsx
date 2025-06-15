import { Button } from "react-bootstrap";
import Modal from "react-modal";
import React from "react";
import LoaderButton from "../buttons/LoaderButton";
import { Tooltip } from "@mui/material";
import { RiErrorWarningFill } from "react-icons/ri";

const FormModal = ({
    showModal, 
    setShowModal, 
    title, 
    children, 
    disableSubmitButton,
    disabledMessage, 
    loadingSubmitButton, 
    onSubmit,
    width,
    deleteAction
}) => {
    return (
        <Modal 
            isOpen={showModal}
            style={{
                overlay: {
                    zIndex: 100, // Lower than modal content but higher than page elements
                },
                content: {
                    top: "50%",
                    left: "50%",
                    right: "auto",
                    bottom: "auto",
                    transform: "translate(-50%, -50%)",
                    backgroundColor: "white",
                    width: width,
                    height: "auto",
                    maxHeight: "90vh",
                    maxWidth: "90%",
                    overflowY: "auto",
                    overflowX: "hidden",
                    zIndex: 100
                }
            }}
        >
            <div className="modal-content d-flex card">
                {/* MODAL HEADER*/}
                <div className="modal-header align-items-center color-bs-primary py-2" style={{ width: "100%"}}>
                    <h2 className={`${deleteAction ? "text-left" : "text-center"} mx-auto py-0 my-0`}>{title}</h2>
                </div>
                <hr className="custom-hr"></hr>

                {/* MODAL BODY*/}
                <div className="modal-body"> 
                    {children}
                </div>
                <hr className="custom-hr"></hr>

                {/* Modal Footer */}
                <div className="modal-footer d-flex justify-content-between mb-0">
                    <Button
                        className="ms-3 px-4"
                        variant="outline-secondary"
                        onClick={() => setShowModal(false)}
                    >
                        Cancel
                    </Button>
                    <div className="d-flex align-items-center">
                        {disableSubmitButton && (
                            <Tooltip
                                title={
                                    <span style={{ fontSize: "1.2em" }}>
                                        {disabledMessage}
                                    </span>
                                }
                                placement="left"
                            >
                                <div>
                                    <RiErrorWarningFill style={{ color: "#DC3545", fontSize: "1.3em" }}/>
                                </div>
                            </Tooltip>
                        )}
                        <LoaderButton
                            disabled={disableSubmitButton}
                            loading={loadingSubmitButton}
                            onClick={onSubmit}
                            text={deleteAction ? "Delete" : "Save"}
                            className="mx-3 px-4"
                            variant={deleteAction ? "outline-danger" : "outline-primary"}
                        />
                    </div>
                </div>
            </div>
        </Modal>
    );
}

export default FormModal;