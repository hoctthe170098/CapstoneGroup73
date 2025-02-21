import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ChuongtrinhService } from '../shared/chuongtrinh.service';
import { Chuongtrinh } from '../shared/chuongtrinh.model';

@Component({
  selector: 'app-createchuongtrinh',
  templateUrl: './createchuongtrinh.component.html',
  styles: []
})
export class CreatechuongtrinhComponent implements OnInit {
  constructor(public dialogRef: MatDialogRef<CreatechuongtrinhComponent>,
    private formBuilder: FormBuilder, private service: ChuongtrinhService, private toastr: ToastrService) { }
  chuongtrinh: Chuongtrinh;
  formChuongTrinh = this.formBuilder.group({
    code: ['', [Validators.required, Validators.pattern('^[a-zA-Z0-9_-]{0,}$'), Validators.minLength(3)]],
    name: ['', [Validators.required, Validators.maxLength(50)]],
    status: ['Hoạt động']
  });
  ngOnInit(): void {
  }
  createChuongTrinh() {
    this.chuongtrinh = new Chuongtrinh();
    this.chuongtrinh.code = this.formChuongTrinh.value.code;
    this.chuongtrinh.name = this.formChuongTrinh.value.name;
    this.chuongtrinh.status = this.formChuongTrinh.value.status;
    this.service.createChuongTrinh(this.chuongtrinh).subscribe(
      (res: any) => {
        if (!res.isError) {
          this.toastr.success(res.message);
          this.service.isCreateOrUpdateOrDelete = true;
          this.dialogRef.close();
        } else {
          this.toastr.error(res.message)
        }
      }
    );
  }
}
