import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChuongtrinhRoutingModule } from './chuongtrinh-routing.module';
import { ChuongtrinhComponent } from './chuongtrinh.component';

@NgModule({
  declarations: [ChuongtrinhComponent],
  imports: [CommonModule, ChuongtrinhRoutingModule]
})
export class ChuongtrinhModule { }
