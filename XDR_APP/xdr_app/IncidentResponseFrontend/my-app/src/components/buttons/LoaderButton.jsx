import React from 'react';
import { Spinner, Button} from "react-bootstrap";

const LoaderButton = (props) => {

    return (
        <Button
            disabled={props.disabled || props.loading ? true : false}
            variant={props.variant}
            className={props.className}
            onClick={props.onClick}
            style={props.style}
            type={props.type}
            size={props.size}
        >
            { 
                props.loading ? (
                    <Spinner
                        as="span"
                        size="sm"
                        role="status"
                        aria-hidden="true"
                        animation="border"
                    />
                ): ( props.text )
            }
                
        </Button>
    );
}


export default LoaderButton;