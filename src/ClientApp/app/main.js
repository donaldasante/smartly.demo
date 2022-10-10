import '@riotjs/hot-reload'
import { component } from 'riot'
import 'skeleton-css/css/normalize.css';
import 'skeleton-css/css/skeleton.css';
import '../app/style/style.css';
import { library, dom } from "@fortawesome/fontawesome-svg-core";
import { faEdit } from "@fortawesome/free-solid-svg-icons/faEdit";
import { faEye } from "@fortawesome/free-solid-svg-icons/faEye";
import App from './tags/app.riot';

/* Initialize Boot page */
console.log('Initializing App');

//Add edit and eye fontawesome font for use in the admin page
library.add(faEdit, faEye);
dom.watch();

component(App)(document.getElementById('app'), {
    name: 'Smartly Demo App',
})