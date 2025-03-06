import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChuongtrinhComponent } from './chuongtrinh.component'; 

describe('ChuongtrinhComponent', () => {
  let component: ChuongtrinhComponent;
  let fixture: ComponentFixture<ChuongtrinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChuongtrinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChuongtrinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
