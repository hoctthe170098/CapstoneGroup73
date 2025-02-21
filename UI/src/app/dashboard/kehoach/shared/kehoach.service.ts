import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { environment } from 'environments/environment';
import { KeHoach } from './kehoach.model';
import { catchError } from 'rxjs/operators';
import { of, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ItemService {
  isCreateOrUpdateOrDelete : boolean = false;
  constructor(private http:HttpClient) { 
  }
  getKehoachList(data:any){
     return this.http.post(environment.apiURL+'/KeHoachs/Search',data);
  }
  createKeHoach(data: KeHoach){
     return this.http.post(environment.apiURL+'/KeHoachs/Add',data);
  }
  updateKeHoach(data:KeHoach){
     return this.http.put(environment.apiURL+'/KeHoachs/Update',data);
  }
  deleteKeHoach(id:string){
    return this.http.delete(environment.apiURL+'/KeHoachs/Delete/'+id);
  }
}
