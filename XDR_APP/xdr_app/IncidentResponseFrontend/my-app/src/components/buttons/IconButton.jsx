import { Spinner } from "react-bootstrap";
import { Link } from 'react-router-dom'
import React from 'react';

const IconButton = (props) => {

    return (
        props.conditionForSpinner ? (
            <Spinner
                as="span"
                variant={props.variant}
                size="sm"
                role="status"
                aria-hidden="true"
                animation="border"
                className={props.spinnerClassName}
            />
        ):(
            <Link className={props.linkClassName}>
                <props.icon
                    size={props.iconSize}
                    onClick={props.onClick}
                    title={props.title}
                    color={props.color}
                />
            </Link>
        )
    );
}

export default IconButton;