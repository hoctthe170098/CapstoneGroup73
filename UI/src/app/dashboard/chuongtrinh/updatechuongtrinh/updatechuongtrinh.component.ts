import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ChuongtrinhService } from '../shared/chuongtrinh.service';
import { ToastrService } from 'ngx-toastr';
import { Chuongtrinh } from '../shared/chuongtrinh.model';

@Component({
  selector: 'app-updatechuongtrinh',
  templateUrl: './updatechuongtrinh.component.html',
  styles: []
})
export class UpdatechuongtrinhComponent implements OnInit {
  
  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<UpdatechuongtrinhComponent>,
    private formBuilder: FormBuilder, private service: ChuongtrinhService, private toastr: ToastrService) { }
  chuongtrinh: Chuongtrinh;
  formChuongTrinh = this.formBuilder.group({
    id: [this.data.id],
    code: [this.data.code, [Validators.required, Validators.pattern('^[a-zA-Z0-9_-]{0,}$'), Validators.minLength(3)]],
    name: [this.data.name, [Validators.required, Validators.maxLength(50)]],
    status: [this.data.status]
  });
  ngOnInit(): void {
  }
  updateChuongTrinh() {
    this.chuongtrinh = new Chuongtrinh();
    this.chuongtrinh.id = this.formChuongTrinh.value.id;
    this.chuongtrinh.code = this.formChuongTrinh.value.code;
    this.chuongtrinh.name = this.formChuongTrinh.value.name;
    this.chuongtrinh.status = this.formChuongTrinh.value.status;
    console.log(this.data)
    this.service.updateChuongTrinh(this.chuongtrinh).subscribe(
      (res: any) => {
        if (!res.isError) {
          this.toastr.success(res.message);
          this.service.isCreateOrUpdateOrDelete=true;
          this.dialogRef.close();
        } else {
          this.toastr.error(res.message)
        }
      }
    );
  }
}
