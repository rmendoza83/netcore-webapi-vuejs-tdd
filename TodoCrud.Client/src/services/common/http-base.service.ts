import { Axios } from 'axios-observable';
import { AxiosObservable } from '~/axios-observable/dist/axios-observable.interface';
import { HTTP_CONSTANTS } from '../../constants/http-constants';

export abstract class HTTPBaseService {
  private axiosInstance: Axios;
  protected readonly serverAPIURL: string = 'http://localhost:5000/api/';
  protected moduleName: string = '';
  protected serviceName: string = '';
  protected extendedServiceData: string = '';

  constructor() {
    this.axiosInstance = Axios.create({
      baseURL: this.serverAPIURL
    });
  }

  private createAxiosInstance() {
    this.axiosInstance = Axios.create({
      baseURL: this.serverAPIURL,
      headers: this.getHTTPHeaders(),
    });
  }

  private getHTTPHeaders(): any {
    let httpHeaders = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization': '',
    };
    return httpHeaders;
  }

  private doHTTPCall<T>(url: string, method: string, bodyData: any): AxiosObservable<T> {
    this.createAxiosInstance();
    let http = this.axiosInstance;

    switch (method) {
      case HTTP_CONSTANTS.GET:
        return http.get<T>(url);
      case HTTP_CONSTANTS.POST:
        return http.post<T>(url, bodyData);
      case HTTP_CONSTANTS.PUT:
        return http.put<T>(url, bodyData);
      case HTTP_CONSTANTS.PATCH:
        return http.patch<T>(url, bodyData);
      case HTTP_CONSTANTS.DELETE:
        return http.delete<T>(url);
      default:
        return http.get<T>(url);
    }
  }

  private doGET<T>(url: string): AxiosObservable<T> {
    return this.doHTTPCall<T>(url, HTTP_CONSTANTS.GET, null);
  }

  private doPOST<T>(url: string, bodyData: any): AxiosObservable<T> {
    return this.doHTTPCall<T>(url, HTTP_CONSTANTS.POST, bodyData);
  }

  private doPUT<T>(url: string, bodyData: any): AxiosObservable<T> {
    return this.doHTTPCall<T>(url, HTTP_CONSTANTS.PUT, bodyData);
  }

  private doPATCH<T>(url: string, bodyData: any): AxiosObservable<T> {
    return this.doHTTPCall<T>(url, HTTP_CONSTANTS.PATCH, bodyData);
  }

  private doDELETE<T>(url: string): AxiosObservable<T> {
    return this.doHTTPCall<T>(url, HTTP_CONSTANTS.DELETE, null);
  }

  private getURL(): string {
    let url = `${this.serverAPIURL}${this.moduleName}/${this.serviceName}`;
    if (this.extendedServiceData && this.extendedServiceData.length > 0) {
      url += `/${this.extendedServiceData}`;
      this.extendedServiceData = '';
    }
    return url;
  }

  protected setExtendedServiceData(data: string) {
    this.extendedServiceData = data;
  }

  protected apiGetList<T>(): AxiosObservable<T> {
    return this.doGET(this.getURL());
  }

  protected apiGetItem<T>(): AxiosObservable<T> {
    return this.doGET<T>(this.getURL());
  }

  protected apiInsert<T>(objectData: any): AxiosObservable<T> {
    return this.doPOST<T>(this.getURL(), objectData);
  }

  protected apiUpdate<T>(objectData: any): AxiosObservable<T> {
    return this.doPUT<T>(this.getURL(), objectData);
  }

  protected apiPatch<T>(objectData: any): AxiosObservable<T> {
    return this.doPATCH<T>(this.getURL(), objectData);
  }

  protected apiDelete<T>(): AxiosObservable<T> {
    return this.doDELETE<T>(this.getURL());
  }

  protected apiDeleteAll<T>(): AxiosObservable<T> {
    return this.doDELETE<T>(this.getURL());
  }

  protected apiCustom<T>(url: string, method: string, bodyData?: any): AxiosObservable<T> {
    return this.doHTTPCall<T>(url, method, bodyData);
  }
}
