import React from 'react';
import {Label} from '../styled/Lib';

const ErrorLabel = (props) => {
    if ( props.error === undefined || props.error === '')
        return <span></span>;

    return (<Label>{props.error}</Label>);
};

export default ErrorLabel;

