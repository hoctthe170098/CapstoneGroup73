import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GiaovienRoutingModule } from './giaovien-routing.module';
import { GiaovienComponent } from './giaovien.component';
import { GiaovienService } from './shared/giaovien.service';
import { ImportGiaovienComponent } from './import-giaovien/import-giaovien.component';

@NgModule({
  declarations: [
    GiaovienComponent,
    ImportGiaovienComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    GiaovienRoutingModule
  ],
  exports: [
    GiaovienComponent
      ],
  providers: [
    GiaovienService 
  ]
})
export class GiaovienModule { }
