import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { SanPham } from './sanpham.model';

@Injectable({
  providedIn: 'root'
})
export class UploadFileService {
  constructor(private http:HttpClient) { 
  }
  getSanPhamList(data:any){
    return this.http.post(environment.apiURL+'/SanPhams/Search',data);
  }
  uploadVanBan(data: any){
     return this.http.post(environment.apiURL+'/VanBans/Upload',data);
  }
  updateSanPham(data:SanPham){
     return this.http.put(environment.apiURL+'/SanPhams/Update',data);
  }
  deleteSanPham(id:string){
    return this.http.delete(environment.apiURL+'/SanPhams/Delete/'+id);
  }
  getPhanLoaiCongViec(){
    return this.http.get(environment.apiURL+'/PhanLoaiCongViecs/Get');
  }
  getGiaiDoanThucHien(){
    return this.http.get(environment.apiURL+'/GiaiDoanDuAns/Get');
  }
  getTrangThaiThucHien(){
    return this.http.get(environment.apiURL+'/GiaiDoanDuAns/Get');
  }
  getMucDoRuiRo(){
    return this.http.get(environment.apiURL+'/MucDoRuiRos/Get');
  }
}