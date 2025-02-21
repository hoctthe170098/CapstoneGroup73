import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { LoaiNhanSuService } from '../shared/loainhansu.service';
import { ToastrService } from 'ngx-toastr';
import { LoaiNhanSu } from '../shared/loainhansu.model';

@Component({
  selector: 'app-createloainhansu',
  templateUrl: './createloainhansu.component.html',
  styles: []
})
export class CreateloainhansuComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<CreateloainhansuComponent>,
    private formBuilder: FormBuilder, private service: LoaiNhanSuService, private toastr: ToastrService) { }
    loainhansu: LoaiNhanSu;
    formLoaiNhansu = this.formBuilder.group({
      code: ['',[Validators.required,Validators.pattern('^[a-zA-Z0-9_-]{0,}$'),Validators.minLength(3)]],
      name: ['',[Validators.required,Validators.maxLength(50)]],
      status: ['Hoạt động']
    });
    ngOnInit(): void {
    }
    createLoaiNhansu(){
      this.loainhansu = new LoaiNhanSu();
     this.loainhansu.code = this.formLoaiNhansu.value.code;
     this.loainhansu.name = this.formLoaiNhansu.value.name;
     this.loainhansu.status = this.formLoaiNhansu.value.status;
     this.service.createLoainhansu(this.loainhansu).subscribe(
      (res:any)=>{
        if(!res.isError){
          this.toastr.success(res.message);
          this.service.isCreateOrUpdateOrDelete=true;
          this.dialogRef.close();
        }else{
          this.toastr.error(res.message)
        }
      }
     );
    }
}
