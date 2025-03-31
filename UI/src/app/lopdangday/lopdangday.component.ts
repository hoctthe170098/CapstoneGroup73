import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';
@Component({
  selector: 'app-lopdangday',
  templateUrl: './lopdangday.component.html',
  styleUrls: ['./lopdangday.component.scss']
})
export class LopdangdayComponent implements OnInit {

  constructor( private spinner: NgxSpinnerService, private router: Router ) { }

  ngOnInit(): void {
  }
  goToDetail(lopId: string) {
    this.router.navigate([`/lopdangday/chi-tiet/${lopId}`]);
  }
}
