import React from 'react';
import ButtonGroup from 'react-bootstrap/ButtonGroup';
import ToggleButton from 'react-bootstrap/ToggleButton';

const ToggleButtonCustom = ({index, value, setValue}) => {

    const options = [
        { name: 'Yes', value: true },
        { name: 'No', value: false }
    ];

    return (
        <ButtonGroup>
            {options.map((option, optionIndex) => (
            <ToggleButton
                key={optionIndex}
                id={`id-${index}-${optionIndex}`}
                type="radio"
                variant="outline-primary"
                name={`name-${index}-${optionIndex}`}
                value={option.value}
                checked={value === option.value}
                style={{"zIndex": "1"}}
                onChange={(event) => {
                    const selectedValue = event.currentTarget.value === "true";
                    setValue(selectedValue);
                }}
            >
                {option.name}
            </ToggleButton>
            ))}
      </ButtonGroup>
    );
}

export default ToggleButtonCustom;