import { Component, Inject, OnInit } from '@angular/core';
import { KeHoach } from '../shared/kehoach.model';
import { FormBuilder, NgForm, Validators } from '@angular/forms';
import { ItemService } from '../shared/kehoach.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { KehoachComponent } from '../kehoach.component';
import { error } from 'protractor';
@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styles: []
})

export class CreateComponent implements OnInit {
  constructor( @Inject(MAT_DIALOG_DATA) public data: any,public dialogRef: MatDialogRef<CreateComponent>,
    private formBuilder: FormBuilder,private service: ItemService, private router: Router,private toastr:ToastrService) { }
  keHoach : KeHoach;
  formKeHoach = this.formBuilder.group({
    code: ['',[Validators.required,Validators.pattern('^[a-zA-Z0-9_-]{0,}$'),Validators.minLength(3)]],
    name: ['',[Validators.required,Validators.maxLength(50)]],
    status: ['Hoạt động']
  });
  ngOnInit(): void {
    
  }
  submitKeHoach():void{
   this.keHoach = new KeHoach();
   this.keHoach.code = this.formKeHoach.value.code;
   this.keHoach.name = this.formKeHoach.value.name;
   this.keHoach.status = this.formKeHoach.value.status;
   this.service.createKeHoach(this.keHoach).subscribe(
    (res:any)=>{
      if(!res.isError){
        this.toastr.success(res.message);
        this.service.isCreateOrUpdateOrDelete = true;
        this.dialogRef.close();
      }else{
        this.toastr.error(res.message)
      }
    }
   );
  }
}
