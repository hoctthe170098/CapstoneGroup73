import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LicdayRoutingModule } from './lichday-routing.module';
import { LichDayComponent } from './lichday.component';
// import { AccountmanagerService } from './shared/quanly.service';


@NgModule({
  declarations: [
    LichDayComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LicdayRoutingModule
  ],
  exports: [
    LicdayRoutingModule
      ],
  providers: [
    // AccountmanagerService 
  ]
})
export class LichdayModule { }
