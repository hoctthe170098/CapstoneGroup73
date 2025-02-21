import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { QuillEditorComponent } from 'ngx-quill';
import {  ToastrService } from 'ngx-toastr';
import { SanPhamService } from '../shared/sanpham.service';
import { UploadFileService } from '../shared/uploadFile.service';

@Component({
  selector: 'app-createsanpham',
  templateUrl: './createsanpham.component.html',
  styleUrls: ['./createsanpham.component.scss']
})
export class CreatesanphamComponent implements OnInit {
  @ViewChild(QuillEditorComponent) editor: QuillEditorComponent;
  @ViewChild('fileInput') fileInput: ElementRef;
  constructor(private formBuilder: FormBuilder,private router:Router,private toast: ToastrService,private service: SanPhamService,private uploadService:UploadFileService ) { }
  rowCount : number = 5 ;
  selectedFile : string ='';
  uploadedFiles: File[] = [];
  phanLoaiCongViec:any[]=[];
  giaiDoanThucHien:any[]=[];
  trangThaiThucHien:any[]=[];
  mucDoRuiRo:any[]=[];
   itemList:[]
   formChuongTrinh = this.formBuilder.group({
    content:['']
  });
  
  ngOnInit(): void {
    this.getPhanLoaiCongViec();
    this.getGiaiDoanThucHien();
    this.getTrangThaiThucHien();
    this.getMucDoRuiRo();
  }
  getPhanLoaiCongViec(){
    this.service.getPhanLoaiCongViec().subscribe((res:any)=>{
      if(!res.isError){
        this.phanLoaiCongViec = res.data;
      }else this.toast.error(res.message);
    })
  }
  getGiaiDoanThucHien(){
    this.service.getGiaiDoanThucHien().subscribe((res:any)=>{
      if(!res.isError){
        this.giaiDoanThucHien = res.data;
      }else this.toast.error(res.message);
    })
  }
  getTrangThaiThucHien(){
    this.service.getTrangThaiThucHien().subscribe((res:any)=>{
      if(!res.isError){
        this.trangThaiThucHien = res.data;
      }else this.toast.error(res.message);
    })
  }
  getMucDoRuiRo(){
    this.service.getMucDoRuiRo().subscribe((res:any)=>{
      if(!res.isError){
        this.mucDoRuiRo = res.data;
      }else this.toast.error(res.message);
    })
  }
  handleFileUpload(event: any) {
    const file = event.target.files[0];
    const body = {
      name:'test',
      file:file
    }
        this.uploadService.uploadVanBan(body).subscribe((res: any) => {
          if (!res.isError) {
            this.toast.success(res.message);
          } else {
            this.toast.error(res.message);
          }
        })
    console.log('Đã chọn file:', file);
    if(this.uploadedFiles.find((x:File)=>x.name == file.name)!=undefined){
      this.toast.error("File này đã được đính kèm!");
    }else{
      this.uploadedFiles.push(file);
    } 
    this.fileInput.nativeElement.value = '';
  } 
  selectFile() {
    this.fileInput.nativeElement.click();
  }
  view(file:any){
  this.selectedFile = URL.createObjectURL(file);
  window.open(this.selectedFile,'_blank');
  }
  close(){
    this.router.navigate(['/sanpham']);
  }
  delete(i:number){
    this.uploadedFiles.splice(i,1);
  }
}
