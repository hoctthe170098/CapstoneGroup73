import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { LoaiNhanSuService } from '../shared/loainhansu.service';

@Component({
  selector: 'app-deleteloainhansu',
  templateUrl: './deleteloainhansu.component.html',
  styles: []
})
export class DeleteloainhansuComponent implements OnInit {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<DeleteloainhansuComponent>,
    private toastr: ToastrService, private service: LoaiNhanSuService) { }

  ngOnInit(): void {
  }
  delete() {
    this.service.deleteLoainhansu(this.data).subscribe(
      (res: any) => {
        if (!res.isError) {
          this.toastr.success(res.message);
          this.service.isCreateOrUpdateOrDelete = true;
          this.dialogRef.close();
        } else {
          this.toastr.error(res.message)
        }
      }
    )
  }
}
