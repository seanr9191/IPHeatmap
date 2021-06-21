import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout/Layout';
import Home from './components/Home/Home';

import './custom.css'

const App = () => {
    App.displayName = "App";

    return (
        <Layout>
            <Route exact path='/' component={Home} />
        </Layout>
    );
};

export default App;