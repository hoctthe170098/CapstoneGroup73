import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DanhsachhocsinhComponent } from './danhsachhocsinh.component';

describe('DanhsachhocsinhComponent', () => {
  let component: DanhsachhocsinhComponent;
  let fixture: ComponentFixture<DanhsachhocsinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DanhsachhocsinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DanhsachhocsinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
