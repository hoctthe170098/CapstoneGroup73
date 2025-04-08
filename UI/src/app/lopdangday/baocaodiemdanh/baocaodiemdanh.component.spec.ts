import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BaocaodiemdanhComponent } from './baocaodiemdanh.component';

describe('BaocaodiemdanhComponent', () => {
  let component: BaocaodiemdanhComponent;
  let fixture: ComponentFixture<BaocaodiemdanhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BaocaodiemdanhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BaocaodiemdanhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
