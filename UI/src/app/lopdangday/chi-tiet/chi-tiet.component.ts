import { Component, OnInit } from '@angular/core';
import {trigger,transition,style,animate,query,group} from '@angular/animations';
@Component({
  selector: 'app-chi-tiet',
  templateUrl: './chi-tiet.component.html',
  styleUrls: ['./chi-tiet.component.scss'],
  animations: [
    trigger('routeAnimations', [
      transition('* <=> *', [
        style({ position: 'relative' }),
        query(':enter, :leave', [
          style({
            position: 'absolute',
            top: 0,
            left: 0,
            width: '100%'
          })
        ]),
        query(':enter', [style({ opacity: 0, transform: 'translateY(10px)' })]),
        group([
          query(':leave', [animate('200ms ease-out', style({ opacity: 0 }))]),
          query(':enter', [
            animate(
              '300ms ease-out',
              style({ opacity: 1, transform: 'translateY(0)' })
            )
          ])
        ])
      ])
    ])
  ]
})
export class ChiTietComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }
  getRouteAnimation(outlet: any) {
    return outlet?.activatedRouteData?.animation || outlet?.activatedRoute?.routeConfig?.path;
  }
  
}
