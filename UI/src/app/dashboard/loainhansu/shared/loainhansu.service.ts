import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { LoaiNhanSu } from './loainhansu.model';

@Injectable({
  providedIn: 'root'
})
export class LoaiNhanSuService {
  isCreateOrUpdateOrDelete : boolean = false;
  constructor(private http:HttpClient) { 
  }
  getLoainhansuList(data:any){
     return this.http.post(environment.apiURL+'/LoaiNhanSus/Search',data);
  }
  createLoainhansu(data: LoaiNhanSu){
     return this.http.post(environment.apiURL+'/LoaiNhanSus/Add',data);
  }
  updateLoainhansu(data:LoaiNhanSu){
     return this.http.put(environment.apiURL+'/LoaiNhanSus/Update',data);
  }
  deleteLoainhansu(id:string){
    return this.http.delete(environment.apiURL+'/LoaiNhanSus/Delete/'+id);
  }
}
