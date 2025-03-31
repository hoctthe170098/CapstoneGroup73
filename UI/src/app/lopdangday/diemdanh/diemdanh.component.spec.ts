import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiemdanhComponent } from './diemdanh.component';

describe('DiemdanhComponent', () => {
  let component: DiemdanhComponent;
  let fixture: ComponentFixture<DiemdanhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DiemdanhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DiemdanhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
