import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { LoaiNhanSuService } from '../shared/loainhansu.service';
import { ToastrService } from 'ngx-toastr';
import { LoaiNhanSu } from '../shared/loainhansu.model';

@Component({
  selector: 'app-updateloainhansu',
  templateUrl: './updateloainhansu.component.html',
  styles: []
})
export class UpdateloainhansuComponent implements OnInit {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
  public dialogRef: MatDialogRef<UpdateloainhansuComponent>,
  private formBuilder: FormBuilder, private service: LoaiNhanSuService, private toastr: ToastrService) { }
loaiNhanSu: LoaiNhanSu;
formloaiNhanSu = this.formBuilder.group({
  id: [this.data.id],
  code: [this.data.code, [Validators.required, Validators.pattern('^[a-zA-Z0-9_-]{0,}$'), Validators.minLength(3)]],
  name: [this.data.name, [Validators.required, Validators.maxLength(50)]],
  status: [this.data.status]
});
ngOnInit(): void {
}
updateloaiNhanSu() {
  this.loaiNhanSu = new LoaiNhanSu();
  this.loaiNhanSu.id = this.formloaiNhanSu.value.id;
  this.loaiNhanSu.code = this.formloaiNhanSu.value.code;
  this.loaiNhanSu.name = this.formloaiNhanSu.value.name;
  this.loaiNhanSu.status = this.formloaiNhanSu.value.status;
  console.log(this.data)
  this.service.updateLoainhansu(this.loaiNhanSu).subscribe(
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
