import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CosoRoutingModule } from './coso-routing.module';
import { CosoComponent } from './coso.component';
@NgModule({
  declarations: [
    CosoComponent
  ],
  imports: [
    CommonModule,
    CosoRoutingModule
  ],
  exports: [ 
    CosoComponent
  ]
})
export class CosoModule { }