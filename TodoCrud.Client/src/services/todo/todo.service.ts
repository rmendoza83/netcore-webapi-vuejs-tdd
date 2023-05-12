import { AxiosObservable } from 'axios-observable';
import { HTTPBaseService } from '../common/http-base.service';
import { HTTP_CONSTANTS } from '../../constants/http-constants';

export class TodoService extends HTTPBaseService {

  constructor() {
    super();
  }

  getList(): AxiosObservable<any> {
    const url = 'Todo';
    return this.apiCustom<any>(url, HTTP_CONSTANTS.GET);
  }

  insert(model: any): AxiosObservable<any> {
    const url = 'Todo';
    return this.apiCustom<any>(url, HTTP_CONSTANTS.POST, model);
  }

  update(id: string, model: any): AxiosObservable<any> {
    const url = `Todo/${id}`;
    return this.apiCustom<any>(url, HTTP_CONSTANTS.PUT, model);
  }

  delete(id: string): AxiosObservable<any> {
    const url = `Todo/${id}`;
    return this.apiCustom<any>(url, HTTP_CONSTANTS.DELETE);
  }
}
