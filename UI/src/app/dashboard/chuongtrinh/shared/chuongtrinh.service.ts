import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { Chuongtrinh } from './chuongtrinh.model';

@Injectable({
  providedIn: 'root'
})
export class ChuongtrinhService {
  isCreateOrUpdateOrDelete : boolean = false;
  constructor(private http:HttpClient) { 
  }
  getChuongTrinhList(data:any){
    return this.http.post(environment.apiURL+'/ChuongTrinhs/Search',data);
  }
  createChuongTrinh(data: Chuongtrinh){
     return this.http.post(environment.apiURL+'/ChuongTrinhs/Add',data);
  }
  updateChuongTrinh(data:Chuongtrinh){
     return this.http.put(environment.apiURL+'/ChuongTrinhs/Update',data);
  }
  deleteChuongTrinh(id:string){
    return this.http.delete(environment.apiURL+'/ChuongTrinhs/Delete/'+id);
  }
}
