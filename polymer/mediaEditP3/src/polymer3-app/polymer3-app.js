import { html, PolymerElement } from '@polymer/polymer/polymer-element.js';

import '@polymer/paper-dropdown-menu/paper-dropdown-menu.js';
import '@polymer/paper-item/paper-item.js';
import '@polymer/paper-listbox/paper-listbox.js';
import '@polymer/paper-button/paper-button.js';
import '@polymer/iron-ajax/iron-ajax.js';
import '../../node_modules/xtal-json-editor/xtal-json-editor.js';
import '../../node_modules/xtal-json-merge/xtal-json-merge.js';
import '../../node_modules/pass-down/pass-down.js';

/**
 * @customElement
 * @polymer
 */
class Polymer3App extends PolymerElement {
  static get template() {
    return html`
    <style>
    :host {
      --paper-listbox-background-color: var(--ltg-color-background-lighter);
      --paper-input-container-input-color: var(--ltg-color-white);
      --paper-input-container-focus-color: var(--ltg-color-green);

      display: block;
    }

    paper-dropdown-menu {
      width: 100%;
    }

    paper-listbox {
      min-width: 260px;
      padding: 0;
    }
  </style>
    <div class="card">
    <iron-ajax id="getPresentations"
    url="http://localhost:8011/presentations"
    last-response="{{presentations}}" on-response="gotPresentations"></iron-ajax>
    <paper-button raised on-tap="loadPresentations">Load Presentations</paper-button>

    <paper-dropdown-menu label="Presentations">
    <paper-listbox slot="dropdown-content" selected="1">
      <template is="dom-repeat" items="[[presentations]]">
        <paper-item>[[item]]</paper-item>
      </template>
    </paper-listbox>
  </paper-dropdown-menu>
  </div>

<iron-ajax id="getExample" method="GET" body='{{getBody}}' handle-as="json" content-type="application/json"
  on-response="serverSuccessCb" last-response="{{serverSuccessObj}}" on-error="serverErrorCb" last-error="{{serverErrorObj}}"
  url="http://localhost:8011/project/1">
</iron-ajax>


<div class="card">
<div data-pd>
<pass-down></pass-down>
<h3>Basic xtal-json-editor demo</h3>
<p>Instructions:  Edit the object below and see the values reflected the second JSON Editor (which will appear after making an edit)</p>
<xtal-insert-json input="{}"
  data-on="merged-prop-changed: pass-to-next:{input:serverSuccessObj}">
</xtal-insert-json>

<xtal-json-editor options="{}" as="json"
  data-on="edited-result-changed: pass-to:xtal-json-editor{input:serverSuccessObj}"
></xtal-json-editor>

<div>Edited:</div>
<xtal-json-editor options="{}" as="json"></xtal-json-editor>
<paper-button raised on-tap="getData">Get Media</paper-button>
</div>

<div class="card">
  <h3>serverErrorObj Results:</h3>
  <span>{{serverErrorObj}}</span>
</div>
    `;
  }
  static get properties() {
    return {
      prop1: {
        type: String,
        value: 'polymer3-app'
      }
    };
  }
  getData(event) {
    console.log("calling ajax get" + JSON.stringify(event, null, 2));
    this.$.getExample.generateRequest();
  }

  loadPresentations(event) {
    console.log("calling ajax get" + JSON.stringify(event.detail, null, 2));
    this.$.getPresentations.generateRequest();
  }

  serverSuccessCb(e) {
    const resp = e.detail.response;
    console.log("serverSuccess call back ajax get " + JSON.stringify(resp, null, ' '));
  }

  gotPresentations(e) {
    const resp = e.detail.response;
    console.log("serverSuccess call back ajax get " + JSON.stringify(resp, null, ' '));
  }
}

window.customElements.define('polymer3-app', Polymer3App);
