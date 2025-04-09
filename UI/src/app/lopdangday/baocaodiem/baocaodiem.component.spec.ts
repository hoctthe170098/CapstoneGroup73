import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BaocaodiemComponent } from './baocaodiem.component';

describe('BaocaodiemComponent', () => {
  let component: BaocaodiemComponent;
  let fixture: ComponentFixture<BaocaodiemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BaocaodiemComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BaocaodiemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
