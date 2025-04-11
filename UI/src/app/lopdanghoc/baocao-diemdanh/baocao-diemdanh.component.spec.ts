import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BaocaoDiemdanhComponent } from './baocao-diemdanh.component';

describe('BaocaoDiemdanhComponent', () => {
  let component: BaocaoDiemdanhComponent;
  let fixture: ComponentFixture<BaocaoDiemdanhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BaocaoDiemdanhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BaocaoDiemdanhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
