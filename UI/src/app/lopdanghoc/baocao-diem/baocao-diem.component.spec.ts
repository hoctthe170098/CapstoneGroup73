import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BaocaoDiemComponent } from './baocao-diem.component';

describe('BaocaoDiemComponent', () => {
  let component: BaocaoDiemComponent;
  let fixture: ComponentFixture<BaocaoDiemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BaocaoDiemComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BaocaoDiemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
