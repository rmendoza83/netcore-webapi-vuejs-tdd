import { createApp } from 'vue'
import router from './router'
import filter from './filter'
import 'bootstrap'

import './sass/styles.scss'

// PrimeVue Imports
import PrimeVue from 'primevue/config'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import ConfirmDialog from 'primevue/confirmdialog'
import ConfirmationService from 'primevue/confirmationservice'
import Dialog from 'primevue/dialog'
import Textarea from 'primevue/textarea'
import Toast from 'primevue/toast'
import ToastService from 'primevue/toastservice'

const app = createApp({})
app.config.globalProperties.$filters = filter

app.use(router)
app.use(PrimeVue, { ripple: true })
app.use(ConfirmationService)
app.use(ToastService)
app.component('DataTable', DataTable)
app.component('Column', Column)
app.component('InputText', InputText)
app.component('Button', Button)
app.component('Dialog', Dialog)
app.component('Textarea', Textarea)
app.component('ConfirmDialog', ConfirmDialog)
app.component('Toast', Toast)

app.mount('#app')
