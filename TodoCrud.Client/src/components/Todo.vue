<template>
  <div class="p-3" style="w-100">
    <div class="row">
      <div class="col-6">
        <h1>Todo Items</h1>
      </div>
      <div class="col-6 d-flex justify-content-end pb-3">
        <Button label="Add Record" severity="success" icon="pi pi-plus" @click="addNewRecord()" autofocus raised class="action-button" />
      </div>
    </div>
    <div class="pb-2 d-flex justify-content-center table-content">
      <DataTable :value="listData" dataKey="id" paginator :rows="10" :rowsPerPageOptions="[5, 10, 20, 50]" 
        :loading="loading" :class="`p-datatable-sm p-datatable-responsive`">
        <template #empty> No Records </template>
        <Column field="id" header="Id" style="width: 30%"></Column>
        <Column field="name" header="Name" sortable style="width: 25%">
          <template #editor="{ data, field }">
            <InputText v-model="data[field]" class="w-100"/>
          </template>
        </Column>
        <Column field="description" header="Description" sortable style="width: 25%">
          <template #editor="{ data, field }">
            <InputText v-model="data[field]" class="w-100"/>
          </template>
        </Column>
        <Column field="createdAt" header="Created At" sortable style="width: 10%">
          <template #body="slotProps">
            <div class="d-flex justify-content-center">
              <span>{{ $filters.formatDateTime(slotProps.data.createdAt) }}</span>
            </div>
          </template>
        </Column>
        <Column field="updatedAt" header="Updated At" sortable style="width: 10%">
          <template #body="slotProps">
            <div class="d-flex justify-content-center">
              <span>{{ $filters.formatDateTime(slotProps.data.updatedAt) }}</span>
            </div>
          </template>
        </Column>
        <Column :exportable="false" style="min-width:8rem">
          <template #body="slotProps">
            <Button icon="pi pi-pencil" outlined rounded class="mr-3 pr-3" @click="editRecord(slotProps.data)" />
            <Button icon="pi pi-trash" outlined rounded severity="danger" @click="deleteRecord(slotProps.data)" />
          </template>
        </Column>
      </DataTable>
    </div>
    <Dialog v-model:visible="editDialogVisible" modal :header="!editing ? `Add Record` : `Edit Record ${record.id}`" :style="{ width: '50vw' }">
      <div class="card p-3">
        <div class="mb-3">
          <label for="nameLabel" class="form-label">Name</label>
          <InputText v-model="record.name" placeholder="Name" id="nameLabel" class="form-control"></InputText>
        </div>
        <div class="mb-3">
          <label for="descriptionLabel" class="form-label">Description</label>
          <Textarea v-model="record.description" id="descriptionLabel" rows="5" cols="50" class="form-control"></Textarea>
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" @click="cancelRecord()" text />
        <Button label="Save" icon="pi pi-check" @click="saveRecord()" autofocus />
      </template>
    </Dialog>
    <ConfirmDialog />
    <Toast position="bottom-right" />
  </div>
</template>

<script lang="ts">

import { Component, Vue } from 'vue-facing-decorator';
import { TodoService } from "../services/todo/todo.service";
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";
import { timer } from "rxjs";
import moment from 'moment';

@Component
export default class TodoComponent extends Vue {
  timeRefresher = 300000;
  private todoService: TodoService = new TodoService();
  public listData: any[] = [];
  public editDialogVisible: boolean = false;
  public editing: boolean = false;
  public record: any = {};  
  private confirmService = useConfirm();
  private toastService = useToast();
  public loading: boolean = false;
  private refresher = timer(this.timeRefresher, this.timeRefresher);

  created() {
    console.log("created...");
  }

  private getListData() {
    this.loading = true;
    this.todoService.getList().subscribe((response) => {
      this.listData = response.data;
      this.loading = false;
    });
  }

  mounted() {
    this.$nextTick().then(() => {
      this.getListData();
      this.refresher.subscribe(() => {
        this.getListData();
      });
    });
  }

  addNewRecord() {
    this.record = {}
    this.editDialogVisible = true
  }

  editRecord(record: any) {
    this.editing = true;
    this.record = record;
    this.editDialogVisible = true
  }

  saveRecord() {
    if (!this.editing) {
      this.insertRecord();
    } else {
      this.updateRecord();
    }
  }

  cancelRecord() {
    this.editing = false;
    this.record = {};
    this.editDialogVisible = false;
  }

  insertRecord() {
    this.record.createdAt = moment(new Date()).toDate();
    this.record.updatedAt = moment(new Date()).toDate();
    this.todoService.insert(this.record)
      .subscribe(response => {
        if (response.status == 201) {
          this.toastService.add({
            severity: 'success',
            summary: 'Confirmed',
            detail: 'Record Added',
            life: 3000
          });
          this.getListData();
        } else {
          this.toastService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Some error ocurred, nothing saved!',
            life: 3000
          });
        }
        this.record = {};
        this.editDialogVisible = false;
      });
  }

  updateRecord() {
    this.record.updatedAt = moment(new Date()).toDate();
    this.todoService.update(this.record.id, this.record)
      .subscribe(response => {
        if (response.status == 204) {
          this.toastService.add({
            severity: 'success',
            summary: 'Confirmed',
            detail: 'Record Updated',
            life: 3000
          });
          this.getListData();
        } else {
          this.toastService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Some error ocurred, nothing saved!',
            life: 3000
          });
        }
        this.record = {};
        this.editing = false;
        this.editDialogVisible = false;
      });
  }

  deleteRecord(recordToDelete: any) {
    this.confirmService.require({
      message: 'Do you want to delete this record?',
      header: 'Delete Confirmation',
      icon: 'pi pi-info-circle',
      acceptClass: 'p-button-danger',
      position: 'top',
      accept: () => {
        this.todoService.delete(recordToDelete.id)
          .subscribe(response => {
            if (response.status == 204) {
              this.toastService.add({
                severity: 'success',
                summary: 'Confirmed',
                detail: 'Record Deleted',
                life: 3000
              });
              this.getListData();
            } else {
              this.toastService.add({
                severity: 'error',
                summary: 'Error',
                detail: 'Some error ocurred, nothing saved!',
                life: 3000
              });
            }
          });
      }
    });
  }
}
</script>

<style lang="css">
.table-content {
  width: 95%;
}

.action-button {
  width: 135px;
  margin-left: 10px !important;
}
</style>
