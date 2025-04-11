import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiemkiemtraComponent } from './diemkiemtra.component';

describe('DiemkiemtraComponent', () => {
  let component: DiemkiemtraComponent;
  let fixture: ComponentFixture<DiemkiemtraComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DiemkiemtraComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DiemkiemtraComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
