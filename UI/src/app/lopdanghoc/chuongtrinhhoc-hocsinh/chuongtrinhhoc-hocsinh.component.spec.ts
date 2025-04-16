import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChuongtrinhhocHocsinhComponent } from './chuongtrinhhoc-hocsinh.component';

describe('ChuongtrinhhocHocsinhComponent', () => {
  let component: ChuongtrinhhocHocsinhComponent;
  let fixture: ComponentFixture<ChuongtrinhhocHocsinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChuongtrinhhocHocsinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChuongtrinhhocHocsinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
