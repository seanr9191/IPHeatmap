import React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu/NavMenu';

const Layout = (props) => {
    Layout.displayName = "Layout";

    return (
        <div>
            <NavMenu />
            <Container style={{ width: '100%', height: '90vh' }}>
                {props.children}
            </Container>
        </div>
    );
};

export default Layout;
