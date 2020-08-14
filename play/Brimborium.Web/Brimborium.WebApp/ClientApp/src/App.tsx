import React, { Component } from 'react';
// import { Route } from 'react-router';
// import { Layout } from './components/Layout';
// import { Home } from './components/Home';
// import { FetchData } from './components/FetchData';
// import { Counter } from './components/Counter';

import './custom.css'
import { AppState } from './service';
type App_Props={
  appState: AppState;
};
type App_State={

};
export default class App extends Component<App_Props,App_State> {
  static displayName = App.name;
  constructor(props:App_Props) {
    super(props);
    this.state={};
  }
  /*
  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/counter' component={Counter} />
        <Route path='/fetch-data' component={FetchData} />
      </Layout>
    );
  }
  */
 render(){
   return (
    <div>Hallo Welt</div>
   );
 }
}
