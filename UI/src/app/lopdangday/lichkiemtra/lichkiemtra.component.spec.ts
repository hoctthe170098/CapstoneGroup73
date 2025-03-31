import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LichkiemtraComponent } from './lichkiemtra.component';

describe('LichkiemtraComponent', () => {
  let component: LichkiemtraComponent;
  let fixture: ComponentFixture<LichkiemtraComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LichkiemtraComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LichkiemtraComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
