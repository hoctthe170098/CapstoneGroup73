import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ChuongtrinhService } from '../shared/chuongtrinh.service';

@Component({
  selector: 'app-deletechuongtrinh',
  templateUrl: './deletechuongtrinh.component.html',
  styles: []
})
export class DeletechuongtrinhComponent implements OnInit {

  constructor( @Inject(MAT_DIALOG_DATA) public data: any,public dialogRef: MatDialogRef<DeletechuongtrinhComponent>,
  private toastr:ToastrService,private service: ChuongtrinhService) { }

  ngOnInit(): void {
  }
  delete(){
    this.service.deleteChuongTrinh(this.data).subscribe(
      (res:any)=>{
        if(!res.isError){
          this.toastr.success(res.message);
          this.service.isCreateOrUpdateOrDelete = true;
          this.dialogRef.close();
        }else{
          this.toastr.error(res.message)
        }
      }
    )
  }
}
