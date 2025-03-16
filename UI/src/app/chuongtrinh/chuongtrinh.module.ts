import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // Thêm dòng này
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { ChuongtrinhComponent } from './chuongtrinh.component';
import { EditchuongtrinhComponent } from './editchuongtrinh/editchuongtrinh.component';
import { ChuongtrinhRoutingModule } from './chuongtrinh-routing.module';
import { AddchuongtrinhComponent } from './addchuongtrinh/addchuongtrinh.component';


@NgModule({
  declarations: [
    ChuongtrinhComponent,
    EditchuongtrinhComponent,
    AddchuongtrinhComponent,
    
  ],
  imports: [
    CommonModule,
    FormsModule,            
    RouterModule,
    ChuongtrinhRoutingModule,
    HttpClientModule
  ]
})
export class ChuongtrinhModule { }
