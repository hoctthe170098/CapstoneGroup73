import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ItemService } from '../shared/kehoach.service';
import { FormBuilder, Validators } from '@angular/forms';
import { KeHoach } from '../shared/kehoach.model';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-updatekehoach',
  templateUrl: './updatekehoach.component.html',
  styles: []
})
export class UpdatekehoachComponent implements OnInit {
  kehoach:KeHoach;
  constructor( @Inject(MAT_DIALOG_DATA) public data: any,public dialogRef: MatDialogRef<UpdatekehoachComponent>,
  private formBuilder: FormBuilder,private service: ItemService, private router: Router,private toastr:ToastrService) { }
  formKeHoach = this.formBuilder.group({
    id:[this.data.id],
    code: [this.data.code,[Validators.required,Validators.pattern('^[a-zA-Z0-9_-]{0,}$'),Validators.minLength(3)]],
    name: [this.data.name,[Validators.required,Validators.maxLength(50)]],
    status: [this.data.status]
  });
  ngOnInit(): void {
  }
  submitKeHoach(){
    this.kehoach = new KeHoach();
    this.kehoach.id = this.formKeHoach.value.id;
    this.kehoach.code = this.formKeHoach.value.code;
    this.kehoach.name = this.formKeHoach.value.name;
    this.kehoach.status = this.formKeHoach.value.status;
    this.service.updateKeHoach(this.kehoach).subscribe((res:any)=>{
      if(!res.isError){
         this.toastr.success(res.message);
         this.service.isCreateOrUpdateOrDelete = true;
         this.dialogRef.close();
      }else{
        this.toastr.error(res.message);
      }
    })
  }
}
