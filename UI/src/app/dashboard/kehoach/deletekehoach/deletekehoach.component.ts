import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ItemService } from '../shared/kehoach.service';

@Component({
  selector: 'app-deletekehoach',
  templateUrl: './deletekehoach.component.html',
  styles: []
})
export class DeletekehoachComponent implements OnInit {

  constructor( @Inject(MAT_DIALOG_DATA) public data: any,public dialogRef: MatDialogRef<DeletekehoachComponent>,
  private toastr:ToastrService,private service: ItemService) { }

  ngOnInit(): void {
  }
  delete(){
      this.service.deleteKeHoach(this.data).subscribe(
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
