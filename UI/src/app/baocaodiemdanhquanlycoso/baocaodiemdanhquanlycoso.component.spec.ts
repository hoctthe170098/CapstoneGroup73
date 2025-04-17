import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BaocaodiemdanhquanlycosoComponent } from './baocaodiemdanhquanlycoso.component';

describe('BaocaodiemdanhquanlycosoComponent', () => {
  let component: BaocaodiemdanhquanlycosoComponent;
  let fixture: ComponentFixture<BaocaodiemdanhquanlycosoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BaocaodiemdanhquanlycosoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BaocaodiemdanhquanlycosoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
