import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import { Fabric } from 'office-ui-fabric-react/lib/Fabric';
import { FluentCustomizations } from '@uifabric/fluent-theme';
import { Customizer, mergeStyles } from 'office-ui-fabric-react';
import  {AppState} from './service';
import App from './App';
//import * as serviceWorker from './serviceWorker';
//import registerServiceWorker from './registerServiceWorker';

function getBaseUrl(){
  const baseTags = document.getElementsByTagName('base');
  if (baseTags.length>0){
    return baseTags[0].getAttribute('href');
  } else {
    return window.location.protocol+"//"+window.location.host;
  }
}

const baseUrl=getBaseUrl();
const rootElement = document.getElementById('root');

// Inject some global styles
mergeStyles({
  selectors: {
    ':global(body), :global(html), :global(#root)': {
      margin: 0,
      padding: 0,
      height: '100vh'
    }
  }
});

const appState = new AppState();
ReactDOM.render(
  <Fabric>
  <Customizer {...FluentCustomizations}>
  <BrowserRouter basename={baseUrl}>
    <App appState={appState}/>
  </BrowserRouter>
  </Customizer>
  </Fabric>,
  rootElement);


  //registerServiceWorker();

