import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
@Component({
  selector: 'app-lopdangday',
  templateUrl: './lopdangday.component.html',
  styleUrls: ['./lopdangday.component.scss']
})
export class LopdangdayComponent implements OnInit {

  constructor( private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
  }

}
